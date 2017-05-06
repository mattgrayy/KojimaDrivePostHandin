using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
namespace Bam
{
    public class VolcanoMadness:Kojima.GameMode
    {
        public static VolcanoMadness singleton;
        public Canvas countdownCanvas;
        //public Text countdownText;
        [Range(1, 10)]
        public float countdownDuration;
        float roundCountdown;
        bool roundCountdownCompleted;
        bool roundFailed;
        [Range (2, 10)]
        public float bowlingTimeLimit;
        float bowlingTime;
        bool bowlingCompleted;
        int currentRound = 0;
        [Range(10, 50)]
        public float roundDuration;
        [Range(1, 10)]
        public float intervalDuration;     
        public float roundTimer;      
        public float roundInterval;
        public bool gamemodeRunning;
        protected int runnerID = -1;
        public bool isBowlingMode;
        bool goalReached;
        bool displayHints, hintDisplayed;

        int m_currentOverallRound = 0;
        int m_overallRounds = 2;

        public int[] playerScores;

        public List<GameObject> playerObjects;

        public Transform[] spawnPoints;

        public BowlingManager bowlingManager;

        bool setupDone = false;

        public string runnerName;

        public List<Bam.VolcanoMadnessPlayer> players;

        public Camera pinCam;

        [SerializeField]
        GameObject runnerTarget;

        [SerializeField]
        CountdownScript countdownRef;

        [SerializeField]
        VolcanoMadnessCanvas canvasRef;

        public int GetRunnerID
        {
            get { return runnerID; }
        }

        void OnDisable()
        {
            if (countdownRef)
            {
                countdownRef.gameObject.SetActive(false);
            }
        }

        protected void NewRound()
        {
            GetPhase("Setup").m_timer.ResetTimer();
            GetPhase("Running").m_timer.ResetTimer();
            GetPhase("Ending").m_timer.ResetTimer();

            if (currentRound == Kojima.GameController.s_ncurrentPlayers * m_overallRounds)
            {
                EndGame();
                return;
            }

            pinCam.enabled = false;
            GatherPlayers();

            countdownCanvas.enabled = false;

            //canvasRef.ClearHints();
            //displayHints = false;
            //hintDisplayed = false;
           

            if (isBowlingMode == true)
            {
                BowlingHUDScript.singleton.Reset();

                bowlingManager.RemovePins();
                bowlingManager.SetupPins();
            }

            Kojima.CameraManagerScript.singleton.SetupThirdPersonForAllPlayers();

            foreach (VolcanoMadnessPlayer player in players)
            {
                if (player != null)
                {
                    player.myRole = VolcanoMadnessPlayer.Role.Blocker;
                    player.myObject.GetComponent<Kojima.CarScript>().ResetCar();
                }
            }

            runnerID++;
            if(runnerID == Kojima.GameController.s_ncurrentPlayers)
            {
                runnerID = 0;
                m_currentOverallRound++;

                Debug.Log("Bowling now in session " + m_currentOverallRound);
            }

            currentRound++;
            goalReached = false;
            Debug.Log("Current Round: " + currentRound);           

            players[runnerID].myRole = VolcanoMadnessPlayer.Role.Runner;
            players[runnerID].myObject.transform.position = spawnPoints[0].position;
            players[runnerID].myObject.transform.rotation = spawnPoints[0].rotation;

            if (isBowlingMode)
            {
                Kojima.CarSwapManager.m_sInstance.ChangeCar(runnerID, Kojima.GameController.s_singleton.m_players[runnerID].m_nControllerID, Kojima.CarSwapManager.CarType.CARLO);
            }
            else
            {
                Kojima.CarSwapManager.m_sInstance.ChangeCar(runnerID, Kojima.GameController.s_singleton.m_players[runnerID].m_nControllerID, Kojima.CarSwapManager.CarType.ZUK);
                players[runnerID].myObject = Kojima.GameController.s_singleton.m_players[runnerID].gameObject;
                if (!players[runnerID].myObject.GetComponent<Bam.CalculateAirtime>())
                {
                    players[runnerID].myObject.gameObject.AddComponent<Bam.CalculateAirtime>();
                }
            }
            runnerName = players[runnerID].myObject.name;


            if (isBowlingMode)
            {
                //Bird.CheckpointManager.CM.AddCheckpoint(runnerTarget, runnerID);
            }


            Debug.Log(players[runnerID].myObject.name + " is the runner!");       
            int i = 1;
            foreach (VolcanoMadnessPlayer player in players)
            {               
                if (player.myRole == VolcanoMadnessPlayer.Role.Blocker)
                {
                    player.myObject.transform.position = spawnPoints[i].position;
                    player.myObject.transform.rotation = spawnPoints[i].rotation;

                    if (isBowlingMode)
                    {
                        Kojima.CarSwapManager.m_sInstance.ChangeCar(player.myObject.GetComponent<Kojima.CarScript>().m_nplayerIndex - 1,
                        Kojima.GameController.s_singleton.m_players[player.myObject.GetComponent<Kojima.CarScript>().m_nplayerIndex - 1].m_nControllerID, Kojima.CarSwapManager.CarType.LOUIS);
                    }
                    else
                    {
                        Kojima.CarSwapManager.m_sInstance.ChangeCar(player.myObject.GetComponent<Kojima.CarScript>().m_nplayerIndex - 1,
                        Kojima.GameController.s_singleton.m_players[player.myObject.GetComponent<Kojima.CarScript>().m_nplayerIndex - 1].m_nControllerID, Kojima.CarSwapManager.CarType.ZUK);
                        players[i].myObject = Kojima.GameController.s_singleton.m_players[i].gameObject;
                    }

                    i++;

                    //Bird.CheckpointManager.CM.AddCheckpoint(Kojima.GameController.s_singleton.m_players[runnerID].gameObject, i);
                }
            }
            foreach(VolcanoMadnessPlayer player in players)
            {
                if (player.myObject)
                {
                    if (!player.myObject.GetComponent<Bam.CalculateAirtime>())
                    {
                        player.myObject.gameObject.AddComponent<Bam.CalculateAirtime>();
                    }
                }
            }

            roundCountdownCompleted = false;
            roundCountdown = countdownDuration;

            roundTimer = roundDuration;

            if (isBowlingMode == true)
            {
                bowlingCompleted = false;
                bowlingTime = bowlingTimeLimit;
            }
            else
            {
                bowlingCompleted = true;
            }

            if (currentRound > 1)
            {
                //BamResultsScript.ShowResults("Dunno").GiveScores(playerScores);
                //enabled = false;
            }

        }

