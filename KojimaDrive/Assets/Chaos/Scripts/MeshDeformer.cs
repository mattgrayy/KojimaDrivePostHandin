using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class DeformChunk
{
    //list of verticies that are to be changed
    public List<int> vertexIndexes = new List<int>();

    public List<GameObject> destructableObjects = new List<GameObject>();

    //the pooint on the car this chunk is related to
    public GameObject chunkLocation;

    //has this chunk been deformed yet?
    public int deformation = 0;

    public int getVertexListCount()
    {
        return vertexIndexes.Count;
    }

    public void AddToVertexList(int index)
    {
        vertexIndexes.Add(index);
    }
}

public class MeshDeformer : MonoBehaviour
{
    [SerializeField] List<GameObject> deformChunkObjects = new List<GameObject>();
    [SerializeField] List<GameObject> ignoreObjects = new List<GameObject>();
	[SerializeField] List<GameObject> lights =  new List<GameObject>();

    List<DeformChunk> deformChunks = new List<DeformChunk> ();

    Mesh baseMesh;
    Mesh m_deformingMesh;
	Vector3[] m_currentVerticies;

    [SerializeField] float m_fdmgMod = 1;

    bool changedMesh = false;

    float m_fTimer = 5;

    void Start()
	{
		//is this script is attached to a car or not
		if (GetComponent<Kojima.CarScript> ())
        {
            baseMesh = GetComponent<Kojima.CarScript>().GetCarBody().transform.FindChild("Body").GetComponent<MeshFilter>().sharedMesh;
            //it is a car
            m_deformingMesh = GetComponent<Kojima.CarScript> ().GetCarBody ().transform.FindChild("Body").GetComponent<MeshFilter> ().mesh;
        }
        else
        {
            baseMesh = transform.GetComponent<MeshFilter>().sharedMesh;
            //it is somthing else
            m_deformingMesh = transform.GetComponent<MeshFilter> ().mesh;
            if (GetComponent<MeshCollider>())
            {
                GetComponent<MeshCollider>().sharedMesh = m_deformingMesh;
            }
		}

		m_currentVerticies = m_deformingMesh.vertices;

		ChunkSetUp ();
	}

	void ChunkSetUp()
	{
        if (deformChunkObjects.Count != 0)
        {
            foreach (GameObject chunk in deformChunkObjects)
            {
                DeformChunk newChunk = new DeformChunk();
                newChunk.chunkLocation = chunk;

				//set up the lights 
				if (chunk.name == "LIGHTPOS_FL") {
					newChunk.destructableObjects.Add(lights [0]);
				}

				if (chunk.name == "LIGHTPOS_FR") {
					newChunk.destructableObjects.Add(lights [1]); 
				}

				if (chunk.name == "LIGHTPOS_BL") {
					newChunk.destructableObjects.Add(lights [2]);
					newChunk.destructableObjects.Add(lights [3]);
					newChunk.destructableObjects.Add(lights [4]);
				}

				if (chunk.name == "LIGHTPOS_BR") {
					newChunk.destructableObjects.Add(lights [5]);
					newChunk.destructableObjects.Add(lights [6]);
					newChunk.destructableObjects.Add(lights [7]);
				}

				if (chunk.name == "LOW FRONT")
				{
					if(lights.Count > 8)
					{
						newChunk.destructableObjects.Add(lights [8]);
					}
				}

                deformChunks.Add(newChunk);
            }

            int chunkSize = m_currentVerticies.Length / deformChunks.Count;
            int vertIndex = 0;

            //set up the chuncks
			//foireach vertex of the mesh
            foreach (Vector3 vert in m_currentVerticies)
            {
                float tempClosestDist = float.MaxValue;
                int tempChunkIndex = 0;


				//for each deform chunk
                for (int i = 0; i < deformChunks.Count; i++)
                {
                    Vector3 convertedPoint = transform.InverseTransformPoint(deformChunks[i].chunkLocation.transform.position);

                    if (Vector3.Distance(vert, convertedPoint) < tempClosestDist)
                    {
                        tempChunkIndex = i;
                        tempClosestDist = Vector3.Distance(vert, convertedPoint);
                    }
                }

                //add the vertex to the correct chunk
                if (deformChunks[tempChunkIndex].getVertexListCount() < chunkSize)
                {
                    deformChunks[tempChunkIndex].AddToVertexList(vertIndex);
                }
                else
                {
                    deformChunks[tempChunkIndex].AddToVertexList(vertIndex);
                    removeFurthestChunkVertex(tempChunkIndex, chunkSize);
                }

                vertIndex++;
            }
        }
	}

