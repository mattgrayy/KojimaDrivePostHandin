using UnityEngine;
using System.Collections;

namespace HF
{
    //===================== Kojima Drive - Half-Full 2017 ====================//
    //
    // Author: ADAM MOOREY
    // Purpose: Handles the deletion based on contact with a chaser
    // Namespace: HALF-FULL
    //
    //===============================================================================//

    public class Waypoint : MonoBehaviour
    {
        GameObject m_gRemovedWaypoint;
        WaypointSpawner m_wsSpawner = null;
		int m_iRemoveVariable = 0;

		void Awake()
		{
			if (gameObject.GetComponent<CapsuleCollider>()) 
			{
				gameObject.GetComponent<CapsuleCollider>().radius = 5;
			} 
			else 
			{
				Debug.Log("Flag has no collider.");
			}
		}

        public void Intialise(WaypointSpawner _spawner)
        {
            m_wsSpawner = _spawner;
        }

        void OnTriggerEnter(Collider _collider)
        {
            if (_collider.GetComponent<Chaser>())
            {
                foreach (GameObject go in m_wsSpawner.m_glisWayPoints)
                {
                    if (go == this.gameObject)
                    {
                        _collider.gameObject.GetComponent<Kojima.CarScript>().PlayerEXP.AddEXP(10);
                        m_gRemovedWaypoint = go;
                        Destroy(this.gameObject);
                    }
                }
                m_wsSpawner.m_glisWayPoints.Remove(m_gRemovedWaypoint);
            }
        }

		void Update()
		{
			if (FinishManager.s_pFinishManager.m_bSpawned && m_iRemoveVariable == 0) 
			{
				Destroy(gameObject.GetComponent<Bird.Checkpoint> ());
				m_iRemoveVariable++;
			}
		}
    }
}