        public void EndRound(bool reached)
        {
            goalReached = reached;
            roundTimer = 0;

            //Bird.CheckpointManager.CM.RemoveCheckpoint(Kojima.GameController.s_singleton.m_players[runnerID].gameObject, 1);
            //Bird.CheckpointManager.CM.RemoveCheckpoint(Kojima.GameController.s_singleton.m_players[runnerID].gameObject, 2);
            //Bird.CheckpointManager.CM.RemoveCheckpoint(Kojima.GameController.s_singleton.m_players[runnerID].gameObject, 3);
            //Bird.CheckpointManager.CM.RemoveCheckpoint(Kojima.GameController.s_singleton.m_players[runnerID].gameObject, 4);
        }

        new
        void Start()
        {
            gamemodeRunning = false;
            singleton = this;
            base.Start();
            m_active = true;

            m_currentPhase = m_phases[0];

            Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_HIDE_ALL_ELEMENTS);

            // Unhide the elements we want (this event accepts both hudElementToggleData_t and hudElementToggleMultiData_t - multi accepts an array of types)
            Bird.HUDController.hudElementToggleMultiData_t dataobject = new Bird.HUDController.hudElementToggleMultiData_t();
            dataobject.m_nPlayerID = 0; // Target player ID 0 = all players (otherwise, it's 1 - 4)
            dataobject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.ENABLE;
            dataobject.m_ArrayTypes = new System.Type[] { typeof(Bird.HUD_EXP), typeof(Bird.HUD_ScorePopupMgr), typeof(Bird.HUD_NavArrow) };

            // This will enable the exp display, timer, score popup and race position
            Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, dataobject);

