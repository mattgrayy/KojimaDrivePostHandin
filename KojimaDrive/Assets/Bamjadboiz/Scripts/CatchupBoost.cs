using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Author:       Thomas Moreton
//Description:  Script added to Catchup manager prefab 
//              Detects when to boost specific cars
//Last edit:    Thomas Moreton @ 14/04/2017
namespace Kojima
{
    public class CatchupBoost : MonoBehaviour
    {
        public static CatchupBoost s_singleton;
        public bool m_raceCatchupEnabled;
        public bool m_groupCatchupEnabled;
        [Range (0, 100)]
        public int m_groupMaxRadius;
        public List<Kojima.CarScript> m_cars;
        public int m_totalDistance;
        public Kojima.RaceScript m_racescript;
        [Range(0, 20)]
        public float m_boostMaxSpeed;
        [Range(0, 5)]
        public float m_boostAcceleration;

        bool m_groupBoost;
        //Calculates the distance between all the cars
        bool CalculateGroupDistance()
        {
            if (m_cars.Count != 0)
            {
                m_totalDistance = 0;
                for (int i = 0; i + 1 <= m_cars.Count -1; i++)
                {
                    if (m_cars[i] && m_cars[i + 1])
                    {
                        m_totalDistance += (int)Vector3.Distance(m_cars[i].gameObject.transform.position, m_cars[i + 1].gameObject.transform.position);
                    }
                }
                if (m_totalDistance <= m_groupMaxRadius)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }                      
        //Calculates distance in a CoRoutine to avoid overhang in update function
        IEnumerator CheckDistance()
        {
            for (;;)
            {
                m_groupBoost = CalculateGroupDistance();
                yield return new WaitForSeconds(.1f);
            }            
        }

        void Awake()
        {
            if(!s_singleton)
            {
                s_singleton = this;
            }
        }

        void Start()
        {
            m_cars = new List<Kojima.CarScript>();
            if (m_groupCatchupEnabled == true)
            {
                StartCoroutine("CheckDistance");
            }
            if (m_raceCatchupEnabled == true)
            {
                //Will do stuff here soon
            }
        }
        //Will apply boost if total distance is below defined threshold
        void Update()
        {
            if (m_groupCatchupEnabled == true)
            {
                if (m_cars.Count > 1)
                {
                    if (m_groupBoost == true)
                    {                    
                        foreach (Kojima.CarScript car in m_cars)
                        {
                            if (car.CurrentlyBoosting == false)
                            {
                                car.m_boostStats.m_maxSpeed = m_boostMaxSpeed;
                                car.m_boostStats.m_acceleration = m_boostAcceleration;
                            }                            
                        }
                    }
                    else
                    {
                        foreach (Kojima.CarScript car in m_cars)
                        {
                            if (car.CurrentlyBoosting == true)
                            {
                                car.m_boostStats.m_maxSpeed = 0;
                                car.m_boostStats.m_acceleration = 0;
                            }
                        }
                    }
                }
                else
                {
                    //Debug.Log("Error: Group boost requires 2 or more cars assigned to 'm_cars'");
                }
            }   
            if (m_raceCatchupEnabled == true)
            {
                if (m_racescript)
                {
                    for (int i = 0; i < Kojima.GameController.s_ncurrentPlayers; i++)
                    {
                        if (i == m_racescript.GetLastPerson())
                        {
                            Kojima.GameController.s_singleton.m_players[i].m_boostStats.m_maxSpeed = m_boostMaxSpeed;
                            Kojima.GameController.s_singleton.m_players[i].m_boostStats.m_acceleration = m_boostAcceleration;
                        }
                        else
                        {
                            Kojima.GameController.s_singleton.m_players[i].m_boostStats.m_maxSpeed = 0;
                            Kojima.GameController.s_singleton.m_players[i].m_boostStats.m_acceleration = 0;
                        }
                    }                                                       
                    
                }
                else
                {
                    Debug.Log("Error: Unable to use Race catchup. Please reference the racescript in the catchupmanager's inspector view");
                }
            }                      
        }
    }
}

