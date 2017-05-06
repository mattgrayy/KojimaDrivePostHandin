using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//===================== Kojima Drive - Half-Full 2017 ====================//
//
// Author: SAM BRITNALL
// Purpose: Manager for the game twists
// Namespace: HALF-FULL
//
//===============================================================================//

namespace HF
{


    /// <summary>
    /// Directions of use:
    /// 
    /// 1: First, add your new twist to the twist enum, this acts as a store for all
    /// the twists in Kojima. This can be found in the Twists struct, then, add a public bool of your twist 'off'.
    /// This will control whether or not the player wishes the twist to be active.
    /// 2: Create a public setter function to set the bool you just created, and set it as false by default at the start
    /// Then, create a game object variable, and assign the manager of your new twist using the resources.load function
    /// 3: Next, go to the PopulateLists function and add in an else if onto the if statements
    /// to check what game mode is currently in play. This should use the game mode structure provided
    /// by the event system that Half Full has created. You will also want an if to check to see if the twist is enabled
    /// 4: Go to the SpawnScripts function, and add an else if on the end of the if statements for your twist
    /// if the current twist is equal to your twist, then you should spawn in a manager and add it to the m_twistmanagers list
    /// the scripts should then handle themselves, this function is called when a timer expires
    /// 5: That's it, the twist has been added. However, you need to call the public function StartTwists to start off
    /// the process somewhere in your game mode logic, the manager will then take care of itself.
    /// 6: If you want the twists to start after a certain time. Use the SetTimer function, to set a customised time for the twists
    /// before you call StartTwists (DEFAULT IS SET TO TEN SECONDS)
    /// 7: If you wish to stop the twists at any point in your logic, call StopTwists and the twist manager will return to it's 
    /// otherwise passive state
    /// 8: If you want more than one twists to spawn, then use the Setnumberoftwistsatonce function
    /// </summary>

    public class TwistsManager : MonoBehaviour
    {

        int m_previousTwist = -1;
        int m_numberOfTwistsAtOnce = 1;

        float m_timer = 10.0f;
        bool m_timerStart;

        public GameObject m_tsunamiPrefab;
        //public GameObject m_erruptionPrefab;
        public GameObject m_testTwistPrefab;
        public GameObject m_tornadoTwistPrefab;

        Twists m_twistContainer = new Twists();

        public List<Twists.Twist> m_eventTwists = new List<Twists.Twist>();

        public List<Twists.Twist> m_currentTwists = new List<Twists.Twist>();

        public List<GameObject> m_TwistManagers = new List<GameObject>();

        public static TwistsManager m_instance;

        // Use this for initialization
        void Start()
        {
            if (m_instance)
            {
                Destroy(this);
            }
            else
            {
                m_instance = this;
            }

            m_timerStart = false;

            m_twistContainer.allOff = false;
            m_twistContainer.tsunamiOff = false;

            //event subscriptions, call to start the twists
            Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.TW_START, StartTwists);
            Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.TW_STOP, StopTwists);


