using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HF
{
    //===================== Kojima Drive - Half-Full 2017 ====================//
    //
    // Author: SAM BRITNALL, ADAM MOOREY
    // Purpose: Spawns an object behind the car, acts as a waypoint
    // Namespace: HALF-FULL
    //
    //===============================================================================//

    public class WaypointSpawner : MonoBehaviour
    {
        public List<GameObject> m_glisWayPoints = new List<GameObject>();

        GameObject m_gWayPointReference;
        GameObject m_gWayPointHolder;
        GameObject m_gWaypointManager;
        public GameObject m_gWayPoint;

        Coroutine m_timerCoroutine = null;

        public bool m_bTimer;
        float m_fWaypointWaitTime = 5.0f;
        float m_fMinDistance = 30.0f;
        int m_index;

        void Start()
        {
            Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.DS_RUNNING, StartWayPointSpawns);

            m_gWayPointReference = (GameObject)Resources.Load("Flag_Prefab");
            m_gWayPointHolder = Instantiate((GameObject)Resources.Load("WaypointHolder"));
            m_gWaypointManager = GameObject.Find("WaypointManager");
            m_bTimer = true;
            m_index = 0;
        }

        //check to see if the event has started, check to see if the car needs to start a timer to drop waypoints...
        void Update()
        {
			if (m_timerCoroutine == null && !m_bTimer)
            {
                m_timerCoroutine = StartCoroutine(WayPointTimer());
            }

            //... check to see if I should be alive
            if (Kojima.GameModeManager.m_instance.m_currentMode == Kojima.GameModeManager.GameModeState.FREEROAM)
            {
                Destroy(m_gWayPointHolder);
                //Destroy(m_oilReference);
                Destroy(gameObject.GetComponent<HiderAbilities>());
            }
        }

        private void OnDestroy()
        {
            Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.DS_RUNNING, StartWayPointSpawns);

            for (int iter = 0; iter <= m_glisWayPoints.Count - 1; iter++)
            {
                Destroy(m_glisWayPoints[iter]);
            }
            m_glisWayPoints.Clear();
            Destroy(m_gWayPointHolder);
        }

        //this is the event trigger which flips a bool to allow the car to start spawning waypoints
        void StartWayPointSpawns()
        {
            m_bTimer = false;
        }

        void SpawnWayPointCheck()
        {
            if (m_glisWayPoints.Count <= 0)
            {
                SpawnWayPoint();
            }
            else if (m_glisWayPoints.Count > 0)
            {
                Vector3 previousPosition = m_glisWayPoints[m_glisWayPoints.Count - 1].transform.position;
                float currentDistance = Vector3.Distance(previousPosition, gameObject.transform.position);

                if (currentDistance >= m_fMinDistance)
                {
                    SpawnWayPoint();
                }
            }
        }

        //this spawns the waypoint by the back spawn
        void SpawnWayPoint()
        {
            m_gWayPoint = Instantiate(m_gWayPointReference);

            m_gWayPoint.AddComponent<Waypoint>();
            m_gWayPoint.GetComponent<Waypoint>().Intialise(this);
            m_gWayPoint.GetComponent<CapsuleCollider>().isTrigger = true;

            m_gWayPoint.transform.position = gameObject.transform.position;
            m_gWayPoint.transform.parent = m_gWayPointHolder.transform;
            m_glisWayPoints.Add(m_gWayPoint);

            m_index++;
        }

        public void ResetWaypoints()
        {
            for (int iter = 0; iter <= m_glisWayPoints.Count - 1; iter++)
            {
                Destroy(m_glisWayPoints[iter]);
            }
            m_glisWayPoints.Clear();
            Destroy(m_gWayPointHolder);
        }

        //this is a timer to seperate out the waypoint drops
        IEnumerator WayPointTimer()
        {
            yield return new WaitForSeconds(m_fWaypointWaitTime);
            SpawnWayPointCheck();
            m_timerCoroutine = null;
        }
    }
}