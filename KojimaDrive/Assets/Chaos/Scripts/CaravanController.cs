using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CaravanController : MonoBehaviour
{
    [SerializeField] List<GameObject> m_Wheels = new List<GameObject>();
    [SerializeField] int m_iMaxHP;
    int m_iHP;

    CaravanManager m_Manager;
    bool m_isdetached = false;

    public void Start()
	{
		if (transform.parent != null && transform.parent.GetComponent<Rigidbody>())
		{
			GetComponent<ConfigurableJoint>().connectedBody = transform.parent.GetComponent<Rigidbody>();
			transform.parent = null;
		}

        m_iHP = m_iMaxHP;
	}
    
	void Update ()
    {
        if (!m_isdetached && m_iHP == (m_iMaxHP / 10))
        {
            foreach (GameObject wheel in m_Wheels)
            {
                wheel.transform.parent = null;
                wheel.AddComponent<Rigidbody>();
                m_iHP--;
            }
        }
        if (!m_isdetached && m_iHP <= 0)
        {
            detach();

            if (m_Manager != null)
            {
                m_Manager.removeCaravan(transform);
            }
		}
	}

	void OnCollisionEnter(Collision col)
	{
		//take away from HP
		if (!m_isdetached)
        {
            m_iHP--;
		}
	}

    public void setManager(CaravanManager _newManager)
    {
        m_Manager = _newManager;
    }

    public void detach()
    {
        Destroy(GetComponent<ConfigurableJoint>());
        m_isdetached = true;
    }

    void OnDestroy()
    {
        foreach (GameObject wheel in m_Wheels)
        {
            Destroy(wheel);
        }
    }
}