            //prefab list
            m_tsunamiPrefab = (GameObject)Resources.Load("TwistPrefabs/TsunamiManager");
            m_testTwistPrefab = (GameObject)Resources.Load("TwistPrefabs/TestTwist");
            m_tornadoTwistPrefab = (GameObject)Resources.Load("TwistPrefabs/TornadoTwist");
        }

        // Update is called once per frame to check to see if the timer is ready to be started
        void Update()
        {
            if (m_timerStart == true)
            {
                SetUpCountdown();
            }

        }

        void OnDestroy()
        {
            Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.TW_START, StartTwists);
            Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.TW_STOP, StopTwists);
        }

        //trigger to start the twist process
        public void StartTwists()
        {
            if (m_twistContainer.allOff == false)
            {
              
                PopulateTwistList();
                m_timerStart = true;
                Debug.Log("TwistsStarted");
            }
        }

        //trigger to stop the twist process
        public void StopTwists()
        {
            m_timerStart = false;
            StopAllCoroutines();
            RemoveScripts();
        }

        //add each of the twists to the list dependent on game mode
        void PopulateTwistList()
        {
            m_eventTwists.Clear();
            for (int iter = 0; iter < m_currentTwists.Count; iter++)
            {

                if (Kojima.GameModeManager.m_instance.m_currentMode != Kojima.GameModeManager.GameModeState.FREEROAM)
                {
                    bool sameTwistCheck = false;
                    for(int iter2 = 0; iter2 < m_eventTwists.Count; iter2++)
                    {
                        if(m_eventTwists[iter2] == m_currentTwists[iter])
                        {
                            sameTwistCheck = true;
                        }
                    }

                    if (sameTwistCheck == false)
                    {
                        //TwistBase newTwist = new TwistBase();
                        //newTwist.thisTwist = m_currentTwists[iter];
                        m_eventTwists.Add(m_currentTwists[iter]);
                    }

                }
                else if (Kojima.GameModeManager.m_instance.m_currentMode == Kojima.GameModeManager.GameModeState.FREEROAM)
                {
                    StopTwists();
                    Debug.Log("Twist tried to start in freeroam. Twist manager was disabled");
                }

            }
        }

        //selects a twist from the list
        void ChooseTwist()
        {
            int twist;

            for (int iter = 0; iter < m_currentTwists.Count; iter++)
            {
                m_currentTwists.RemoveAt(iter);
            }

            m_currentTwists.Clear();

            for (int iter = 0; iter < m_numberOfTwistsAtOnce; iter++)
            {
                twist = m_previousTwist;
                while (twist == m_previousTwist)
                {
                    twist = Random.Range(0, m_eventTwists.Count);
                }

                m_currentTwists.Add(m_eventTwists[twist]);
                m_previousTwist = twist;
            }

            Debug.Log("swap");
            RemoveScripts();
            //SpawnTwistScripts();
            m_timerStart = true;
        }

        //removes all the current twist scripts
        void RemoveScripts()
        {
            for (int iter = 0; m_TwistManagers.Count > iter; iter++)
            {
                Destroy(m_TwistManagers[iter]);
            }

            for (int iter2 = 0; Kojima.GameController.s_singleton.m_players.Length > iter2; iter2++)
            {
                Destroy(Kojima.GameController.s_singleton.m_players[iter2].gameObject.GetComponent<GCSharp.Orbiting>());

            }
            m_TwistManagers.Clear();
        }

        //spawns the seperate twist scripts for implementation
        void SpawnTwistPrefabs()
        {
            for (int iter = 0; m_currentTwists.Count > iter; iter++)
            {
                if (m_currentTwists[iter] == Twists.Twist.tsunami)
                {
                    m_TwistManagers.Add(Instantiate(m_tsunamiPrefab));
                }
                else if (m_currentTwists[iter] == Twists.Twist.test)
                {
                    m_TwistManagers.Add(Instantiate(m_testTwistPrefab));
                }
                else if (m_currentTwists[iter] == Twists.Twist.tornado)
                {
                    m_TwistManagers.Add(Instantiate(m_tornadoTwistPrefab));
                    m_TwistManagers[iter].GetComponent<TornadoTwist>().SpawnTwistPrefab();
                }
                else if (m_currentTwists[iter] == Twists.Twist.NULL)
                {
                    //do nothing
                }
            }
        }

        //sets the countdown to be started
        void SetUpCountdown()
        {
            m_timerStart = false;
            StartCoroutine(CountdownToTwist());
        }

        //countdown before twist kicks in
        IEnumerator CountdownToTwist()
        {
            yield return new WaitForSeconds(m_timer); //time that the thing will start
            ChooseTwist();
        }

        //helper function allowing you to set the time in seconds when the twist kicks in
        public void SetTimer(float toSet)
        {
            m_timer = toSet;
        }

        //helper function allowing you to set the time in seconds when the twist kicks in
        public void SetTimer(int toSet)
        {
            m_timer = toSet;
        }

        public void SetAllOff(bool toSet)
        {
            m_twistContainer.allOff = toSet;
        }
        public void SetTsunamiOff(bool toSet)
        {
            m_twistContainer.tsunamiOff = toSet;
        }
        /*public void SetEruptionOff(bool toSet)
        {
            m_twistContainer.eruptionOff = toSet;
        }*/

        public void SetNumberOfTwistsAtOnce(int toSet)
        {
            m_numberOfTwistsAtOnce = toSet;
        }
    }
}