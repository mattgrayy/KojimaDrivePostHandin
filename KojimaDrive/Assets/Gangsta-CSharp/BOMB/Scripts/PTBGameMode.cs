using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kojima;
using UnityEngine.SceneManagement;
using Bam;

namespace GCSharp
{
    public class PTBGameMode : GameMode
    {
        //private PTBSetUp ptbSetUp;
        private bool loadScene = false;

        public bool m_testing = false;

        public string m_levelName;
        public int m_rounds;
        private int m_currentRound;

        public CarSwapManager.CarType m_car;
        private List<CarSwapManager.CarType> m_originalCars;
        private List<Vector3> m_originalPositions;
        private GameObject[] m_players;

        private new void Start()
        {
            base.Start();
            SpawnPlayers();
            m_originalCars = new List<CarSwapManager.CarType>();
            m_originalPositions = new List<Vector3>();
            m_mode = GameModeManager.GameModeState.PASSTHEBOMB;
            Bird.HUDController.hudElementToggleMultiData_t dataobject = new Bird.HUDController.hudElementToggleMultiData_t();
            dataobject.m_nPlayerID = 0; // Target player ID 0 = all players (otherwise, it's 1 - 4)
            dataobject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.ENABLE;
            dataobject.m_ArrayTypes = new System.Type[] { typeof(Bird.HUD_EXP), typeof(Bird.HUD_ScorePopupMgr), typeof(Bird.HUD_NavArrow) };
            Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, dataobject);
            m_players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < m_players.Length; i++)
            {
                m_originalCars.Add(m_players[i].GetComponent<CarData>().m_type);
                m_originalPositions.Add(m_players[i].transform.position);
                Kojima.CarSwapManager.m_sInstance.ChangeCar(i, Kojima.GameController.s_singleton.m_players[i].m_nControllerID, m_car);
            }
            Kojima.CarSwapManager.m_sInstance.SetSwapping(false);
            gameObject.GetComponent<PTBSetUp>().Init();
            EventManager.m_instance.SubscribeToEvent(Events.Event.GM_PASSTHEBOMB, PassTheBombSetup);
            m_currentRound = 1;
            //PassTheBombSetup(); //uncomment to get the gamemode working
        }

        private void SpawnPlayers()
        {
            GameObject[] t_players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < t_players.Length; i++)
            {
                SpawnNextSpawn(t_players[i]);
                //t_players[i].transform.position = m_spawns[i].m_transform.position;
                //t_players[i].transform.rotation = m_spawns[i].m_transform.rotation;
            }
        }

        private void OnDestroy()
        {
            EventManager.m_instance.UnsubscribeToEvent(Events.Event.GM_PASSTHEBOMB, PassTheBombSetup);
        }

        private void PassTheBombSetup()
        {
            //SceneManager.LoadScene(m_levelName, LoadSceneMode.Additive);
            print("Loading PTB Scene");
        }

        private new void Update()
        {
            base.Update();
            //if (m_active)
            //{
            //    if (!loadScene)
            //    {
            //        PassTheBombSetup();
            //        loadScene = true;
            //    }
            //}
        }

        /// <summary>
        /// Handles game logic when the game ends
        /// </summary>
        new
            public void EndGame()
        {
            if (m_currentRound < m_rounds)
            {
                m_currentRound++;
                SpawnPlayers();
                GetComponent<PTBSetUp>().BombSpawnerSetUp();
            }
            else
            {
                GetComponent<PTBSetUp>().Cleaner();
                GameObject[] t_players = GameObject.FindGameObjectsWithTag("Player");
                for (int i = 0; i < t_players.Length; i++)
                {
                    t_players[i].transform.position = m_originalPositions[i];
                }
                Bird.HUDController.hudElementToggleMultiData_t dataobject = new Bird.HUDController.hudElementToggleMultiData_t();
                dataobject.m_nPlayerID = 0; // Target player ID 0 = all players (otherwise, it's 1 - 4)
                dataobject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.DISABLE;
                SceneManager.UnloadScene(m_levelName); // need to update it to unload any PTB scenes

                for (int i = 0; i < m_players.Length; i++)
                {
                    //m_players[i].transform.position = m_originalPositions[i];
                    Kojima.CarSwapManager.m_sInstance.ChangeCar(i, Kojima.GameController.s_singleton.m_players[i].m_nControllerID, m_originalCars[i]);
                }

                if (!m_testing)
                {
                    Bam.LobbyManagerScript.singleton.ReturnToLobby();
                }
            }
        }
    }
}