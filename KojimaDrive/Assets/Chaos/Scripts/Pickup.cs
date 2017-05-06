using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

    public enum PickupType_e
    {
        None, Caravan, Grapple, Glider, Random//, Bomb
    }

    public enum PickupLimitation_e
    {
        Infinite, Timed//, Uses
    }

    public PickupType_e m_PickupType;
    public PickupLimitation_e m_PickupLimitation;

    Transform m_Box;

    public float m_fRespawnTimer = 5.0f;
    float m_fRespawnCountdown = 0.0f;

    int m_nReference;
    int m_nRandomRange = 4;

    void Start ()
    {
        BoxSelect();
	}
	
	void Update ()
    {
        if (!m_Box.gameObject.activeInHierarchy)
        {
            m_fRespawnCountdown -= Time.deltaTime;
            if(m_fRespawnCountdown <= 0)
            {
                m_Box.gameObject.SetActive(true);
            }
        }

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Kojima.CarScript>() && m_Box.gameObject.activeInHierarchy)
        {
            int m_nPlayerNum = col.GetComponent<Kojima.CarScript>().m_nplayerIndex - 1;
            if (m_PickupType == PickupType_e.Random)
            {
                m_nReference = Random.Range(1, m_nRandomRange);
            }

            AddonManager.m_instance.PickupConverter(m_nReference , m_nPlayerNum, true);
            
            if (m_PickupLimitation == PickupLimitation_e.Timed)
            {
                m_Box.gameObject.SetActive(false);
                m_fRespawnCountdown = m_fRespawnTimer;
            }
        }
    }

    void BoxSelect()
    {
        switch (m_PickupType)
        {
            case PickupType_e.Random:
                m_Box = transform.FindChild("Random");
                break;
            case PickupType_e.Caravan:
                m_Box = transform.FindChild("Caravan");
                m_nReference = 1;
                break;
            case PickupType_e.Grapple:
                m_Box = transform.FindChild("Grapple");
                m_nReference = 2;
                break;
            case PickupType_e.Glider:
                m_Box = transform.FindChild("Glider");
                m_nReference = 3;
                break;
            //case PickupType_e.Bomb:
            //    m_Box = transform.FindChild("Bomb");
            //    m_nReference = 4;
            //    break;
        };
        m_Box.gameObject.SetActive(true);
    }
}
