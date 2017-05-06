using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Kojima
{
    public class CaravanScoreRace : GameMode
    {
        [SerializeField] int m_iCaravanCount;
        [SerializeField] float m_fCaravanSpawnTimer;
        [SerializeField] Transform caravanSpawnPoint, caravanScoreAreaSpawnPoint, caravanPrefab, caravanScoreAreaPrefab;

        List<Transform> m_Cars = new List<Transform>();

        AddonManager m_AddonManager;
        List<GameObject> m_GeneratedAssets = new List<GameObject>();

        bool running = false;
        float spawnTimer;

        new
         void Start()
        {
            m_AddonManager = AddonManager.m_instance;
            base.Start();
            m_mode = GameModeManager.GameModeState.CARAVANSCORE;
            spawnTimer = m_fCaravanSpawnTimer;

            // Hide all HUDElements
            Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_HIDE_ALL_ELEMENTS);

            // Unhide the elements we want (this event accepts both hudElementToggleData_t and hudElementToggleMultiData_t - multi accepts an array of types)
            Bird.HUDController.hudElementToggleMultiData_t dataobject = new Bird.HUDController.hudElementToggleMultiData_t();
            dataobject.m_nPlayerID = 0; // Target player ID 0 = all players (otherwise, it's 1 - 4)
            dataobject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.ENABLE;
            dataobject.m_ArrayTypes = new System.Type[] { typeof(Bird.HUD_EXP), typeof(Bird.HUD_Timer), typeof(Bird.HUD_ScorePopupMgr), typeof(Bird.HUD_NavArrow) };

			Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, dataobject);
        }

        new
       void Update()
        {
            base.Update();

            //Game Mode Loop
            if (m_active)
            {
                if (!running)
                {
                    TransistionToNextPhase();
                    GetPhase("Timer").m_timer.StartTimer();
                    running = true;
                    setup();
                }

                if (GetPhase("Timer").m_timer.CheckFinished())
                {
                    List<int> carPositions = new List<int>();

                    for (int i = 0; i < m_Cars.Count; i++)
                    {
                        carPositions.Add(m_playerScores[i]);
                    }

                    TransistionToNextPhase();

                    EndGame();
                }

                if (spawnTimer > 0)
                {
                    spawnTimer -= Time.deltaTime;

                    if (spawnTimer <= 0)
                    {
                        spawnTimer = m_fCaravanSpawnTimer;

                        Transform newCaravan = Instantiate(caravanPrefab, caravanSpawnPoint.position, Quaternion.identity) as Transform;
                        Destroy(newCaravan.GetComponent<ConfigurableJoint>());
                        m_GeneratedAssets.Add(newCaravan.gameObject);
                    }
                }
            }
        }

        public void addScore(int _value, Transform _carToAddTo)
        {
            for (int i = 0; i < m_Cars.Count; i++)
            {
                if (m_Cars[i] == _carToAddTo)
                {
                    HF.PlayerExp.AddEXP(i, 250, true, true, "Scored a Caravan!", true);
                    AddScore(i, _value);
                    Bird.CheckpointManager.CM.RemoveCheckpoint(caravanScoreAreaSpawnPoint.gameObject, i+1);
                    Bird.CheckpointManager.CM.AddCheckpoint(caravanSpawnPoint.gameObject, i+1);
                    break;
                }
            }
        }

        void setup()
        {
            if (m_AddonManager == null)
            {
                m_AddonManager = AddonManager.m_instance;
            }

            m_Cars = m_AddonManager.getAllCars();
            m_AddonManager.addToAllCars(AddonManager.AddonType_e.GRAPPLE);

            for (int carsIndex = 0; carsIndex < m_Cars.Count; carsIndex++)
            {
                m_Cars[carsIndex].gameObject.AddComponent<CaravanManager>().setCaravanScoreRace(this);
                m_Cars[carsIndex].position = m_spawns[carsIndex].m_transform.position;
                m_Cars[carsIndex].rotation = m_spawns[carsIndex].m_transform.rotation;
                Bird.CheckpointManager.CM.AddCheckpoint(caravanSpawnPoint.gameObject, carsIndex+1);
            }

            Transform scoreArea = Instantiate(caravanScoreAreaPrefab, caravanScoreAreaSpawnPoint.position, caravanScoreAreaSpawnPoint.rotation) as Transform;
            scoreArea.GetComponent<CaravanScoreArea>().setRace(this);
            m_GeneratedAssets.Add(scoreArea.gameObject);

            for (int i = 0; i < m_iCaravanCount; i++)
            {
                Vector3 spawnPoint = caravanSpawnPoint.position + (new Vector3(0, i*3,0));
                Transform newCaravan = Instantiate(caravanPrefab, spawnPoint, Quaternion.identity) as Transform;
                Destroy(newCaravan.GetComponent<ConfigurableJoint>());
                m_GeneratedAssets.Add(newCaravan.gameObject);
            }
        }

        public void carHasCaravan(Transform _car)
        {
            for (int i = 0; i < m_Cars.Count; i++)
            {
                if (m_Cars[i] == _car)
                {
                    Bird.CheckpointManager.CM.RemoveCheckpoint(caravanSpawnPoint.gameObject, i+1);
                    Bird.CheckpointManager.CM.AddCheckpoint(caravanScoreAreaSpawnPoint.gameObject, i+1);
                    break;
                }
            }
        }
        public void carDroppedCaravan(Transform _car)
        {
            for (int i = 0; i < m_Cars.Count; i++)
            {
                if (m_Cars[i] == _car)
                {
                    Bird.CheckpointManager.CM.RemoveCheckpoint(caravanScoreAreaSpawnPoint.gameObject, i+1);
                    Bird.CheckpointManager.CM.AddCheckpoint(caravanSpawnPoint.gameObject, i+1);
                    break;
                }
            }
        }


        /// <summary>
        /// Handles game logic when the game ends
        /// </summary>
        new
        void EndGame()
        {
            foreach (GameObject g in m_GeneratedAssets)
            {
                Destroy(g);
            }
            m_GeneratedAssets.Clear();

            base.EndGame();
        }
    }
}
