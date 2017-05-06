using UnityEngine;
using System.Collections.Generic;

public class CaravanManager : MonoBehaviour {

	public List<Transform> m_Caravans = new List<Transform>();

    Kojima.CaravanScoreRace currentRaceMode;

    bool m_isCaravanGrappled = false;
    Transform m_grappledCaravan;

	public void addCaravan(Transform _caravan)
	{
		if(m_Caravans.Count == 0)
		{
			Vector3 spawnPos = GetComponent<Bam.CarSockets> ().GetSocket (Bam.CarSockets.Sockets.LowRear).position;

			Transform madeCaravan = Instantiate (_caravan, spawnPos, transform.rotation) as Transform;
			madeCaravan.transform.parent = transform;
			madeCaravan.GetComponent<CaravanController> ().Start();
            madeCaravan.GetComponent<CaravanController>().setManager(this);
            m_Caravans.Add(madeCaravan);
		}
        else
        {
            Vector3 spawnPos = m_Caravans[m_Caravans.Count-1].GetComponent<Bam.CarSockets>().GetSocket(Bam.CarSockets.Sockets.LowRear).position;

            Transform madeCaravan = Instantiate(_caravan, spawnPos, m_Caravans[m_Caravans.Count - 1].rotation) as Transform;
            madeCaravan.transform.parent = m_Caravans[m_Caravans.Count - 1];
            madeCaravan.GetComponent<CaravanController>().Start();
            madeCaravan.GetComponent<CaravanController>().setManager(this);

            m_Caravans.Add(madeCaravan);
        }
	}

    public void removeCaravan(Transform _Caravan)
    {
        bool remove = false;

        for (int i = 0; i < m_Caravans.Count; i++)
        {
            if (m_Caravans[i] == _Caravan)
            {
                remove = true;
            }

            if (remove)
            {
                m_Caravans.RemoveAt(i);
                i--;
            }
        }
    }

    public void setIsCaravanGrappled(bool _newBool)
    {
        if (_newBool != m_isCaravanGrappled)
        {
            m_isCaravanGrappled = _newBool;

            if (_newBool)
            {
                if (currentRaceMode != null)
                {
                    currentRaceMode.carHasCaravan(transform);
                }
            }
            else
            {
                if (currentRaceMode != null)
                {
                    currentRaceMode.carDroppedCaravan(transform);
                }

                m_grappledCaravan = null;
            }
        }
    }
    public bool getIsCaravanGrappled()
    {
        return m_isCaravanGrappled;
    }

    public void setGrappledCaravan(Transform _newCaravan)
    {
        m_grappledCaravan = _newCaravan;
    }
    public Transform getGrappledCaravan()
    {
        return m_grappledCaravan;
    }

    public void setCaravanScoreRace(Kojima.CaravanScoreRace _NewRace)
    {
        currentRaceMode = _NewRace;
    }
}
