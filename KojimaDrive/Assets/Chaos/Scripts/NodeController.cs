using UnityEngine;
using System.Collections;

public class NodeController : MonoBehaviour {

    [SerializeField] GameObject m_nodePrefab;

    GameObject m_Target;

	bool m_LastNode = true;
	LineRenderer m_RopeRenderer;
	GrappleRopeManager m_RopeManager;

    float m_fRopeLength = 0, m_fMinNodeDistance = 5;

	void Start()
	{
        m_RopeRenderer = GetComponent<LineRenderer> ();

		if(GetComponent<GrappleRopeManager>())
		{
            m_RopeManager = GetComponent<GrappleRopeManager> ();
		}
	}

	void Update ()
	{
        if (m_Target != null)
        {
            m_RopeRenderer.SetPosition(0, transform.position);
            m_RopeRenderer.SetPosition(1, m_Target.transform.position);
            m_fRopeLength = Vector3.Distance(transform.position, m_Target.transform.position);

            RaycastHit m_hit;

            if (m_RopeManager.canAddNode())
            {
                if (Physics.Raycast(transform.position, m_Target.transform.position - transform.position, out m_hit))
                {
					if (m_hit.collider.gameObject != m_Target && !m_hit.collider.transform.root.GetComponent<Kojima.CarScript>() && !m_hit.collider.transform.GetComponent<NodeController>())
                    {
                        // bounce the swapn position off the normal of the object we m_hit a little bit (0.2 of a unit)
                        Vector3 spawnPoint = (Vector3.Reflect(m_hit.point - transform.position, m_hit.normal) / 10) + m_hit.point;
                        spawnPoint.y = m_hit.point.y;

						if(Vector3.Distance(transform.position, spawnPoint) > m_fMinNodeDistance)
						{
	                        //create a node at hit position
	                        GameObject newNode = Instantiate(m_nodePrefab, spawnPoint, Quaternion.identity) as GameObject;
	                        newNode.transform.parent = m_hit.transform;
	                        newNode.transform.localScale = new Vector3(1 / newNode.transform.parent.localScale.x,
                                                                       1 / newNode.transform.parent.localScale.y,
                                                                       1 / newNode.transform.parent.localScale.z) * 0.1f;
                            newNode.GetComponent<NodeController>().setManager(m_RopeManager);
	                        newNode.GetComponent<NodeController>().m_Target = m_Target;

                            m_Target = newNode;

	                        if (m_LastNode)
	                        {
	                            newNode.GetComponent<NodeController>().m_LastNode = true;
                                m_RopeManager.setLastNode(newNode);
                                m_LastNode = false;
	                        }

                            m_RopeManager.addNode(newNode);
						}
                    }
                }
            }
            
            if (!m_LastNode && m_Target.GetComponent<NodeController>().m_Target != null)
            {
                //ray to target's target
                if (Physics.Raycast(transform.position, m_Target.GetComponent<NodeController>().m_Target.transform.position - transform.position, out m_hit))
                {
                    if (m_hit.collider.gameObject == m_Target.GetComponent<NodeController>().m_Target)
                    {
                        if (m_Target.GetComponent<NodeController>().m_LastNode)
                        {
                            m_LastNode = true;
                            m_RopeManager.setLastNode(gameObject);
                        }

                        GameObject m_TT = m_Target.GetComponent<NodeController>().m_Target;
                        // call delete on the target
                        m_RopeManager.GetComponent<GrappleRopeManager>().killNode(m_Target);
                        m_Target = m_TT;
                    }
                }
            }

			if(m_Target != m_RopeManager && m_Target == m_RopeManager.GetComponent<GrappleRopeManager>().getLastNode() 
                                 && Vector3.Distance(m_Target.transform.position, m_Target.GetComponent<NodeController>().m_Target.transform.position) < m_fMinNodeDistance)
			{
                m_LastNode = true;
                m_RopeManager.setLastNode(gameObject);
				GameObject m_TT = m_Target.GetComponent<NodeController>().m_Target;
                m_RopeManager.GetComponent<GrappleRopeManager>().killNode(m_Target);
                m_Target = m_TT;
			}
        }
    }

	public void setTarget(GameObject _newTarget)
	{
        m_Target = _newTarget;
	}
	public GameObject getTarget()
	{
		return m_Target;
	}
	public void setManager(GrappleRopeManager _newManager)
	{
        m_RopeManager = _newManager;
	}

	public float getRopeLength()
	{
		return m_fRopeLength;
	}
}
