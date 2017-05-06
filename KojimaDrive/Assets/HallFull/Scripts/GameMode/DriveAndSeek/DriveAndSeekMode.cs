using UnityEngine;
// using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace HF
{
    public class DriveAndSeekMode : Kojima.GameMode
    {
        [HideInInspector]
        public int m_hiderNumber = -1;
        private int m_prevHiderNum = -1;

        [HideInInspector]
        public bool m_hiderWon = false;

        [HideInInspector]
        public GameObject m_infoText;
        public GameObject m_gCheckpointManager;

        private GameObject m_finishManager;

        private int m_winner = -1;

        public string m_runnerName = "";
        public string m_chaserName = "";

        private AudioSource m_audio;

        new
        void Start()
        {
            base.Start();

            m_infoText = GameObject.FindGameObjectWithTag("DaSText");

            m_mode = Kojima.GameModeManager.GameModeState.DRIVEANDSEEK;

            m_finishManager = GameObject.Find("FinishManager");

            m_audio = GetComponent<AudioSource>();
        }

        new
        void Update()
        {
            base.Update();
            UpdatePhase();
        }

        public void InitializePhase()
        {
            switch (m_currentPhase.m_phaseName)
            {
                case "Inactive":
                    {
                        m_infoText.GetComponent<Text>().text = "";
                        ChangeAllPlayerMovement(true);
                        break;
                    }
                case "CarSetup":
                    {
                        m_infoText.GetComponent<Text>().text = "";

                        IdentifyRunnerAndChasers();
                        SpawnPlayers();

                        Kojima.CarSwapManager.m_sInstance.SetSwapping(true);
                        ChangeAllPlayerMovement(false);

                        if (!m_lockCarChanges)
                        {
                            SetAllCarsLocks(false);
                        }
                        else
                        {
                            SetAllCarsLocks(true);
                        }

                        m_currentPhase.BeginPhase();

                        break;
                    }
                case "Setup":
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.DS_SETUP);
                        m_currentPhase.BeginPhase();

                        if (m_winner != -1 && m_hiderWon)
                        {
                            Kojima.GameController.s_singleton.m_players[m_winner].PlayerEXP.AddEXP(100);
                        }

                        Kojima.CarSwapManager.m_sInstance.SetSwapping(false);

                        SetupHiderAndSeekers();

                        m_infoText.GetComponent<Text>().text = "Runner Get Ready!";

                        m_audio.Play();
                        break;
                    }
                case "Running":
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.DS_RUNNING);

                        m_infoText.GetComponent<Text>().text = "Start Running!";
                        m_currentPhase.BeginPhase();

                        ChangeAllPlayerMovement(false);
                        ChangePlayerMovement(m_hiderNumber, true);
                        break;
                    }
                case "Chasing":
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.DS_CHASING);

                        m_infoText.GetComponent<Text>().text = "Catch the Runner!";

                        ChangeAllPlayerMovement(true);

                        m_currentPhase.BeginPhase();
                        break;
                    }
                case "Pause":
                    {
                        break;
                    }
                case "Reset":
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.DS_RESET);

                        if (m_hiderWon)
                        {
                            AddScore(m_hiderNumber, 1);
                            m_winner = m_hiderNumber;
                        }
                        else
                        {
                            int highestScore = -1;
                            m_winner = -1;

                            for (int iter = 0; iter <= m_numberOfPlayers - 1; iter++)
                            {
                                if (iter != m_hiderNumber)
                                {
                                    if (Kojima.GameController.s_singleton.m_players[iter].gameObject.GetComponent<Chaser>().ReturnDamageDealt() > highestScore)
                                    {
                                        Debug.Log("Triggered");
                                        highestScore = Kojima.GameController.s_singleton.m_players[iter].gameObject.GetComponent<Chaser>().ReturnDamageDealt();
                                        m_winner = iter;
                                    }
                                }
                            }

                            AddScore(m_winner, 1);
                        }

                        if (CheckWinnerAll())
                        {
                            m_currentPhase = GetPhase("Finish");
                            InitializePhase();
                        }
                        else
                        {
                            ResetRound();
                            m_currentPhase = GetPhase("CarSetup");
                            InitializePhase();
                        }

                        m_audio.Stop();
                        break;
                    }
                case "Finish":
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.DS_FINISH);

                        ChangeAllPlayerMovement(false);

                        m_currentPhase.m_message = "Player " + (m_gameWinner + 1) + " Wins!";
                        m_infoText.GetComponent<Text>().text = m_currentPhase.m_message;

                        Kojima.GameController.s_singleton.m_players[m_gameWinner].PlayerEXP.AddEXP(1000);

                        ResetScores();

                        m_audio.Stop();

                        m_currentPhase.m_timer.StartTimer();

                        break;
                    }
                case "Dynamic":
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.DS_DYNAMIC);

                        ChangeAllPlayerMovement(false);

                        m_currentPhase.m_timer.m_fTimerLength = m_currentPhase.m_length;
                        m_currentPhase.m_timer.m_fDelayLength = m_currentPhase.m_delay;
                        m_currentPhase.m_delay = 0.0f;
                        m_infoText.GetComponent<Text>().text = m_currentPhase.m_message;
                        m_currentPhase.m_timer.StartTimer();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        void UpdatePhase()
        {
            switch (m_currentPhase.m_phaseName)
            {
                case "Inactive":
                    {
                        if (m_active)
                        {
                            Bird.HUDController.hudElementToggleData_t PositionObject = new Bird.HUDController.hudElementToggleData_t();
                            PositionObject.m_nPlayerID = 0;
                            PositionObject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.DISABLE;
                            PositionObject.m_Type = typeof(Bird.HUD_RacePosition);

                            Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, PositionObject);

                            Instantiate(m_gCheckpointManager);
                            TransistionToNextPhase();
                            InitializePhase();
                        }

                        break;
                    }
                case "CarSetup":
                    {
                        ChangeAllPlayerMovement(false);

                        if (CheckAllCarLocked())
                        {
                            m_currentPhase.m_timer.StopTimer();
                            TransistionToNextPhase();
                            InitializePhase();
                        }

                        if (m_currentPhase.m_timer.CheckFinished())
                        {
                            SetAllCarsLocks(true);
                        }
                        break;
                    }
                case "Setup":
                    {
                        if (m_currentPhase.m_timer.CheckFinished())
                        {
                            TransistionToNextPhase();
                            InitializePhase();
                        }
                        break;
                    }
                case "Running":
                    {
                        if (m_currentPhase.m_timer.CheckFinished())
                        {
                            TransistionToNextPhase();
                            InitializePhase();
                        }
                        break;
                    }
                case "Chasing":
                    {
                        if (CheckHidersCaught())
                        {
                            TransistionToNextPhase();
                            InitializePhase();
                        }

                        if (CheckSeekersDead())
                        {
                            m_hiderWon = true;
                            m_winner = m_hiderNumber;
                            TransistionToNextPhase();
                            InitializePhase();
                        }

                        if (m_currentPhase.m_timer.CheckFinished())
                        {
                            m_hiderWon = true;
                            m_winner = m_hiderNumber;
                            TransistionToNextPhase();
                            InitializePhase();
                        }

                        if ((Time.time - m_currentPhase.m_timer.m_fStartTime) >= (m_currentPhase.m_timer.m_fTimerLength / 2))
                        {
                            m_finishManager.GetComponent<FinishManager>().m_bPick = true;
                            m_finishManager.GetComponent<FinishManager>().PickFinish();
                        }

                        break;
                    }
                case "Pause":
                    {
                        break;
                    }
                case "Reset":
                    {
                        break;
                    }
                case "Finish":
                    {
                        if (m_currentPhase.m_timer.CheckFinished())
                        {
                            EndGame();
                            TransistionToNextPhase();
                            InitializePhase();
                        }
                        break;
                    }
                case "Dynamic":
                    {
                        if (!m_currentPhase.m_timer.m_bDelaying)
                        {
                            if (m_currentPhase.m_timer.CheckFinished())
                            {
                                TransistionToNextPhase();
                                InitializePhase();
                            }
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        void IdentifyRunnerAndChasers()
        {
            if (!m_hiderWon && m_hiderNumber == -1)
            {
                m_prevHiderNum = m_hiderNumber;
                do
                {
                    m_hiderNumber = UnityEngine.Random.Range(1, m_numberOfPlayers + 1) - 1;
                } while (m_hiderNumber == m_prevHiderNum);
            }
            else if (!m_hiderWon)
            {
                m_prevHiderNum = m_hiderNumber;

                m_hiderNumber = m_winner;

                RemoveMemberFromTeam(m_prevHiderNum, "Runners");
                RemoveMemberFromTeam(m_hiderNumber, "Seekers");
            }
            else if(m_hiderWon)
            {

            }
        }

        void SetupHiderAndSeekers()
        {        
            for (int iter = 0; iter <= m_numberOfPlayers - 1; iter++)
            {
                Kojima.GameController.s_singleton.m_players[iter].gameObject.AddComponent<DriveAndSeek>();
            }

            string HiderTag = "Player" + m_hiderNumber;

            Kojima.GameController.s_singleton.m_players[m_hiderNumber].gameObject.GetComponent<DriveAndSeek>().SetRunner();
            Kojima.GameController.s_singleton.m_players[m_hiderNumber].gameObject.GetComponent<Kojima.CarScript>().ResetCar();
            AddMemberToTeam(m_hiderNumber, "Runners");

            for (int iter = 0; iter <= m_numberOfPlayers - 1; iter++)
            {
                if (iter != m_hiderNumber)
                {
                    Kojima.GameController.s_singleton.m_players[iter].gameObject.GetComponent<DriveAndSeek>().SetChaser(HiderTag, m_hiderNumber);
                    Kojima.GameController.s_singleton.m_players[iter].gameObject.GetComponent<Kojima.CarScript>().ResetCar();
                    AddMemberToTeam(iter, "Chasers");
                }
            }

            ResetSpawns();

            ////////////////TMS - BAM////////////////////////
            //This causes the big white spinny transition and everything while telling the hider's camera to focus on the new car/van
            // Kojima.CameraManagerScript.singleton.SetupThirdPersonForAllPlayers();
            /////////////////////////////////////////////////

            m_hiderWon = false;
        }

        void SpawnPlayers()
        {
            for (int iter = 0; iter <= m_numberOfPlayers - 1; iter++)
            {
                if (iter != m_hiderNumber)
                {
                    Kojima.CarSwapManager.m_sInstance.ChangeCar(iter, Kojima.GameController.s_singleton.m_players[iter].m_nControllerID, m_chaserName);
                }
                else
                {
                    Kojima.CarSwapManager.m_sInstance.ChangeCar(iter, Kojima.GameController.s_singleton.m_players[iter].m_nControllerID, m_runnerName);
                }
            }

            SpawnNextSpawn(Kojima.GameController.s_singleton.m_players[m_hiderNumber].gameObject);

            for (int iter = 0; iter <= m_numberOfPlayers - 1; iter++)
            {
                if (iter != m_hiderNumber)
                {
                    SpawnNextSpawn(Kojima.GameController.s_singleton.m_players[iter].gameObject);
                }
            }
        }

        bool CheckHidersCaught()
        {
            if (Kojima.GameController.s_singleton.m_players[m_hiderNumber].GetComponent<Health>().m_fCurrentHealth <= 0.0f)
            {
                return true;
            }
            return false;
        }

        bool CheckSeekersDead()
        {
            for (int iter = 0; iter <= m_numberOfPlayers - 1; iter++)
            {
                if (iter != m_hiderNumber)
                {
                    if (Kojima.GameController.s_singleton.m_players[iter].GetComponent<Health>())
                    {
                        if (Kojima.GameController.s_singleton.m_players[iter].GetComponent<Health>().m_fCurrentHealth > 0.0f)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        void ResetRound()
        {
            ResetAllPhases();

            for (int iter = 0; iter <= m_numberOfPlayers - 1; iter++)
            {
                if (iter == m_hiderNumber)
                {
                    Kojima.GameController.s_singleton.m_players[iter].gameObject.GetComponent<DriveAndSeek>().ResetRunner();
                }
                else
                {
                    Kojima.GameController.s_singleton.m_players[iter].gameObject.GetComponent<DriveAndSeek>().ResetChaser();
                }
            }
        }

        /// <summary>
        /// Handles game logic when the game ends
        /// </summary>
        new
        public void EndGame()
        {
            ResetRound();

            for (int iter = 0; iter <= m_numberOfPlayers - 1; iter++)
            {
                Destroy(Kojima.GameController.s_singleton.m_players[iter].gameObject.GetComponent<DriveAndSeek>());
            }

            for (int iter = 0; iter <= m_numberOfPlayers - 1; iter++)
            {
                if (iter != m_hiderNumber)
                {
                    HF.AddOnManagerHF.m_instance.removeAddOns(Kojima.GameController.s_singleton.m_players[iter].gameObject.transform);
                }
            }

            ResetMode();

            SetAllCarsLocks(false);

            base.EndGame();
        }

        /// <summary>
        /// Resets game mode upon completion of the game
        /// </summary>
        void ResetMode()
        {
            ResetAllPhases();

            m_hiderNumber = -1;
            m_gameWinner = -1;
            m_hiderWon = false;

            FinishManager.s_pFinishManager.ResetFinish();
        }
    }
}
