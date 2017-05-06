using UnityEngine;
using System.Collections;

namespace Kojima
{
    public class ExampleMode : GameMode
    {
        /* ============= Game Specific Variables ============= */

        public int m_numberOfRounds = 3;
        private int m_roundsPlayed = 0;

        /* =================================================== */

        new
        void Start()
        {
            base.Start();
            m_mode = GameModeManager.GameModeState.EXAMPLE;
        }

        new
       void Update()
        {
            base.Update();

            //Link to phase specific logic
            UpdatePhase();
        }

        /* ================= Phase Functions ================= */

        /// <summary>
        /// Called when changing to a new phase to initialise the phase
        /// </summary>
        void InitializePhase()
        {
            switch (m_currentPhase.m_phaseName)
            {
                case "Inactive":
                    {
                        break;
                    }
                case "Setup":
                    {
                        //Informs the game of the change of phase
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.EX_SETUP);

                        for (int iter = 0; iter <= Kojima.GameController.s_singleton.m_players.Length - 1; iter++)
                        {
                            if (Kojima.GameController.s_singleton.m_players[iter])
                            {
                                m_numberOfPlayers++;
                            }
                        }

                        SpawnPlayers();

                        //Setup buffer phase
                        m_currentPhase = GetPhase("Dynamic");
                        m_currentPhase.m_length = 5.0f;
                        m_currentPhase.m_nextPhase = "Play";
                        m_currentPhase.m_message = "Round " + (m_roundsPlayed + 1) + " Set!";
                        InitializePhase();
                        break;
                    }
                case "Play":
                    {
                        //Informs the game of the change of phase
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.EX_PLAY);

                        //Starts pre-made timer for play phase
                        m_currentPhase.m_timer.StartTimer();
                        break;
                    }
                case "Pause":
                    {
                        break;
                    }
                case "Reset":
                    {
                        //Informs the game of the change of phase
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.EX_RESET);

                        if (CheckFinished())
                        {
                            m_currentPhase = GetPhase("Finish");
                            InitializePhase();
                        }
                        else
                        {
                            ResetRound();
                            m_currentPhase = GetPhase("Setup");
                            InitializePhase();
                        }
                        break;
                    }
                case "Finish":
                    {
                        //Informs the game of the change of phase
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.DS_FINISH);

                        //Call standard function for deactivating game
                        EndGame();

                        //Setup Dynamic phase
                        m_currentPhase = GetPhase("Dynamic");
                        m_currentPhase.m_length = 5.0f;
                        m_currentPhase.m_nextPhase = "Inactive";
                        m_currentPhase.m_message = "Game Over!";
                        InitializePhase();

                        break;
                    }
                case "Dynamic":
                    {
                        //Informs the game of the change of phase
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.DS_DYNAMIC);

                        //Initialise timer for dynamic buffer phase
                        Debug.Log(m_currentPhase.m_message);
                        m_currentPhase.m_timer.m_fTimerLength = m_currentPhase.m_length;
                        m_currentPhase.m_timer.StartTimer();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// Phase loops for each individual phase
        /// </summary>
        void UpdatePhase()
        {
            switch (m_currentPhase.m_phaseName)
            {
                case "Inactive":
                    {
                        //Checks whether the game has been activated for play
                        if (m_active)
                        {
                            //Initialse when the game is active
                            TransistionToNextPhase();
                            InitializePhase();
                        }

                        break;
                    }
                case "Setup":
                    {
                        break;
                    }
                case "Play":
                    {
                        //Checks whether the play phase has end
                        if (m_currentPhase.m_timer.CheckFinished())
                        {
                            //Increases the number of rounds played
                            m_roundsPlayed++;

                            //Changes phse to reset phase to hand end of round logic
                            TransistionToNextPhase();
                            InitializePhase();
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
                        break;
                    }
                case "Dynamic":
                    {
                        //Checks whether the buffer phase has end
                        if (m_currentPhase.m_timer.CheckFinished())
                        {
                            //Changes to next phase upon buffer phase completion
                            TransistionToNextPhase();
                            InitializePhase();
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /* =================================================== */


        /* =============== Game Mode Functions =============== */

        /// <summary>
        /// Determines whether the game has ended
        /// </summary>
        /// <returns></returns>
        bool CheckFinished()
        {
            //Ends game when pre-allotted rounds played 
            if (m_roundsPlayed == m_numberOfRounds)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Moves Players to spawn position
        /// </summary>
        void SpawnPlayers()
        {
            for (int iter = 0; iter <= m_numberOfPlayers - 1; iter++)
            {
                SpawnNextSpawn(Kojima.GameController.s_singleton.m_players[iter].gameObject);
            }
        }

        /// <summary>
        /// Resets variables and timers between rounds
        /// </summary>
        void ResetRound()
        {
            ResetAllPhases();
        }

        /// <summary>
        /// Resets game mode upon completion of the game
        /// </summary>
        void ResetMode()
        {
            ResetAllPhases();
            m_roundsPlayed = 0;
        }
    }

    /* =================================================== */
}
