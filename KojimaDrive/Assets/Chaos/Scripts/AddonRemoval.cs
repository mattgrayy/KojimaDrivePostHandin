using UnityEngine;
using System.Collections;

public class AddonRemoval : MonoBehaviour
{

    public enum RemoveSelection_e
    {
        None, Caravan, Grapple, Glider, All
    }
    public RemoveSelection_e m_RemoveSelection;

    int m_nReference;

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Kojima.CarScript>())
        {
            int m_nPlayerNum = col.GetComponent<Kojima.CarScript>().m_nplayerIndex - 1;
            if (m_RemoveSelection == RemoveSelection_e.All)
            {
                AddonManager.m_instance.PickupConverter(1, m_nPlayerNum, false);
                AddonManager.m_instance.PickupConverter(2, m_nPlayerNum, false);
                m_nReference = 3;
            }
            AddonManager.m_instance.PickupConverter(m_nReference, m_nPlayerNum, false);
        }
    }

    void Start()
    {
        switch (m_RemoveSelection)
        {
            case RemoveSelection_e.Caravan:
                m_nReference = 1;
                break;
            case RemoveSelection_e.Grapple:
                m_nReference = 2;
                break;
            case RemoveSelection_e.Glider:
                m_nReference = 3;
                break;
        };
    }
}