	void removeFurthestChunkVertex(int _chunkIndex, float _chunkSize)
	{
		int furthestIndex = 0;
		float furthestDistance = 0;

		Vector3 convertedChunkPos = transform.InverseTransformPoint (deformChunks[_chunkIndex].chunkLocation.transform.position);

		for(int i = 0; i < deformChunks[_chunkIndex].getVertexListCount(); i++)
		{
			if(Vector3.Distance(m_currentVerticies[deformChunks[_chunkIndex].vertexIndexes[i]], convertedChunkPos) > furthestDistance)
			{
				furthestIndex = i;
				furthestDistance = Vector3.Distance (m_currentVerticies[deformChunks[_chunkIndex].vertexIndexes[i]], convertedChunkPos);
			}
		}

		int vertexIndextoAdd = deformChunks [_chunkIndex].vertexIndexes [furthestIndex];

		deformChunks[_chunkIndex].vertexIndexes.RemoveAt (furthestIndex);

		// List of chunks in order of distance to vertex to add.
		List<int> orderedChunks = new List<int> ();

		for (int i = 0; i < deformChunks.Count; i++)
		{
			// Just add first chunk
			if(i == 0)
			{
				orderedChunks.Add (i);
				continue;
			}

			bool added = false;

			// Is the chunk (i) closer to the vertex than a chunk in the list
			for(int orderIndex = 0; orderIndex < orderedChunks.Count; orderIndex++)
			{
				if(Vector3.Distance(transform.InverseTransformPoint (deformChunks[i].chunkLocation.transform.position), m_currentVerticies[vertexIndextoAdd])
					< Vector3.Distance(transform.InverseTransformPoint (deformChunks[orderedChunks[orderIndex]].chunkLocation.transform.position), m_currentVerticies[vertexIndextoAdd]))
				{
					orderedChunks.Insert (orderIndex, i);
					added = true;
					break;
				}
			}
			// chunk (i) wasn't closer than any previous chunks so add it to the end of list
			if(!added)
			{
				orderedChunks.Add (i);
			}
		}
		
		foreach(int chunkIndex in orderedChunks)
		{
			// closest chunk is not full so add vertex
			if(deformChunks[chunkIndex].getVertexListCount() < _chunkSize)
			{
				deformChunks [chunkIndex].AddToVertexList (vertexIndextoAdd);
				break;
			}

			Vector3 tempChunkPos = transform.InverseTransformPoint (deformChunks[chunkIndex].chunkLocation.transform.position);

			// see if we are closer than any vertices in chunk
			foreach(int vertIndex in deformChunks [chunkIndex].vertexIndexes)
			{
				if(Vector3.Distance(tempChunkPos, m_currentVerticies[vertexIndextoAdd]) < Vector3.Distance(tempChunkPos, m_currentVerticies[vertIndex]))
				{
					deformChunks[chunkIndex].AddToVertexList (vertexIndextoAdd);
					//removeFurthestChunkVertex (chunkIndex, _chunkSize);
					return;
				}
			}
		}
	}

	void Update()
	{
        if (m_fTimer <= 0)
        {
            if (m_deformingMesh != null)
            {
                if (changedMesh)
                {
                    m_deformingMesh.vertices = m_currentVerticies;

                    m_deformingMesh.RecalculateNormals();

                    changedMesh = false;
                }
            }
            else
            {
                if (GetComponent<Kojima.CarScript>())
                {
                    //it is a car
                    m_deformingMesh = GetComponent<Kojima.CarScript>().GetCarBody().transform.FindChild("Body").GetComponent<MeshFilter>().mesh;
                }
            }
        }
        else
        {
            m_fTimer -= Time.deltaTime;
        }
    }

