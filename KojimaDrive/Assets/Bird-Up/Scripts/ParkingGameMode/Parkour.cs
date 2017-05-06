using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bird
{
    public class Parkour : Kojima.GameMode
    {
       Kojima.GameController GC;

       public enum ParkingState
        {
            SETUP= 0,
            PLAYING,
            MIDROUND,
            ENDING
        }
        public ParkingState currentState;


        private GameObject endPoint;
        List<int> locations;

        public GameObject parkingSpace;
        GameObject parkingSpaceClone;

        public float roundTimerPlaying;
        public float roundTimerInterval;
        public float camflybyLength;
        public float heldRoundTimer;
        float heldRoundTimerInterval;

        public Transform[] collectionOfStartPoints;
        public Transform[] collectionOfEndPoints;
        public GameObject CameraSplines;

        private Transform[] currentStartLocs;
        //a list of 'levels' that have already been used
        private List<int> usedLevelList;

        public bool randomStart;
        public int nonRandomStartLoc;

        public GameObject displayCamera;
        public GameObject midRoundObject;

        int currentRound;

        GameObject cameraMount;
        GameObject cameraViewTarget;

        Transition_Generic TRG;

        AddonManager ADM;

        SplineFollower CSF;
        Bird.HUDController.hudElementToggleMultiData_t dataobject;

        public void Start()
        {
            base.Start();
            heldRoundTimer = roundTimerPlaying;
            heldRoundTimerInterval = roundTimerInterval;
            cameraMount = new GameObject();
            cameraMount.name = "CameraMount";
            m_active = true;

            ADM = GameObject.Find("AddonManager(Clone)").GetComponent<AddonManager>();

            currentState = ParkingState.SETUP;
            currentRound = 0;
            TRG = GameObject.Find("Transition_GameMode_Default_Out(Clone)").GetComponent<Transition_Generic>();
            buildFlyCam();
            BeginParking();
        
			Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_HIDE_ALL_ELEMENTS);

            // Unhide the elements we want (this event accepts both hudElementToggleData_t and hudElementToggleMultiData_t - multi accepts an array of types)
            dataobject = new Bird.HUDController.hudElementToggleMultiData_t();
			dataobject.m_nPlayerID = 0; // Target player ID 0 = all players (otherwise, it's 1 - 4)
			dataobject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.ENABLE;
			dataobject.m_ArrayTypes = new System.Type[] { typeof(Bird.HUD_EXP), typeof(Bird.HUD_ScorePopupMgr), typeof(Bird.HUD_NavArrow) };

			// This will enable the exp display, timer, score popup and race position
			
            
    

        }
        bool isScore = true;
        void ToggleScore()
        {
            isScore = !isScore;

            if (isScore)
            {
                displayCamera.SetActive(true);
                for (int i = 0; i < GC.m_players.Length; i++)
                {
                    if (GC.m_players[i] != null)
                    {
                        midRoundObject.transform.GetChild(1).transform.GetChild(i).gameObject.SetActive(true);
                        midRoundObject.transform.GetChild(2).transform.GetChild(i).gameObject.SetActive(true);
                        midRoundObject.transform.GetChild(3).transform.GetChild(i).gameObject.SetActive(true);
                        midRoundObject.transform.GetChild(0).gameObject.SetActive(true);
                    }

                }
            }
            else
            {
                displayCamera.SetActive(false);
                midRoundObject.transform.GetChild(0).gameObject.SetActive(false);
                foreach(Transform t in midRoundObject.transform.GetChild(1).transform)
                {
                    t.gameObject.SetActive(false);
                }
                foreach (Transform t in midRoundObject.transform.GetChild(2).transform)
                {
                    t.gameObject.SetActive(false);
                }
                foreach (Transform t in midRoundObject.transform.GetChild(3).transform)
                {
                    t.gameObject.SetActive(false);
                }
            }

        }

        void buildFlyCam()
        {
        

            cameraMount.AddComponent<SplineFollower>();
            cameraMount.AddComponent<Camera>();
            cameraMount.AddComponent<SimpleLookAt>();

            CSF = cameraMount.GetComponent<SplineFollower>();
            cameraViewTarget = new GameObject();
            cameraViewTarget.name = "ViewTarget";

            cameraViewTarget.AddComponent<SplineFollower>();

            cameraViewTarget.GetComponent<SplineFollower>().duration = camflybyLength;
            cameraMount.GetComponent<SimpleLookAt>().target = cameraViewTarget.transform;

            CSF.resetProgress();
            CSF.duration = camflybyLength;
        }

        void setSpline(int _in)
        {
            cameraViewTarget.GetComponent<SplineFollower>().spline = CameraSplines.transform.GetChild(_in).gameObject.transform.GetChild(0).gameObject.GetComponent<BezierSpline>();
            CSF.spline = CameraSplines.transform.GetChild(_in).gameObject.transform.GetChild(1).GetComponent<BezierSpline>();
        }
    
        void toggleControl()
        {

            Kojima.CarScript.s_playersCanMove = !Kojima.CarScript.s_playersCanMove;

            //foreach (Kojima.CarScript s in GC.m_players)
            //{
            //    s.enabled = !s.enabled;
            //}
        }

        void setControl(bool _in)
        {
            Kojima.CarScript.s_playersCanMove = _in;
            //foreach (Kojima.CarScript s in GC.m_players)
            //{
            //    s.enabled = _in;
            //}
        }

        void BeginParking()
        {
            GC = FindObjectOfType<Kojima.GameController>();
            toggleControl();
            parkingSpaceClone = Instantiate(parkingSpace);
            locations = new List<int>();


           
            for (int i = 0; i < collectionOfStartPoints.Length; i++)
            {
                locations.Add(i);
            }

            Bird.CheckpointManager.CM.AddCheckpoint(parkingSpaceClone);

            //Add arrow system Start here 

            if (collectionOfStartPoints.Length != collectionOfEndPoints.Length)
            {
            //    print("Error: Parking points must have equatable stat points");
                //enabled = false;
            }
            else
            {
                int start = 0;

                if (randomStart)
                {
                    start = Random.Range(0, collectionOfStartPoints.Length);
                    //Do random number of size of array here
                }

                ToggleScore();
                setUpCars();
             
                moveToStart(start);
            }
        }

        void setUpCars()
        {
            for(int i = 0; i < GC.m_players.Length; i++)
            {
                if (GC.m_players[i] != null)
                {
            
                    GC.m_players[i].gameObject.AddComponent<ParkingGameModeTracker>();
                }
            }
        }

        void cleanUpGameMode()
        {
            foreach (Kojima.CarScript player in GC.m_players)
            {
                if (player != null)
                {
                    Destroy(player.gameObject.GetComponent<ParkingGameModeTracker>());
                }
            }
            setControl(true);
            CheckpointManager.CM.RemoveCheckpoint(parkingSpaceClone);
            Destroy(parkingSpaceClone);
            Destroy(cameraViewTarget);
           
  
        }

        void moveToStart(int _in)
        {

            int count = 0;
            foreach (Kojima.CarScript player in GC.m_players)
            {
                if (player != null)
                {
                    player.transform.position = collectionOfStartPoints[_in].transform.GetChild(count).transform.position;
                    player.transform.rotation = collectionOfStartPoints[_in].transform.GetChild(count).transform.rotation;
                    player.GetComponent<Rigidbody>().velocity = Vector3.zero;

                    player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    player.GetComponent<Rigidbody>().Sleep();
                    count++;
                }
            }

            parkingSpaceClone.transform.position = collectionOfEndPoints[_in].position;




        }
        //Core loop

            //Tick parking space in here
        public void Update()
        {
            base.Update();


            //Game happens here 
            if (m_active)
            {
                gamePlay();

            }
        }

        void gamePlay()
        {

            switch (currentState)
            {
                case ParkingState.SETUP:
                    setUpUpdate();
                    break;
                case ParkingState.PLAYING:
                    playUpdate();
                    break;
                case ParkingState.MIDROUND:
                    MidRoundUpdate();
                    break;
                case ParkingState.ENDING:
                    endParking();
                    break;

            }





        }

        void LateUpdate()
        {
            if (m_active)
            { 
                cameraMount.GetComponent<SimpleLookAt>().tick();
            }
        }

        bool roundSet = false;
        bool transitionControl= false;
        //use this as a kind of late update post inital setup
        void setUpUpdate()
        {
            if (!roundSet)
            {
                
                if ((currentRound + 1) <= collectionOfStartPoints.Length)
                {
                    setSpline(currentRound);
                    moveToStart(currentRound++);
                    roundSet = true;
                    cameraMount.SetActive(true);

                    CSF.resetProgress();
                }
                else
                {
             
                    currentState = ParkingState.ENDING;
                    return;
                }

            }

            if ((CSF.Progress >= 0.8f)&&(!transitionControl))
            {
                TRG.StartTransition();
                transitionControl = true;
            }


            if (CSF.Progress >= 1.0f)
            {
                for (int i = 0; i < GC.m_players.Length; i++)
                {
                    if (GC.m_players[i] != null)
                    {
                        GC.m_players[i].GetComponent<Kojima.CarScript>().PutAwayGlider();
                        Transform go = GC.m_players[i].transform.Find("Glider_Prefab(Clone)");
                        if (go != null)
                        {
                            go.gameObject.SetActive(false);
                        }
                    }
                }
                
                TRG.StartTransition();
                currentState = ParkingState.PLAYING;
                cameraMount.SetActive(false);
                transitionControl = false;

                toggleControl();
            }
         


        }

        float playCountDown= 5.0f;
        bool playDoOnce =false;
        bool tranControl = false;
        
        //Tick the current control point 
        void playUpdate()
        {
            if (!playDoOnce)
            {
                //dataobject.m_nState = HUDController.hudElementToggleData_t.elementState_e.TOGGLE;
                Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, dataobject);
				setControl (true);
                TRG.StartTransitionReverse();
                playDoOnce = true;
            }
            //if (playCountDown >= 0)
            //{
            //    playCountDown = playCountDown - Time.deltaTime;
                
            //    return;
            //}
            if((heldRoundTimer < 1.0f)&& !tranControl)
            {
           
                tranControl = true;
                TRG.StartTransition();
            }


            if (updateRoundTime(heldRoundTimerInterval))
            {
                tranControl = false;
                 playDoOnce = false;
                currentState = ParkingState.MIDROUND;
                toggleControl();
          
            }



        }

        bool updateRoundTime( float newTime = 30)
        {
            heldRoundTimer = heldRoundTimer - Time.deltaTime;
            if (heldRoundTimer <= 0)
            {
                heldRoundTimer = newTime;
                return true;
            }
            return false;
        }

        bool midroundDoOnce = false;
        float midroundCountTransit = 2.0f;

        bool endTransitOnce = false;

        //have some sort of camera flythrough
        void MidRoundUpdate()
        {
            if(!midroundDoOnce)
            {
                Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_HIDE_ALL_ELEMENTS);
                TRG.StartTransitionReverse();
                ToggleScore();
                setRoundWinner();
                midroundDoOnce = true;
   
            }

            if ((heldRoundTimerInterval  < 1.0f) && !endTransitOnce)
            {

                endTransitOnce = true;
                TRG.StartTransition();
            }

            displayCurrentScores();

            if (updateRoundTime(roundTimerPlaying))
            {
                endTransitOnce = false;
                midroundDoOnce = false;
                roundSet = false;
                TRG.StartTransitionReverse();
                ToggleScore();
                currentState = ParkingState.SETUP;
                resetPlayersScore();
            }
        }

        void displayCurrentScores()
        {
            Transform t = midRoundObject.transform.GetChild(2);
            for(int i = 0; i < GC.m_players.Length;i++)
            {
                if (GC.m_players[i] != null)
                {
                    t.GetChild(i).GetComponent<TypogenicText>().Text = GC.m_players[i].gameObject.GetComponent<ParkingGameModeTracker>().getScore().ToString();
                }
            }
             t = midRoundObject.transform.GetChild(3);
            for (int i = 0; i < GC.m_players.Length; i++)
            {
                if (GC.m_players[i] != null)
                {
                    t.GetChild(i).GetComponent<TypogenicText>().Text = GC.m_players[i].gameObject.GetComponent<ParkingGameModeTracker>().getWins().ToString();
                }
            }
        }


        void resetPlayersScore()
        {
            for (int i = 0; i < GC.m_players.Length; i++)
            {
                if (GC.m_players[i] != null)
                {
                    GC.m_players[i].GetComponent<ParkingGameModeTracker>().resetScore();
                }
            }
        }

        void setRoundWinner()
        {
            int currentHigh = -1;
            int currentHighScore = 0;

            for(int i = 0; i < GC.m_players.Length; i++)
            {
                if (GC.m_players[i] != null)
                {
                    int hold = GC.m_players[i].GetComponent<ParkingGameModeTracker>().getScore();
                    if (hold > currentHighScore)
                    {
                        currentHighScore = hold;
                        currentHigh = i;
                    }
                }
            }

            if(currentHigh != -1)
            {
                GC.m_players[currentHigh].GetComponent<ParkingGameModeTracker>().addWin();
                HF.PlayerExp.AddEXP(currentHigh, 100, true, true, "ROUND WIN!");
            }


        }

        void rewardWinner()
        {
            int currentWins = -1;
            int currentHigh = -1;
            for (int i = 0; i < GC.m_players.Length; i++)
            {
                if (GC.m_players[i] != null)
                {
                    int hold = GC.m_players[i].GetComponent<ParkingGameModeTracker>().getWins();
                    if (hold > currentWins)
                    {
                        currentWins = hold;
                        currentHigh = i;
                    }
                }
            }
            if(currentHigh != -1)
            {
                HF.PlayerExp.AddEXP(currentHigh,300);
            }
        }



        //Clean up and return to menu

        void endParking()
        {
            m_active = false;
            //Force control back on at end 
            rewardWinner();
			setControl (true);
			cleanUpGameMode();
			EndGame ();
            
        }


        /// <summary>
        /// sets up a new round for the players, can randomly select from the current stages
        /// on the list or a spesific stage, requesting random with a requested stage will be
        /// overruled
        /// </summary>
        void beginRound(bool _randomStage, int _requestedStage = 0)
        {
                        
            currentStartLocs = new Transform[4];

            if (_randomStage)
            {
                int currentStage = 0;
                do
                {
                    currentStage = Random.Range(0, collectionOfStartPoints.Length);
                }
                while (usedLevelList.Contains(currentStage));
                usedLevelList.Add(currentStage);
                getCurrentStartLocs(currentStage);
                getCurrentEndLoc(currentStage);
            }
            else
            {
                getCurrentStartLocs(_requestedStage);
                getCurrentEndLoc(_requestedStage);
            }




        }


        void getCurrentEndLoc(int _stageNo)
        {
            endPoint.transform.position = collectionOfEndPoints[_stageNo].transform.position;
        }


        /// <summary>
        /// Gets the current children of the currently selected start locations and placed them
        /// were the cars can acess them
        /// </summary>
        void getCurrentStartLocs(int _stageNo)
        {
            int i = 0;
            foreach (Transform child in collectionOfStartPoints[_stageNo])
            {
                currentStartLocs[i] = child;
                i++;
            }
        }

        static public int s_nMaxRounds = 3;

        void RestartRound(bool _randomStage, int _requestedStage = 0)
        {
            int currentStage = 0;
            currentRound++;

            if (_randomStage)
            {
                do
                {
                    currentStage = Random.Range(0, collectionOfStartPoints.Length);
                }
                while (usedLevelList.Contains(currentStage));
                usedLevelList.Add(currentStage);
                getCurrentStartLocs(currentStage);
                getCurrentEndLoc(currentStage);
            }
            else
            {
                getCurrentStartLocs(_requestedStage);
                getCurrentEndLoc(_requestedStage);
            }            
                moveToStart(currentStage);
            
        }


    }
}