            pinCam.enabled = false;
            canvasRef.ClearHints();
        }

        void OnTriggerEnter(Collider col)
        {
            if(col.gameObject.CompareTag("Player"))
            {
                Kojima.CarScript thisCar = col.gameObject.GetComponent<Kojima.CarScript>();

                if (thisCar.m_nplayerIndex-1 == runnerID)
                {
                    pinCam.enabled = true;

                    if (!isBowlingMode)
                    {
                        if (VolcanoMadnessCanvas.singleton != null)
                        {
                            VolcanoMadnessCanvas.singleton.ResetAirTime();
                        }
                    }

                    EndRoundEarly(8);
                    //Bird.CheckpointManager.CM.RemoveCheckpoint(Kojima.GameController.s_singleton.m_players[runnerID].gameObject, 0);
                }
                else
                {
                    Debug.Log(thisCar.name + " entered the trigger but isn't the runner");
                }
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Kojima.CarScript thisCar = col.gameObject.GetComponent<Kojima.CarScript>();

                if (thisCar.AllWheelsGrounded && thisCar.CurrentlyInWater)
                {
                    if (thisCar.m_nplayerIndex - 1 == runnerID)
                    {
                        pinCam.enabled = false;
                    }
                    else
                    {
                        Debug.Log(thisCar.name + " left the trigger but isn't the runner");
                    }
                }
            }
        }

        void GatherPlayers()
        {
            players = new List<VolcanoMadnessPlayer>();
            playerObjects = new List<GameObject>();

            foreach (Kojima.CarScript carscript in Kojima.GameController.s_singleton.m_players)
            {
                if (carscript)
                {
                    playerObjects.Add(carscript.gameObject);
                }
            }

            if (playerScores == null || playerScores.Length == 0)
            {
                playerScores = new int[Kojima.GameController.s_ncurrentPlayers];
            }

            players.Add(new VolcanoMadnessPlayer(playerObjects[0], 1));

            if (Kojima.GameController.s_ncurrentPlayers >= 2)
                players.Add(new VolcanoMadnessPlayer(playerObjects[1], 2));

            if (Kojima.GameController.s_ncurrentPlayers >= 3)
                players.Add(new VolcanoMadnessPlayer(playerObjects[2], 3));

            if (Kojima.GameController.s_ncurrentPlayers == 4)
                players.Add(new VolcanoMadnessPlayer(playerObjects[3], 4));
        }

        public void GiveScore(int score)
        {
            playerScores[runnerID] += score;
            Debug.Log("Player " + runnerID + " has scored " + score + " and now has " + playerScores[runnerID]);
        }

        new
       void Update()
        {
            base.Update();
            gamemodeRunning = m_active;

            //Game Mode Loop
            if (m_active)
            {
                VolcanoMadnessCanvas.singleton.Toggle(roundCountdownCompleted);
                VolcanoMadnessCanvas.singleton.GiveTimeValue(Mathf.FloorToInt(roundTimer), pinCam.enabled);

                if (setupDone == false)
                {
                    GatherPlayers();

                    roundInterval = intervalDuration;
                    roundTimer = roundDuration;

                    foreach (VolcanoMadnessPlayer player in players)
                    {
                        if (player.myObject)
                        {
                            player.myObject.AddComponent<CalculateAirtime>();
                        }
                    }
                    NewRound();
                            
                            
                    setupDone = true;
                }
                
                if (roundCountdownCompleted == true)
                {
                    if (roundTimer <= 0)
                    {
                        m_currentPhase = GetPhase("Ending");
                        if (m_currentPhase.m_timer.m_bCounting == false)
                        {
                            m_currentPhase.m_timer.StartTimer();
                        }
                        if (isBowlingMode == true && bowlingCompleted == false)
                        {
                            if (bowlingTime >= 0)
                            {
                                bowlingTime -= Time.deltaTime;
                            }
                            else
                            {
                                bowlingCompleted = true;
                            }
                            
                        }
                        else
                        {
                            Kojima.GameController.s_singleton.AllCarsCanMove(false);
                            if (roundInterval > 0)
                            {
                                roundInterval -= Time.deltaTime;                              
                            }
                        }

                        if (roundInterval <= 0 && (!isBowlingMode || bowlingCompleted == true) && Kojima.CarScript.AreAllCarsAvailable)
                        {
                            
                            for (int i=0; i<Kojima.GameController.s_ncurrentPlayers; i++)
                            {
                                Kojima.GameController.s_singleton.m_players[i].ResetCar();
                            }

                            roundTimer = roundDuration;
                            roundInterval = intervalDuration;

                            if (goalReached == true)
                            {
                                if (isBowlingMode == false)
                                {
                                    playerScores[runnerID] += (int)(players[runnerID].myObject.GetComponent<CalculateAirtime>().m_fLastAirtime * 100) + (int)roundTimer * 5;
                                    Debug.Log("Player " + runnerID + " Scored an airtime of " + playerScores[runnerID]);
                                    NewRound();
                                }
                                else
                                {
                                    BowlingHUDScript.singleton.FadeAway();
                                    //playerScores[runnerID] = bowlingManager.CheckBowlingScore();
                                    NewRound();
                                }

                            }
                            else
                            {
                                //Debug.Log("runner id : " + runnerID);
                                playerScores[runnerID] += 0;
                                Debug.Log("Player " + runnerID + " Scored an airtime of " + playerScores[runnerID]);

                                if (isBowlingMode)
                                {
                                    BowlingHUDScript.singleton.FadeAway();
                                }

                                NewRound();
                            }
                        }
                    }
                    else
                    {
                        roundTimer -= Time.deltaTime;
                        m_currentPhase = GetPhase("Running");
                        if (m_currentPhase.m_timer.m_bCounting == false)
                        {
                            m_currentPhase.m_timer.StartTimer();
                        }
                        if (CheckIfRunnerDied())
                        {
                            if(roundTimer>1)
                            {
                                EndRoundEarly(0.1f);
                            }
                        }
                    }
                }
                else
                {
                    m_currentPhase = GetPhase("Setup");

                    if (m_currentPhase.m_timer.m_bCounting == false)
                    {
                        m_currentPhase.m_timer.StartTimer();
                    }
                    roundCountdown -= Time.deltaTime * 0.9f;

                    

                    if (Mathf.Round(roundCountdown) <= 4)
                    {
                        countdownRef.SetCountdown(Mathf.RoundToInt(roundCountdown));

                        if (!displayHints)
                        {
                            if (isBowlingMode)
                            {
                                canvasRef.SetHintText("Stop P" + (runnerID + 1) + " reaching the pins!");
                            }
                            else
                            {
                                canvasRef.SetHintText("Stop P" + (runnerID + 1) + " reaching the ramp!");
                            }

                            displayHints = true;
                        }

                        if (roundCountdown <= 3.4f)
                        {
                            countdownCanvas.enabled = true;
                            canvasRef.ResetHints();
                        }
                        
                        if(roundCountdown <= 0.3f)
                        {
                            if (displayHints && !hintDisplayed)
                            {
                                //if (isBowlingMode)
                                {

                                }
                            }
                        }                    
                        //countdownText.text = Mathf.Round(roundCountdown).ToString();
                    }              
                    else
                    {
                        //countdownText.text = "";
                    }

                    Kojima.GameController.s_singleton.AllCarsCanMove(false);

                    if (roundCountdown <= 0)
                    {
                        Kojima.GameController.s_singleton.AllCarsCanMove(true);
                        roundCountdownCompleted = true;
                        countdownCanvas.enabled = false;

                        canvasRef.HideHints();
                        displayHints = false;
                        hintDisplayed = true;
                    }
                }
                

                ////Game Modes are required to have an exit point
                //if (Input.GetKeyDown(KeyCode.Return))
                //{
                //    EndGame();
                //}
            }
        }

        bool CheckIfRunnerDied()
        {
            for(int i=0; i<Kojima.GameController.s_singleton.m_players.Length; i++)
            {
                if(Kojima.GameController.s_singleton.m_players[i])
                {
                    if(Kojima.GameController.s_singleton.m_players[i].CurrentlyInWater && Kojima.GameController.s_singleton.m_players[i].m_nplayerIndex-1==runnerID)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Handles game logic when the game ends
        /// </summary>
        /// 
        new
        void EndGame()
        {
            if (bowlingManager)
            {
                bowlingManager.RemovePins();
            }

            //Sets game to inactive
            m_active = false;

            ResetEvent();

            //Sets the game manager back to freeroam to initalise the game mode's deactivation globaly
            Kojima.GameModeManager.m_instance.m_currentMode = Kojima.GameModeManager.GameModeState.FREEROAM;

            // Resets the gamemode specific score
            HF.PlayerExp.ResetCurrentEXP();

            //Sets the game manager back to freeroam to initalise the game mode's deactivation globaly
            //Kojima.GameModeManager.m_instance.m_currentMode = Kojima.GameModeManager.GameModeState.FREEROAM;
            Debug.Log(m_numberOfPlayers);
            m_numberOfPlayers = Kojima.GameController.s_ncurrentPlayers;

            if(isBowlingMode)
                BamResultsScript.ShowResults(this, "Bowling Madness").GiveScores(playerScores);
            else
                BamResultsScript.ShowResults(this, "Volcano Madness").GiveScores(playerScores);
            enabled = false;

            //base.EndGame();
        }

        public void EndRoundEarly(float interval)
        {
            if (roundTimer > 0)
            {
                Debug.Log("Round ending early");
                roundTimer = 0;
                //roundInterval = interval;
            }

            if (isBowlingMode)
            {
                bowlingTime = interval;
            }
            else
            {
                roundTimer = interval;
            }

        }
    }
}

