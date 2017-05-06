//Author:       TMS
//Description:  Script that creates skid marks when the attached CarScript reports that it is skidding
//              
//Last edit:    Harrison @ 02/05/2017

using UnityEngine;
using System.Collections.Generic;


namespace Bam
{
    public class SkidEffectsScript : MonoBehaviour
    {
        public GameObject m_skidSmokePrefab;
        public GameObject m_skidMarkPrefab;

        //TrailRenderer[] m_skidTrails;
        //ParticleSystem[] m_skidMarkParticles;
        ParticleSystem[] m_skidSmokeParticles;

        bool[] m_trailActive = new bool[4];

        Kojima.CarScript m_myCar;


		private struct SkidMeshData
		{
			public SkidMeshData(Mesh mesh, Material mat, Vector3 position)
			{
				this.mesh = mesh;
				this.mat = mat;
				this.position = position;

				creationTime = Time.time;
			}

			public Mesh mesh;
			public Material mat;
			public float creationTime;

			public Vector3 position;
		}

		public Material skidMaterial;
		public float skidWidth = 0.3f;
		public float minSkidLength = 0.5f;
		public float skidLifetime = 13f;
		public float skidFadeOutTime = 3f;
		private HashSet<SkidMeshData> skidMarks = new HashSet<SkidMeshData>();
		private SkidMeshData[] currentSkidMeshes;



        void Awake()
        {
            m_myCar = GetComponent<Kojima.CarScript>();

            CreateTrails();
        }
		