	void OnCollisionEnter(Collision col)
	{
        if (m_fTimer <= 0)
        {
            // ignore collision if it isn't violent enough or we have no chunks
            if (col.relativeVelocity.magnitude < 10 || deformChunkObjects.Count == 0)
            {
                return;
            }

            // also ignore collision if it is with one of our ignore objects
            foreach (GameObject ignoreObject in ignoreObjects)
            {
                if (col.gameObject == ignoreObject || col.gameObject == gameObject)
                {
                    return;
                }

                foreach (ContactPoint Contact in col.contacts)
                {
                    if (Contact.thisCollider.gameObject == ignoreObject)
                    {
                        return;
                    }
                }
            }

            //get contact point and get the 10 clostest surounding vertices
            foreach (ContactPoint Contact in col.contacts)
            {
                // target for deformation ot move towards
                Vector3 deformTarget = Vector3.zero;
                Vector3 contact = Contact.point;

                int closestChunkIndex = 0;
                float closestChunkDistance = float.MaxValue;

                // Find the closest chunk
                for (int i = 0; i < deformChunks.Count; i++)
                {
                    Vector3 convertedChunkPos = deformChunks[i].chunkLocation.transform.position;

                    deformTarget += convertedChunkPos;

                    if (Vector3.Distance(contact, convertedChunkPos) < closestChunkDistance)
                    {
                        closestChunkIndex = i;
                        closestChunkDistance = Vector3.Distance(contact, convertedChunkPos);
                    }
                }

                // get the average location of the chunks for deform targt
                deformTarget /= deformChunks.Count;
                deformTarget = transform.InverseTransformPoint(deformTarget);

                if (deformChunks[closestChunkIndex].deformation < 100)
                {
                    // set the base modifiers for the deformation
                    int prevVertChangeModX = Random.Range(-4, 4);
                    int prevVertChangeModY = Random.Range(-4, 4);
                    int prevVertChangeModZ = Random.Range(-4, 4);

                    foreach (int index in deformChunks[closestChunkIndex].vertexIndexes)
                    {
						Vector3 changeValue = ((deformTarget - m_currentVerticies[index]) / 25) * m_fdmgMod;

                        // clamp the value the can be deformed to avoid rare bugs
						if(Vector3.Distance(changeValue, m_currentVerticies[index]) <= 3)
						{
                            // change the modievfiers slightly clamped ot values
                            prevVertChangeModX = Mathf.Clamp(Random.Range(prevVertChangeModX - 2, prevVertChangeModX + 2), -4, 4);
                            prevVertChangeModY = Mathf.Clamp(Random.Range(prevVertChangeModY - 2, prevVertChangeModY + 2), -4, 4);
                            prevVertChangeModZ = Mathf.Clamp(Random.Range(prevVertChangeModZ - 2, prevVertChangeModZ + 2), -4, 4);

                            changeValue = new Vector3((changeValue.x + ((changeValue.x / 10) * prevVertChangeModX)),
                                                      (changeValue.y + ((changeValue.y / 10) * prevVertChangeModX)),
                                                      (changeValue.z + ((changeValue.z / 10) * prevVertChangeModZ)));

							m_currentVerticies [index] += changeValue;
						}
                    }

                    foreach (GameObject destructableObject in deformChunks[closestChunkIndex].destructableObjects)
                    {
                        if (destructableObject != null)
                        {
                            if (deformChunks[closestChunkIndex].chunkLocation.name == "LOW FRONT")
                            {
                                destructableObject.transform.parent = null;
                                destructableObject.AddComponent<Draggable>();
                                destructableObject.AddComponent<BoxCollider>();
                                destructableObject.AddComponent<Rigidbody>();
                                destructableObject.GetComponent<Rigidbody>().mass = GetComponent<Rigidbody>().mass / 10;
                            }
                            else
                            {
                                destructableObject.GetComponent<MeshRenderer>().enabled = false;
                            }
                        }
                    }

                    deformChunks[closestChunkIndex].deformation += 15;
                }
            }

            changedMesh = true;
        }
	}
	
	public void resetDeformation()
	{
        if (baseMesh != null)
        {
            // need new chunk calculations
            deformChunks.Clear();

            // make a new mesh and set its vertices and sumbeshes (for materials) to match
            Mesh newMesh = new Mesh();

            newMesh.vertices = baseMesh.vertices;
            newMesh.subMeshCount = baseMesh.subMeshCount;

            // this is needed to match the triangles that relate to each submesh's materials
            for (int subMeshIndex = 0; subMeshIndex < newMesh.subMeshCount; subMeshIndex++)
            {
                newMesh.SetTriangles(baseMesh.GetTriangles(subMeshIndex), subMeshIndex);
            }
            // this makes the lighting work immediately
            newMesh.RecalculateNormals();

            // set up just like on start
            GetComponent<Kojima.CarScript>().GetCarBody().transform.FindChild("Body").GetComponent<MeshFilter>().mesh = newMesh;

            m_deformingMesh = GetComponent<Kojima.CarScript>().GetCarBody().transform.FindChild("Body").GetComponent<MeshFilter>().mesh;

            m_currentVerticies = m_deformingMesh.vertices;
            ChunkSetUp();

            // reset the lights!
            foreach (GameObject light in lights)
            {
                if (light != null && light.GetComponent<MeshRenderer>())
                {
                    light.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }
	}
}
