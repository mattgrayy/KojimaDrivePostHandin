using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Kojima
{
   
    public class CaravanRace : Kojima.GameMode
    {



        bool m_bscoredall = false;
       
        

        /* ============= Game Specific Variables ============= */

        public int m_numberOfRounds = 1;
        private int m_roundsPlayed = 0;

        /* =================================================== */

        new
        void Start()
        {
            // Hide all HUDElements
            Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_HIDE_ALL_ELEMENTS);

            // Unhide the elements we want (this event accepts both hudElementToggleData_t and hudElementToggleMultiData_t - multi accepts an array of types)
            Bird.HUDController.hudElementToggleMultiData_t dataobject = new Bird.HUDController.hudElementToggleMultiData_t();
            dataobject.m_nPlayerID = 0; // Target player ID 0 = all players (otherwise, it's 1 - 4)
            dataobject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.ENABLE;
            dataobject.m_ArrayTypes = new System.Type[] { typeof(Bird.HUD_EXP), typeof(Bird.HUD_Timer), typeof(Bird.HUD_ScorePopupMgr),  typeof(Bird.HUD_NavArrow) }; //, typeof(Bird.HUD_RacePosition)

            // This will enable the exp display, timer, score popup and race position
            Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, dataobject);
           
            base.Start();
            m_mode = GameModeManager.GameModeState.CARAVAN;
            m_active = true;
            //adds grapples and caravans
            AddonManager.m_instance.addToAllCars(AddonManager.AddonType_e.GRAPPLE);
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
                case "Play":
                    {
                        
                        //Informs the game of the change of phase
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.GM_CARAVAN);

                       
                        //adds grapples and caravans
                        AddonManager.m_instance.addToAllCars(AddonManager.AddonType_e.GRAPPLE);

                        //Starts  timer for play phase
                        // m_currentPhase.m_timer.StartTimer();
                        GetPhase("Play").m_timer.StartTimer();
                        Debug.Log("TIMER START");


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
                
                case "Play":
                    {
                      
                        //Checks whether the play phase timer has finished and ends the game
                        if (m_currentPhase.m_timer.CheckFinished())
                            
                        {

                            //Increases the number of rounds played
                            m_roundsPlayed++;

                            //Changes phase to reset phase to hand end of round logic
                            TransistionToNextPhase();
                            InitializePhase();



                            base.EndGame();
                            //  EventManager.m_instance.UnsubscribeToEvent(Events.Event.GM_GRAPPLERACE, RaceSetup);

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
        /// Moves Players to spawn position
        /// </summary>
        void SpawnPlayers()
        {
            for (int iter = 0; iter <= m_numberOfPlayers - 1; iter++)
            {
                SpawnNextSpawn(Kojima.GameController.s_singleton.m_players[iter].gameObject);
            }
         
        }



        //this checks for a collision with the end point to give points to the teams.
        void OnTriggerEnter(Collider col)
        {
            if (m_bscoredall == false) //(gameObject != null)
            {
                for (int i = 1; i < Kojima.GameController.s_singleton.m_players.Length; i++)
                {
                    HF.PlayerExp.AddEXP(i, 1000 / i, true, true);
                }

                m_bscoredall = true;
            }
                if (m_bscoredall == true)
                {
                    //Destroy(gameObject);

                    base.EndGame();
                }
            }
        }
    
  

    /* =================================================== */
}