        void CreateTrails()
        {
            //m_skidTrails = new TrailRenderer[4];
            //m_skidMarkParticles = new ParticleSystem[4];
            m_skidSmokeParticles = new ParticleSystem[4];
			
			currentSkidMeshes = new SkidMeshData[4];


			for (int i = 0; i < m_skidSmokeParticles.Length; i++)
            {
                //m_skidTrails[i] = Instantiate<GameObject>(m_skidMarkPrefab).GetComponent<TrailRenderer>();
                //m_skidMarkParticles[i] = m_skidTrails[i].GetComponent<ParticleSystem>();

                m_skidSmokeParticles[i] = Instantiate<GameObject>(m_skidSmokePrefab).GetComponent<ParticleSystem>();

                //m_skidTrails[i].gameObject.hideFlags = HideFlags.HideInHierarchy;
                m_skidSmokeParticles[i].gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
        }
		
        void OnDestroy()
		{
			for(int i = 0; i < m_skidSmokeParticles.Length; i++)
			{
                //if (m_skidMarkParticles[i])
                //{
                //    Destroy(m_skidMarkParticles[i].gameObject);
                //}

                //if (m_skidTrails[i])
                //{
                //    Destroy(m_skidTrails[i].gameObject);
                //}

                if (m_skidSmokeParticles[i])
                {
                    Destroy(m_skidSmokeParticles[i].gameObject);
                }
            }


			foreach(SkidMeshData skidData in skidMarks)
			{
				Destroy(skidData.mesh);
				Destroy(skidData.mat);
			}
        }

		void Update()
		{
			//We dont use GameObjects for each skid mark because of the overhead that would create
			//Instead we store the meshes, positions, and materials, and draw them ourselves

			HashSet<SkidMeshData> remove = null;

			foreach(SkidMeshData skidData in skidMarks)
			{
				//Update the material's alpha so we can fade out the old ones
				float alpha = Mathf.Clamp01(Mathf.InverseLerp(skidLifetime, skidLifetime - skidFadeOutTime, Time.time - skidData.creationTime));

				if(alpha <= 0f)
				{
					if(remove == null)
					{
						//We only create this here because this doesnt happen too often, and it saves GC
						remove = new HashSet<SkidMeshData>();
					}
					remove.Add(skidData);

					continue;
				}

				skidData.mat.SetFloat("_Alpha", alpha);

				//Draw the mesh
				Graphics.DrawMesh(skidData.mesh, Matrix4x4.TRS(skidData.position, Quaternion.identity, Vector3.one), skidData.mat, LayerMask.NameToLayer("Ignore Raycast"));
			}

			if(remove != null)
			{
				foreach(SkidMeshData skidData in remove)
				{
					Destroy(skidData.mesh);
					Destroy(skidData.mat);

					skidMarks.Remove(skidData);
				}
			}


			//Debug.Log(skidMarks.Count + " skid marks");
		}

        void LateUpdate()
        {
            RaycastHit[] wheelCasts = m_myCar.GetWheelRaycasts;

			Vector3 velocity = GetComponent<Rigidbody>().velocity;

            for (int i = 0; i < m_skidSmokeParticles.Length; i++)
            {               
                if (m_myCar.IsWheelSkidding(i) && m_myCar.IsWheelGrounded(i))
                {
                    m_skidSmokeParticles[i].transform.position = wheelCasts[i].point;

					Vector3 normal = wheelCasts[i].normal;
					Vector3 skidPoint = wheelCasts[i].point + normal * 0.05f;
					Vector3 right = Vector3.Cross(normal, velocity).normalized;

					//m_skidTrails[i].transform.position = skidPoint;

					
					if(m_trailActive[i])
                    {
						//Update the skid mark's mesh
						UpdateSkidMark(currentSkidMeshes[i], skidPoint, normal, right);
					}
                    else
					{
						m_skidSmokeParticles[i].Play();
						//m_skidMarkParticles[i].Play();
						m_trailActive[i] = true;


						//Create a new skid mark

						currentSkidMeshes[i] = NewSkidMark(skidPoint, normal, right);
					}
				}
                else
                {
                    m_skidSmokeParticles[i].Stop();
                    //m_skidMarkParticles[i].Pause();
                    m_trailActive[i] = false;
                }

            }
        }

		private SkidMeshData NewSkidMark(Vector3 position, Vector3 normal, Vector3 right)
		{
			Mesh mesh = new Mesh();
			mesh.MarkDynamic();

			mesh.vertices = new Vector3[]
			{
				right * -0.5f * skidWidth,
				right *  0.5f * skidWidth,
			};
			mesh.normals = new Vector3[]
			{
				normal,
				normal,
			};
			mesh.uv = new Vector2[]
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
			};

			//mesh.triangles = new int[0]; //No triangles yet

			SkidMeshData skidData = new SkidMeshData(mesh, new Material(skidMaterial), position);

			skidMarks.Add(skidData);
			return skidData;
		}
		private void UpdateSkidMark(SkidMeshData meshData, Vector3 position, Vector3 normal, Vector3 right)
		{
			List<Vector3> verts = new List<Vector3>(meshData.mesh.vertices);

			//Position of the current iteration relative to meshData.position
			Vector3 offset = position - meshData.position;

			//Center point of the previous iteration
			Vector3 previousCenter = (verts[verts.Count - 2] + verts[verts.Count - 1]) / 2f;

			//Square distance traveled this iteration
			float distSqr = (offset - previousCenter).sqrMagnitude;

			if(distSqr < minSkidLength * minSkidLength)
			{
				//Too small of a distance, skip this iteration
				return;
			}

			List<Vector3> norms = new List<Vector3>(meshData.mesh.normals);
			List<Vector2> uv = new List<Vector2>(meshData.mesh.uv);
			List<int> tris = new List<int>(meshData.mesh.triangles);

			//Actual distance traveled this iteration
			float dist = Mathf.Sqrt(distSqr);

			//Add two verts
			verts.Add(offset + right * -0.5f * skidWidth);
			verts.Add(offset + right * 0.5f * skidWidth);

			norms.Add(normal);
			norms.Add(normal);

			uv.Add(new Vector2(0f, uv[uv.Count - 2].y + dist));
			uv.Add(new Vector2(1f, uv[uv.Count - 2].y + dist));


			//Build two new triangles

			tris.Add(verts.Count - 4);
			tris.Add(verts.Count - 2);
			tris.Add(verts.Count - 3);

			tris.Add(verts.Count - 3);
			tris.Add(verts.Count - 2);
			tris.Add(verts.Count - 1);


			//Apply changes to the mesh
			meshData.mesh.vertices = verts.ToArray();
			meshData.mesh.normals = norms.ToArray();
			meshData.mesh.uv = uv.ToArray();
			meshData.mesh.triangles = tris.ToArray();

			meshData.mesh.RecalculateBounds();


			//Reset the timer so that it doesnt despawn while its in use
			meshData.creationTime = Time.time;


			//Debug.Log("Updating skid mesh " + i + ": " + verts.Count + " verts, " + tris.Count + " tris. Pos: " + meshData.position);

			//Debug.DrawLine(skidPoint + right * -0.5f * skidWidth, skidPoint + right * 0.5f * skidWidth, Color.white, 3f);
		}

	}
}