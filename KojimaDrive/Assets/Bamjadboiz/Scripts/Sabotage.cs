using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Bam
{
    public class Sabotage : Kojima.GameMode
    {
        public Canvas countdownCanvas;
        public Text countdownText;
        [Range(1, 10)]
        public float countdownDuration;
        float roundCountdown;
        bool roundCountdownCompleted;
        bool roundFailed;
        bool trapSetup;
        [SerializeField]
        int currentRound = 0;
        [Range(10, 120)]
        public float roundDuration;
        [Range(1, 10)]
        public float intervalDuration;
        public float roundTimer;
        public float roundInterval;
        public bool gamemodeRunning;
        [Range(1, 10)]
        public float outOfCircleDuration;
        float outOfCircleTimer;

        protected int chaserID = -1;
        public int m_chaserID { get { return chaserID; } }
        public bool chaserSurvived;

        public int trapInterval;

        public float m_outOfCircleTimer { get { return outOfCircleTimer; } }

        public int[] playerScores;

        public List<GameObject> playerObjects;

        public float surviveTime;

        public Transform[] spawnPoints;

        bool setupDone = false;

        public string chaserName;

        public List<SabotagePlayer> players;

        public SabotageZoneScript m_sabotageZoneScript;
		public RunnerBoundsScript m_runnerBounds;
		public ArrowScript m_chaserArrow;
		public Vector3 m_arrowLocalOffset;

		public EventHandler GameStartEvent;
		public EventHandler GameEndEvent;
		public EventHandler RoundStartEvent;
		public EventHandler RoundEndEvent;

        [SerializeField]
        AudioSource alarmTone;
        bool alarmRunning = false;
        CountdownScript m_myCountdown;

		protected void NewRound()
        {
            Kojima.CatchupBoost.s_singleton.m_groupCatchupEnabled = false;
            if(currentRound==Kojima.GameController.s_ncurrentPlayers)
            {
                foreach (Kojima.CarScript player in Kojima.GameController.s_singleton.m_players)
                {
                    if (player)
                    {
                        if (player.gameObject.GetComponent<Bam.TrapSpawner>())
                        {
                            player.gameObject.GetComponent<Bam.TrapSpawner>().spawnTraps = false;
                            player.gameObject.GetComponent<Bam.TrapSpawner>().DeleteTraps();
                        }
                    }
                }
                EndGame();
                return;
            }

            alarmTone.Stop();
            outOfCircleTimer = 0;

            GetPhase("Setup").m_timer.ResetTimer();
            GetPhase("Running").m_timer.ResetTimer();
            GetPhase("Ending").m_timer.ResetTimer();

            

            trapSetup = false;
            countdownCanvas.enabled = true;
            //added by c3mundy 02/05/17
            m_myCountdown = countdownCanvas.GetComponent<CountdownScript>();

            chaserSurvived = true;
            surviveTime = 0.0f;

            foreach (SabotagePlayer player in players)
            {
                player.myRole = SabotagePlayer.Role.Runner;
                player.myObject.GetComponent<Kojima.CarScript>().ResetCar();
                player.myObject.GetComponent<Bam.TrapSpawner>().spawnTraps = false;
                
            }
            chaserID++;
            if (chaserID == Kojima.GameController.s_ncurrentPlayers)
            {
                chaserID = 0;
            }
            Debug.Log(chaserID);
            currentRound++;

            //Kojima.CameraManagerScript.singleton.SetupThirdPersonForAllPlayers();
            Kojima.CameraManagerScript.screenSetup_s newSetup = new Kojima.CameraManagerScript.screenSetup_s(2);
            newSetup.camInfos = new PlayerCameraScript.CameraInfo[2];

            newSetup.camInfos[0].m_positionOnScreen = PlayerCameraScript.screenPositions_e.topHalf;
            newSetup.camInfos[1].m_positionOnScreen = PlayerCameraScript.screenPositions_e.bottomHalf;

            newSetup.camInfos[0].m_viewStyle = PlayerCameraScript.viewStyles_e.thirdPerson;
            newSetup.camInfos[1].m_viewStyle = PlayerCameraScript.viewStyles_e.overhead;

            newSetup.camInfos[0].m_nmainPlayer = chaserID + 1;

            newSetup.camInfos[1].m_followThesePlayers = new bool[4];

            for (int id = 0; id < Kojima.GameController.s_ncurrentPlayers; id++)
            {
				newSetup.camInfos[1].m_followThesePlayers[id] = id != chaserID;
			}

            Kojima.CameraManagerScript.singleton.NewScreenSetup(newSetup);
            //Kojima.CameraManagerScript.singleton.SetupThirdPersonForAllPlayers();

            Debug.Log("Current Round: " + currentRound);

            players[chaserID].myRole = SabotagePlayer.Role.Chaser;
            players[chaserID].myObject.transform.position = spawnPoints[0].position;
            players[chaserID].myObject.transform.rotation = spawnPoints[0].rotation;
            Kojima.CarSwapManager.m_sInstance.ChangeCar(chaserID, Kojima.GameController.s_singleton.m_players[chaserID].m_nControllerID, Kojima.CarSwapManager.CarType.CARLO);
            players[chaserID].myObject = Kojima.GameController.s_singleton.m_players[chaserID].gameObject;
            chaserName = players[chaserID].myObject.name;
            Debug.Log(players[chaserID].myObject.name + " is the chaser!");
            int i = 1;
			int playerIdx = 1;
            foreach (SabotagePlayer player in players)
			{   
				/* Added by Yams 18/04/2017 */
				RunnerBoundsScript bounds = player.myObject.GetComponent<RunnerBoundsScript>();
                
                if (player.myRole == SabotagePlayer.Role.Runner)
                {
                    player.myObject.transform.position = spawnPoints[i].position;
                    player.myObject.transform.rotation = spawnPoints[i].rotation;
                    Kojima.CarSwapManager.m_sInstance.ChangeCar(player.myObject.GetComponent<Kojima.CarScript>().m_nplayerIndex - 1,
                        Kojima.GameController.s_singleton.m_players[player.myObject.GetComponent<Kojima.CarScript>().m_nplayerIndex - 1].m_nControllerID, Kojima.CarSwapManager.CarType.RV);
                    player.myObject = Kojima.GameController.s_singleton.m_players[player.myObject.GetComponent<Kojima.CarScript>().m_nplayerIndex - 1].gameObject;
                    Kojima.CatchupBoost.s_singleton.m_cars.Add(player.myObject.GetComponent<Kojima.CarScript>());

					++i;
                }
				/* Added by Yams 18/04/2017 */
				else
				{
					string layerName = "InWorldUIP" + playerIdx;
					m_chaserArrow.transform.parent = player.myObject.transform;
					m_chaserArrow.gameObject.layer = LayerMask.NameToLayer(layerName);
					m_chaserArrow.transform.localPosition = m_arrowLocalOffset;
					m_chaserArrow.transform.localRotation = Quaternion.identity;
					if (bounds != null) Destroy(bounds);
				}

                if (!player.myObject.GetComponent<TrapSpawner>())
                {
                    player.myObject.AddComponent<TrapSpawner>();
                    player.myObject.GetComponent<Bam.TrapSpawner>().spawnInterval = trapInterval;
                    if (player.myRole == SabotagePlayer.Role.Chaser)
                    {
                        player.myObject.GetComponent<Bam.TrapSpawner>().spawnTraps = false;
                    }
                    else
                    {
                        player.myObject.GetComponent<Bam.TrapSpawner>().spawnTraps = true;
                    }
                }

				//Added by yams 02/05/17
				CarZoneScript playerZonePull = player.myObject.GetComponent<CarZoneScript>();
				if (!playerZonePull)
				{
					playerZonePull = player.myObject.AddComponent<CarZoneScript>();
					playerZonePull.m_zone = m_sabotageZoneScript;
				}
				playerZonePull.m_pullbackAffectsThisCar = player.myRole == SabotagePlayer.Role.Runner;
				//--

				++playerIdx;
                player.myObject.GetComponent<Rigidbody>().isKinematic = true;
            }
            roundCountdownCompleted = false;
            roundCountdown = countdownDuration;


			if (RoundStartEvent != null) RoundStartEvent(this, EventArgs.Empty);
            Kojima.CatchupBoost.s_singleton.m_groupCatchupEnabled = true;
            
        }

        new
        void Start()
        {
            base.Start();
            m_active = true;

            Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_HIDE_ALL_ELEMENTS);

            // Unhide the elements we want (this event accepts both hudElementToggleData_t and hudElementToggleMultiData_t - multi accepts an array of types)
            Bird.HUDController.hudElementToggleMultiData_t dataobject = new Bird.HUDController.hudElementToggleMultiData_t();
            dataobject.m_nPlayerID = 0; // Target player ID 0 = all players (otherwise, it's 1 - 4)
            dataobject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.ENABLE;
            dataobject.m_ArrayTypes = new System.Type[] { typeof(Bird.HUD_EXP), typeof(Bird.HUD_ScorePopupMgr), typeof(Bird.HUD_NavArrow)};

            // This will enable the exp display, timer, score popup and race position
            Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, dataobject);

            SabotageHUDScript.singleton.Toggle(false);

            StartCoroutine(HandleAlarmTone());
        }

        new
       void Update()
        {
            base.Update();
            gamemodeRunning = m_active;

            //Game Mode Loop           
            if (m_active)
            {
                SabotageHUDScript.singleton.Toggle(roundCountdownCompleted);
                SabotageHUDScript.singleton.GiveTimeValue(Mathf.FloorToInt(roundTimer));
                if (setupDone == false)
                {
                    players = new List<SabotagePlayer>();
                    playerObjects = new List<GameObject>();

                    foreach (Kojima.CarScript carscript in Kojima.GameController.s_singleton.m_players)
                    {
                        if (carscript != null)
                        {
                            playerObjects.Add(carscript.gameObject);
                        }
                    }

                    playerScores = new int[Kojima.GameController.s_ncurrentPlayers];

                    players.Add(new SabotagePlayer(playerObjects[0], 1));

                    if(Kojima.GameController.s_ncurrentPlayers >= 2)
                        players.Add(new SabotagePlayer(playerObjects[1], 2));

                    if(Kojima.GameController.s_ncurrentPlayers>=3)
                        players.Add(new SabotagePlayer(playerObjects[2], 3));

                    if(Kojima.GameController.s_ncurrentPlayers==4)
                        players.Add(new SabotagePlayer(playerObjects[3], 4));


                    roundInterval = intervalDuration;
                    roundTimer = roundDuration;
                    surviveTime = 0;
                    foreach (SabotagePlayer player in players)
                    {
                        if (!player.myObject.GetComponent<TrapSpawner>())
                        {
                            TrapSpawner trapSpawner = player.myObject.AddComponent<TrapSpawner>();
							trapSpawner.spawnInterval = trapInterval;
							trapSpawner.spawnTraps = false;
							//trapSpawner.m_runnerBounds = m_runnerBounds;
						} 
                    }
					if (GameStartEvent != null) GameStartEvent(this, EventArgs.Empty);

					NewRound();
                    setupDone = true;

                }
                if (roundCountdownCompleted == true)
                {
                    foreach(SabotagePlayer player in players)
                    {
                        player.myObject.GetComponent<Rigidbody>().isKinematic = false;

                    }
                    if (trapSetup == false)
                    {
                        //foreach (SabotagePlayer player in players)
                        //{
                        //    if (player.myRole == SabotagePlayer.Role.Runner)
                        //    {
                        //        player.myObject.GetComponent<Bam.TrapSpawner>().spawnTraps = true;
                        //    }
                        //}
                        trapSetup = true;
                    }
                    if (roundTimer <= 0)
                    {
                        Kojima.GameController.s_singleton.AllCarsCanMove(false);
                        Kojima.CatchupBoost.s_singleton.m_groupCatchupEnabled = false;
                        m_currentPhase = GetPhase("Ending");
                        foreach (SabotagePlayer player in players)
                        {
                            player.myObject.GetComponent<Rigidbody>().isKinematic = true;
                        }
                        if (m_currentPhase.m_timer.m_bCounting == false)
                        {
                            m_currentPhase.m_timer.StartTimer();
                        }
                        if (roundInterval > 0)
                        {
                            
                            roundInterval -= Time.deltaTime;
                        }

                        if (roundInterval <= 0)
                        {
                            //if (currentRound == Kojima.GameController.s_ncurrentPlayers)
                            //{
                            //    base.EndGame();
                            //}
                            foreach (SabotagePlayer player in players)
                            {
                                player.myObject.GetComponent<Kojima.CarScript>().ResetCar();

                                foreach (Kojima.CarScript carscript in Kojima.GameController.s_singleton.m_players)
                                {
                                    if (carscript)
                                    {
                                        if (carscript.gameObject.GetComponent<Bam.TrapSpawner>())
                                        {
                                            carscript.gameObject.GetComponent<Bam.TrapSpawner>().DeleteTraps();
                                        }
                                    }
                                }
                            }
                            roundTimer = roundDuration;
                            roundInterval = intervalDuration;
                            playerScores[chaserID] = (int)surviveTime * 10;
                            Kojima.CatchupBoost.s_singleton.m_cars.Clear();

                            NewRound();
                        }
                    }
                    else if (m_sabotageZoneScript.m_chaserInZone == false)
                    {

                        if (roundCountdownCompleted && Kojima.GameController.s_singleton.m_players[chaserID].CanMove)
                        {
                            outOfCircleTimer += Time.deltaTime;
                        }

                        Bam.SabotageHUDScript.singleton.WarnChaser();
                        if (outOfCircleTimer >= outOfCircleDuration)
                        {
                            EndRound();
                        }
                    }
                    else
                    {
                        outOfCircleTimer = 0;
                        roundTimer -= Time.deltaTime;
                        surviveTime += Time.deltaTime;
                        m_currentPhase = GetPhase("Running");
                        if (m_currentPhase.m_timer.m_bCounting == false)
                        {
                            m_currentPhase.m_timer.StartTimer();
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
                    roundCountdown -= Time.deltaTime;

                    if (Mathf.Round(roundCountdown) <= 3)
                    {
                        //countdownText.text = Mathf.Round(roundCountdown).ToString();
                        m_myCountdown.SetCountdown(Mathf.RoundToInt(roundCountdown));

                        if (Bam.SabotageHUDScript.singleton.GetFadeTimer <= 0)
                        {
                            Bam.SabotageHUDScript.singleton.ResetHints();
                            Bam.SabotageHUDScript.singleton.DisplayStartHints(chaserID);
                        }
                    }
                    else
                    {
                        countdownText.text = "";
                    }
                    Kojima.GameController.s_singleton.AllCarsCanMove(false);
                    if (roundCountdown <= 0)
                    {
                        Kojima.GameController.s_singleton.AllCarsCanMove(true);
                        roundCountdownCompleted = true;
                        countdownCanvas.enabled = false;
                    }
                }

                if (outOfCircleTimer > 0)
                {
                    if (!alarmRunning && enabled)
                    {
                        StartCoroutine(HandleAlarmTone());
                    }
                }

                Bam.SabotageHUDScript.singleton.GiveOutOfCircleTimer(outOfCircleTimer);
            }
        }

        IEnumerator HandleAlarmTone()
        {
            float pause = 2;
            float duration = 0.75f;

            if (!alarmRunning)
            {
                while (outOfCircleTimer > 0 && roundCountdownCompleted && roundTimer<roundDuration-1 && roundTimer>0)
                {
                    alarmRunning = true;

                    pause = 0.3f - outOfCircleTimer * 0.2f;
                    duration = 0.2f - outOfCircleTimer * 0.25f;

                    pause = Mathf.Clamp(pause, 0.05f, 1);
                    duration = Mathf.Clamp(duration, 0.05f, 1);

                    alarmTone.pitch = 1 + (outOfCircleTimer);

                    alarmTone.Play();
                    yield return new WaitForSeconds(duration);

                    alarmTone.Stop();
                    yield return new WaitForSeconds(pause);
                }

                alarmRunning = false;
            }
        }

        public void EndRound()
        {
            chaserSurvived = false;

            if(roundTimer>0 && outOfCircleTimer>=outOfCircleDuration)
            {
                Kojima.GameController.s_singleton.m_players[chaserID].Explode();
            }

            roundTimer = 0;

			if (RoundEndEvent != null) RoundEndEvent(this, EventArgs.Empty);
		}

        /// <summary>
        /// Handles game logic when the game ends
        /// </summary>
       new void EndGame()
        {

            foreach (Kojima.CarScript player in Kojima.GameController.s_singleton.m_players)
            {
                if (player)
                {
                    if (player.gameObject.GetComponent<Bam.TrapSpawner>())
                    {
                        player.gameObject.GetComponent<Bam.TrapSpawner>().DeleteTraps();
                    }
                }
            }
            //if (GameEndEvent != null) GameEndEvent(this, EventArgs.Empty);

            /* Remove arrow from car */
            m_chaserArrow.transform.parent = null;


            ////Sets game to inactive
            //m_active = false;

            //////Sets the game manager back to freeroam to initalise the game mode's deactivation globaly
            ////Kojima.GameModeManager.m_instance.m_currentMode = Kojima.GameModeManager.GameModeState.FREEROAM;
            ////Bam.LobbyManagerScript.singleton.ReturnToLobby();
            //base.EndGame();

            //Sets game to inactive
            m_active = false;

            //Sets the game manager back to freeroam to initalise the game mode's deactivation globaly
            //Kojima.GameModeManager.m_instance.m_currentMode = Kojima.GameModeManager.GameModeState.FREEROAM;
            Debug.Log(m_numberOfPlayers);
            m_numberOfPlayers = Kojima.GameController.s_ncurrentPlayers;
            BamResultsScript.ShowResults(this, "Sabotage").GiveScores(playerScores);
            enabled = false;
            //base.EndGame();
        }
    }

    public class SabotagePlayer
    {
        public GameObject myObject;
        int myID;
        public float myTimeSurvived;

        public enum Role
        {
            Runner,
            Chaser
        }

        public Role myRole;

        public SabotagePlayer(GameObject gameobject, int ID)
        {
            myObject = gameobject;
            myID = ID;
            myRole = Role.Runner;
        }
    }
}