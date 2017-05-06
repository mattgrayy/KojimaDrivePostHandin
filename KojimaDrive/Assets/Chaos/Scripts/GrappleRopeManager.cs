using UnityEngine;
using System.Collections.Generic;

public class GrappleRopeManager : MonoBehaviour {

    List<GameObject> m_Nodes = new List<GameObject>();

    GameObject m_Launcher;
    GameObject m_LastNode;

    [SerializeField] int m_iMaxNodes;

	void Awake()
	{
        m_LastNode = gameObject;
	}

    public float calculateRopeLength()
    {
        float m_flength = 0;

        foreach (GameObject node in m_Nodes)
        {
			if(node != null)
			{
                m_flength += node.GetComponent<NodeController>().getRopeLength();
			}
        }
        m_flength += GetComponent<NodeController>().getRopeLength();

        return m_flength;
    }

    public bool canAddNode()
    {
        if (m_Nodes.Count >= m_iMaxNodes)
        {
            return false;
        }
        return true;
    }

    public void addNode(GameObject node)
    {
        m_Nodes.Add(node);
    }

    public void setLastNode(GameObject node)
    {
        m_LastNode = node;
    }

    public void killAllNodes()
    {
        foreach (GameObject node in m_Nodes)
        {
            Destroy(node);
        }

		m_Launcher.GetComponent<GrappleLaunch> ().resetGrappled ();
        Destroy(gameObject);
    }

    public void killNode(GameObject _TargetNode)
    {
        int targetIndex = 0;

        for (int i = 0; i < m_Nodes.Count; i++)
        {
            if (m_Nodes[i] == _TargetNode)
            {
                targetIndex = i;
                break;
            }
        }

        Destroy(m_Nodes[targetIndex]);
        m_Nodes.RemoveAt(targetIndex);
    }

	public void addLauncher(Transform _launcher)
	{
        m_Launcher = _launcher.gameObject;
	}

	public GameObject getLastNode()
	{
		return m_LastNode;
	}

    public GameObject getLauncher()
    {
        return m_Launcher;
    }
}
