//Author:       TMS
//Description:  Script that controls how a car behaves. 
//              Acts as a "Player Controller" for the car.
//Last edit:    TMS @ 16/01/2017

using UnityEngine;
using System.Collections;
using System;
using Rewired;

namespace Kojima
{
    //[RequireComponent(typeof(CarResetter))]
    public class CarScript : MonoBehaviour
    {
        Bam.CarSoundScript m_mySoundScript;
        Bam.CarSockets m_mySockets;
        Bam.SurfaceParticleScript m_surfParticles;

        // Experience System
        HF.PlayerExp m_exp;
        public HF.PlayerExp PlayerEXP {
            get {
                if (m_exp == null) {
                    m_exp = GetComponent<HF.PlayerExp>();

                    if (m_exp == null) {
                        m_exp = gameObject.AddComponent<HF.PlayerExp>();
                    }
                }

                return m_exp;
            }
        }

        public Bird.HUDController m_PlayerHUD = null;
        public Bird.WorldHUD m_WorldUI = null;

        string stringForGizmoDebug;

        [System.Serializable]
        public struct CarInfo_s
        {
            public enum driveMode_e { rearWheels, frontWheels, allWheels };
            public driveMode_e m_myDriveMode;

            [Range(0, 100)]
            public float m_health;

            [Range(4, 10)]
            public float m_acceleration;
            [Range(10, 150)]
            public float m_maxSpeed;

            [Range(0.25f, 6)]
            public float m_turnMaxSpeed;
            [Range(0, 0.5f)]
            public float m_extraGrip;

            [Range(0.35f, 0.5f)]
            public float m_wheelSize;

            [Range(7.5f, 10.0f)]
            public float m_cameraDistance;

            public CarSoundPack mySoundPack;

            public bool m_airControl;

            public CarInfo_s(float _health, float _maxSpeed, float _acceleration, float _turnSpeed, float _wheelSize, float _grip, driveMode_e _driveMode, CarSoundPack _soundPack, float _cameraDistance)
            {
                m_myDriveMode = _driveMode;
                m_health = _health;

                m_acceleration = _acceleration;
                m_maxSpeed = _maxSpeed;

                m_turnMaxSpeed = _turnSpeed;
                m_wheelSize = _wheelSize;

                m_extraGrip = _grip;

                mySoundPack = _soundPack;

                m_airControl = true;
                m_cameraDistance = _cameraDistance;
            }
        }

        public struct Wheel_s
        {
            public bool m_grounded, m_skidding;
            public float m_curSpeed;
            //public float m_steepValue;
        }

        public static bool s_playersCanMove = true;
        private bool m_canIMove = true;
        public bool CanMove
        {
            get { return m_canIMove; }
            set { m_canIMove = value; }
        }

        bool m_changedGCCount = false;

        /// <summary>
        /// One based index
        /// </summary>
        public int m_nplayerIndex = 0;
        public static float m_waterYPosition = -2.5f;
        bool m_inWater = false;
        public bool CurrentlyInWater
        {
            get { return m_inWater; }
        }

        //[HideInInspector]
        public CarInfo_s m_baseCarInfo;
        CarInfo_s m_curStats, m_surfaceStats;
        public CarInfo_s m_boostStats;
        float boostTimer = 0;

        public bool CurrentlyBoosting
        {
            get { return boostTimer > 0; }
        }

        public Transform m_carBody;

        [Tooltip("BL, BR, FL, FR")]
        public Transform[] m_wheels;
        public Wheel_s[] m_wheelInfos;
        public GameObject m_wheelLandParticlePrefab;
        ParticleSystem[] m_wheelLandParticles;
        //public bool[] m_wheelIsGrounded, m_wheelIsSkidding;
        float skiddingRight = 0;

        //POWERUPS
        [SerializeField]
        bool m_climbAllWalls;

        bool m_gliderActive = false;
        [SerializeField]
        float m_gliderBuiltupSpeed = 0;

        /// <summary>
        /// The grapple on me.
        /// </summary>
        public GameObject grappleOnMe;
        public GameObject myGrapple;
        bool currentlyFiringGrapple = false;

        public Transform[] GetAllWheels
        {
            get { return m_wheels; }
        }

        public bool IsMoving
        {
            get { return Mathf.Abs(m_normalisedForwardVelocity) > 0.075f; }
        }

        bool m_currentlyBraking;

        /// <summary>
        /// Returns true if the car is braking but not reversing yet
        /// </summary>
        public bool CurrentlyBraking
        {
            get { return m_currentlyBraking; }
        }

        bool m_currentlyReversing;

        /// <summary>
        /// Returns true if the car is reversing
        /// </summary>
        public bool CurrentlyReversing
        {
            get { return m_currentlyReversing; }
        }

        /// <summary>
        /// Returns true if all the wheels are grounded
        /// </summary>
        public bool AllWheelsGrounded
        {
            get
            {
                int groundedWheels = 0;

                for (int i = 0; i < m_wheelInfos.Length; i++)
                {
                    if (m_wheelInfos[i].m_grounded)
                    {
                        groundedWheels++;
                    }
                }

                return groundedWheels == m_wheelInfos.Length;
            }
        }
        //public bool AllWheelsGrounded
        //{
        //    //@Assumes 4 wheels, can be improved if other vehicles are required.
        //    get { return (m_wheelIsGrounded[0] && m_wheelIsGrounded[1] && m_wheelIsGrounded[2] && m_wheelIsGrounded[3]); }
        //}
        ///// <summary>
        ///// Returns true if all the wheels are NOT grounded
        ///// </summary>
        public bool InMidAir
        {
            get
            {
                int groundedWheels = 0;

                for (int i = 0; i < m_wheelInfos.Length; i++)
                {
                    if (m_wheelInfos[i].m_grounded)
                    {
                        groundedWheels++;
                    }
                }

                return groundedWheels == 0;
            }
        }

        public CarSoundPack GetSoundPack
        {
            get { return m_baseCarInfo.mySoundPack; }
        }


        public bool CurrentlyGliding
        {
            get
            {
                return m_gliderActive && InMidAir;
            }
        }

        //public bool InMidAir
        //{
        //    //@Assumes 4 wheels, can be improved if other vehicles are required.
        //    get { return (!m_wheelIsGrounded[0] && !m_wheelIsGrounded[1] && !m_wheelIsGrounded[2] && !m_wheelIsGrounded[3]); }
        //}
        //private bool m_inWater = false;
        //public bool InWater
        //{
        //    get { return m_inWater; }
        //}



        RaycastHit[] m_wheelRaycasts;
        public float m_fwheelTorque;

        public RaycastHit[] GetWheelRaycasts
        {
            get { return m_wheelRaycasts; }
        }

        public float m_forwardVelocity = 0, m_normalisedForwardVelocity = 0;

        Vector3[] m_wheelLocalPositions;

        //bool m_currentlySkidding = false;
        //public bool IsSkidding { get { return m_currentlySkidding; } }
        float m_fcurSkidIntensity = 0;

        Vector3 m_skidDirection;

        float m_curWheelSpin = 0;

        Rigidbody m_rb;

        Vector3 m_bodyVelocity, m_bodyAngularVelocity;

        float m_fflipTimer = 0;
        float m_respawnTimer = 0;

        float m_respawnCoolDown = 3;
        float m_currentRespawnCooldown = 0;
        float m_respawnCounter = 0;

        public float GetRespawnCounter
        {
            get { return m_respawnCounter; }
        }

        public bool UpsideDown
        {
            get { return m_fflipTimer > 0; }
        }

        //Don't worry about these
        [HideInInspector]
        public string m_strplayerInputTag;

        [SerializeField]
        bool m_handBrake = false;
        float m_handBrakeTorque = 0;
        public bool CurrentlyHandbraking { get { return m_handBrake; } }
        Vector3 m_driftVelo;

        float m_currentSpeedMultiplier = 1;
        float m_prevAcceleratorInput = 0;

        public float m_ftargetAngularDrag = 0.015f;
        float m_fcancelHoriForce = 0, m_fcancelHoriForceTarget = 5;

        private CarResetter mRef_carResetter;
        private CapsuleCollider mRef_collider;
        public CapsuleCollider m_carCollider { get { return mRef_collider; } }

        private Bam.CarSoundScript m_soundScript;
        private Bam.PlayerCameraScript m_myCamera;

        public Bam.PlayerCameraScript GetCam
        {
            get { return m_myCamera; }
        }

        private Rewired.Player m_rewiredPlayer;
        bool droppingOut = false;

        private Bam.CarSuspensionScript m_susScript;

        Kojima.RespawnScript myRespawnScript;
        static RespawnManager respawnManager;

        //Particle Systems
        public GameObject splashPrefab;

        //For Sound
        float acceleratorInput = 0;
        public float GetAcceleratorInput
        {
            get { return acceleratorInput; }
        }

        public static bool AreAllCarsAvailable
        {
            get
            {
                for(int i=0;i<GameController.s_ncurrentPlayers; i++)
                {
                    if(GameController.s_singleton.m_players[i].CurrentlyInWater)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        // Use this for initialization
        void Awake()
        {
            if(m_nplayerIndex==0)
            {
                AutoCorrectPlayerIndex();
            }

            //m_strplayerInputTag = "_P" + m_nplayerIndex;

            m_surfParticles = gameObject.AddComponent<Bam.SurfaceParticleScript>();
            m_wheelLocalPositions = new Vector3[m_wheels.Length];
            m_wheelInfos = new Wheel_s[4];
            //m_wheelIsGrounded = new bool[m_wheels.Length];
            //m_wheelIsSkidding = new bool[m_wheels.Length];
            m_wheelRaycasts = new RaycastHit[m_wheels.Length];

            for (int i = 0; i < m_wheelLocalPositions.Length; i++)
            {
                //Automatically makes all wheels equal in terms of Y pos
                m_wheels[i].transform.localPosition = new Vector3(m_wheels[i].transform.localPosition.x, m_wheels[0].transform.localPosition.y, m_wheels[i].transform.localPosition.z);
                m_wheelLocalPositions[i] = m_wheels[i].transform.localPosition;
            }

            m_rb = GetComponent<Rigidbody>();

            m_bodyVelocity = Vector3.zero;
            m_bodyAngularVelocity = Vector3.zero;

            if (m_nplayerIndex != 0)
            {
                GameController.s_ncurrentPlayers++;
                GameController.s_singleton.m_players[m_nplayerIndex - 1] = this;
                m_changedGCCount = true;
                //Debug.Log("Current players: " + GameController.s_ncurrentPlayers);
            }
            else
            {
                Destroy(gameObject);
            }

            //Cache a reference to the CarRestter script
            mRef_carResetter = GetComponent<CarResetter>();
            mRef_collider = GetComponent<CapsuleCollider>();

            //Cache a reference to the CarSound script
            m_soundScript = GetComponent<Bam.CarSoundScript>();

            //Cache a reference to the script holding my sockets
            m_mySockets = GetComponent<Bam.CarSockets>();


            //Create wheel land effects
            m_wheelLandParticles = new ParticleSystem[m_wheels.Length];
            for (int i = 0; i < m_wheels.Length; i++)
            {
                m_wheelLandParticles[i] = Instantiate<GameObject>(m_wheelLandParticlePrefab).GetComponent<ParticleSystem>();
                m_wheelLandParticles[i].transform.SetParent(m_wheels[i]);
                m_wheelLandParticles[i].transform.localPosition = Vector3.zero;
                m_wheelLandParticles[i].transform.localEulerAngles = new Vector3(-90, 0, 0);
                m_wheelLandParticles[i].transform.localScale = Vector3.one;
            }

            m_mySoundScript = GetComponent<Bam.CarSoundScript>();

            if (m_nplayerIndex > 0)
            {
                SetNewPlayerIndex(m_nplayerIndex);
            }
        }

        void AutoCorrectPlayerIndex()
        {
            for (int i = 0; i < 4; i++)
            {
                if (GameController.s_singleton.m_players[i] == null)
                {
                    SetNewPlayerIndex(i + 1);
                    Debug.LogWarning("Player index auto-corrected to " + m_nplayerIndex + " on " + gameObject.name + "!");
                    break;
                }
            }

            if(m_nplayerIndex==0)
            {
                Debug.LogWarning("" + gameObject.name + " hasn't been auto-assigned a player number!");
            }
        }

		// Attempting to decouple controller ID and player ID so that we can get dropin/out working (fairly low-priority so close to shipdate, though)
		public int m_nControllerID;
        void Start()
        {
			//ApplyCarInfo(new CarInfo_s(100, 15, 10, 10, 0.35f, CarInfo_s.driveMode_e.allWheels, "Engine1"));
			//m_rewiredPlayer = ReInput.players.GetPlayer(m_nplayerIndex - 1);
			m_rewiredPlayer = ReInput.players.GetPlayer(m_nControllerID);


			//Create suspension stuff
			m_susScript = gameObject.AddComponent<Bam.CarSuspensionScript>();
            m_susScript.Initialise(m_carBody, 1, this);

            //PullOutGlider();
            if(!respawnManager)
            {
                respawnManager = FindObjectOfType<RespawnManager>();
            }

            myRespawnScript = gameObject.AddComponent<RespawnScript>();
            myRespawnScript.respawnManager = respawnManager;

            ResetCar();

            if (m_WorldUI == null)
            {
                if (Bird.HUDSpawner.s_WorldHUDs[m_nplayerIndex - 1])
                {
                    m_WorldUI = Bird.HUDSpawner.s_WorldHUDs[m_nplayerIndex - 1].GetComponent<Bird.WorldHUD>();
                    m_WorldUI.AttachToCar(this);
                }
            }
		}

        public void SetNewPlayerIndex(int newIndex)
        {
            //Debug.Log(gameObject.name + " is now player " + newIndex);
            m_nplayerIndex = newIndex;
            m_rewiredPlayer = ReInput.players.GetPlayer(m_nplayerIndex - 1);
        }

        public Rewired.Player GetRewiredPlayer()
        {
            return m_rewiredPlayer;
        }

        public void GiveCamera(Bam.PlayerCameraScript cam)
        {
            if(m_myCamera!=cam)
            {
                cam.transform.position = transform.position;
            }

            m_myCamera = cam;
        }

        public void ClearCamera()
        {
            m_myCamera = null;
        }

        public GameObject GetCarBody()
        {
            return m_carBody.gameObject;
        }

        public void RemoveFromScene()
        {
            if (m_nplayerIndex > 0)
            {
              //  Debug.Log("Removed player " + m_nplayerIndex + "'s car from the scene.");
                GameController.s_singleton.m_players[m_nplayerIndex - 1] = null;
            }

            if(m_changedGCCount)
            {
                GameController.s_ncurrentPlayers--;
                m_changedGCCount = false;
            }

			// Before we kill ourselves, detach our world hud
			if (m_WorldUI) {
				m_WorldUI.DetachFromCar(this);
			}

            Destroy(gameObject);
        }

        void AddWheelSpin(float amount = 0.25f)
        {
            m_curWheelSpin += amount;

            if(m_curWheelSpin>1)
            {
                m_curWheelSpin = 1;
            }
        }

        void OnDestroy()
        {
            if (!droppingOut && m_changedGCCount)
            {
                GameController.s_ncurrentPlayers--;
                Debug.Log("Current players: " + GameController.s_ncurrentPlayers);
                m_changedGCCount = false;
            }
        }

        public void ApplyCarInfo(CarInfo_s newInfo)
        {
            m_baseCarInfo = newInfo;
            //m_soundScript.SetSounds(m_baseCarInfo.m_engineAudioClip, m_baseCarInfo.m_accelerationAudioClip);
        }

        public Vector3 GetVelocity()
        {
            return m_rb.velocity;
        }

        /// <summary>
        /// This is what to call when you want to tell the car to behave differently on its current surface
        /// </summary>
        /// <param name="surfaceStats"></param>
        public void ApplyNewSurfaceStats(ref CarInfo_s surfaceStats)
        {
            m_surfaceStats = surfaceStats;
        }


        ///// <summary>
        ///// Call to check whether the car is in water (expensive, so cache the result)
        ///// @Could be improved as right now it only checks below wheels (always DOWN)
        ///// and above the car on global axis (UP). Using a trigger collider to check this
        ///// would be easier!
        ///// </summary>
        ///// <returns></returns>
        //private bool CheckInWater()
        //{
        //    //Check below wheels
        //    RaycastHit hit;
        //    Ray ray = new Ray();
        //    foreach (Transform wheel in wheels)
        //    {
        //        ray.origin = wheel.position;
        //        ray.direction = Vector3.down;
        //        if (Physics.Raycast(ray, out hit, myInfo.wheelSize))
        //        {
        //            if (hit.collider.gameObject.tag == "Water")
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    //Check above car - probably won't ever happen...
        //    ray.origin = transform.position;
        //    ray.direction = Vector3.up;
        //    if (Physics.Raycast(ray, out hit, mRef_collider.radius, LayerMask.NameToLayer("Player")))
        //    {
        //        if (hit.collider.gameObject.tag == "Water")
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Water")
            {
                EnterWater();
            }
        }

        public void EnterWater()
        {
            if (m_inWater == false)
            {
                m_inWater = true;
                m_respawnTimer = 4;

                SetCanMove(false);

                MakeWheelStopSkidding(0);
                MakeWheelStopSkidding(1);
                MakeWheelStopSkidding(2);
                MakeWheelStopSkidding(3);

                GameObject splashEffectInstance = Instantiate<GameObject>(splashPrefab);
                splashEffectInstance.transform.position = transform.position;
                splashEffectInstance.transform.rotation = Quaternion.LookRotation(Vector3.up);
                Destroy(splashEffectInstance, 4);

                if (m_myCamera)
                {
                    m_myCamera.ResetSpd();
                }
                //m_myCamera.SwitchViewStyle(Bam.PlayerCameraScript.viewStyles_e.caravan);
                //transform.position = mRef_carResetter.GetLastSafePosition();
                //mRef_CarResetter.ResetRecord();
                //mRef_carResetter.ForceRecord();

                //m_rb.velocity = Vector3.zero;
                //m_rb.angularVelocity = Vector3.zero;
                //Vector3 euler = transform.eulerAngles;
                //euler.x = 0;
                //euler.z = 0;
                //transform.rotation = Quaternion.Euler(euler);
            }
        }

        private void OnTriggerExit(Collider other)
        {

        }

        void AddSpeedToWheel(int wheelIndex, float speed)
        {
            m_wheelInfos[wheelIndex].m_curSpeed += speed * Time.deltaTime;

            m_wheelInfos[wheelIndex].m_curSpeed = Mathf.Clamp(m_wheelInfos[wheelIndex].m_curSpeed, -GetMaxSpeed() * 0.65f, GetMaxSpeed());
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //Resets the string
            stringForGizmoDebug = "No debug info right now";

            if (CurrentlyInWater) 
			{
				//If car is stuck in the water then override everything here
				ApplyWater();
				return;
			}
            else
            {
                if (transform.position.y < m_waterYPosition)
                {
                    EnterWater();
                }
            }

            //Basic Input
            acceleratorInput = m_rewiredPlayer.GetAxis("Accelerator") - m_rewiredPlayer.GetAxis("Brake");

            if ((m_prevAcceleratorInput >= 0 && acceleratorInput < 0) || (m_prevAcceleratorInput <= 0 && acceleratorInput > 0))
            {
                m_soundScript.PlayOneShot(m_baseCarInfo.mySoundPack.pedalSound, 0.3f);

                if (acceleratorInput > 0.9f)
                {
                    m_soundScript.SetGear(0);
                }
            }

            //Resetting various bools
            m_currentlyBraking = false;
            m_currentlyReversing = false;

            //Check for reversing
            if (m_normalisedForwardVelocity < 0 && acceleratorInput < 0)
            {
                m_currentlyReversing = true;
            }

            //Handles wheel speed
            for(int i=0; i<4; i++)
            {
                if (!IsWheelGrounded(i) || IsWheelSkidding(i))
                {
                    AddSpeedToWheel(i, acceleratorInput * GetMaxSpeed());
                }
                else
                {
                    m_wheelInfos[i].m_curSpeed = m_forwardVelocity;

                    //if(IsWheelSkidding(i))
                    //{
                    //    m_wheelInfos[i].m_curSpeed = 0;
                    //}
                }
            }

            ManageDrag();
            Movement();

            m_curWheelSpin = Mathf.InverseLerp(m_curWheelSpin, 0, 1 * Time.deltaTime);

            if (InMidAir)
            {
                m_fcurSkidIntensity = 0;

                if (m_baseCarInfo.m_airControl && Mathf.Abs(m_rb.velocity.y) > 1)
                {
                    AirControl();
                }
            }

            MaintainMaxSpeed();

            //Apply some manual effects to suspension
            m_susScript.ApplySteerForce(transform.InverseTransformDirection(GetVelocity()).x * -8);

            m_prevAcceleratorInput = acceleratorInput;
        }

        void ManageDrag()
        {
            //Manage drag
            float targetDrag = 0.0001f;
            m_rb.drag = 0.0f;

            for (int i = 0; i < m_wheels.Length; i++)
            {
                if (m_wheelInfos[i].m_grounded)
                {
                    m_rb.drag += targetDrag * 0.25f * GetGrip();
                }
            }

			//Overrides this when a grapple is in charge
			if (grappleOnMe)
			{
				m_rb.drag = 0.45f;
			}
        }

        void Update()
        {          
            HandleBoostStats();

            if (m_rewiredPlayer.GetButtonDown("Drift"))
            {
                //CarInfo_s s = new CarInfo_s();
                //s.m_extraGrip = -0.95f;

                //ApplyNewSurfaceStats(s);
            }

            if(m_rewiredPlayer.GetButtonDown("Honk"))
            {
                m_mySoundScript.HonkHorn();
                //SetCanMove(!CanMove);
            }

            if(Input.GetKeyDown(KeyCode.R))
            {
                ResetCar();
            }

            if (Input.GetKeyDown(KeyCode.Backslash) && Input.GetKey(KeyCode.DownArrow) && m_nplayerIndex==1)
            {
                //Bam.BamResultsScript.ShowResults(null, "Test game").GiveScores(new int[3] { 200, 13, 21 });
                //Instantiate(CarSwapManager.m_sInstance.m_carPrefab[3], transform.position + Vector3.up * 2, transform.rotation);
                //CameraManagerScript.singleton.SetupThirdPersonForAllPlayers();
                UnityEngine.SceneManagement.SceneManager.LoadScene("SmallIslandLobby", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            }
            if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.DownArrow) && m_nplayerIndex == 1)
            {
                //Bam.BamResultsScript.ShowResults(null, "Test game").GiveScores(new int[3] { 200, 13, 21 });
                //Instantiate(CarSwapManager.m_sInstance.m_carPrefab[3], transform.position + Vector3.up * 2, transform.rotation);
                //CameraManagerScript.singleton.SetupThirdPersonForAllPlayers();
                //Destroy(Kojima.GameController.s_singleton.m_players[Kojima.GameController.s_ncurrentPlayers - 1].gameObject);
                Kojima.GameController.s_singleton.m_players[Kojima.GameController.s_ncurrentPlayers - 1].DropOut();
            }

            if (Input.GetKeyDown(KeyCode.Delete) && gameObject.name== "MonteCarlo")
            {
                DropOut();
            }

            if(m_currentRespawnCooldown> 0)
            {
                m_currentRespawnCooldown -= Time.deltaTime;
            }
            else if (m_rewiredPlayer.GetButton("Respawn") && CanMove)
            {
                if (m_respawnCounter < 1)
                {
                    m_respawnCounter += Time.deltaTime * 0.25f;
                    //Debug.Log(m_respawnCounter);
                }
                else
                {
                    m_respawnTimer = 1.2f;
                    //myRespawnScript.moveToCurrentReset();
                    m_respawnCounter = 0;
                    m_currentRespawnCooldown = 2;
                    print("PLAYER " + m_nplayerIndex + " HAS MANUALLY RESPAWNED");
                }

                //SetCanMove(!CanMove);
            }
            else if(m_respawnCounter>0)
            {
                m_respawnCounter -= Time.deltaTime;
            }

            if (m_respawnTimer > 0)
            {
                m_respawnTimer -= Time.deltaTime;

                if (GetComponent<GrappleLaunchManager>())
                {
                    GetComponent<GrappleLaunchManager>().getGrappleLauncher().GetComponent<GrappleLaunch>().killGrapple();
                }

                if (m_respawnTimer <= 1)
                {
                    if (m_myCamera)
                    {
                        m_myCamera.myTransitionScript.BeginTransition();
                    }
                }

                if(m_respawnTimer<=0)
                {
                    m_inWater = false;
                    SetCanMove(true);
                    myRespawnScript.moveToCurrentReset();

                    if (m_myCamera)
                    {
                        m_myCamera.SwitchViewStyle(Bam.PlayerCameraScript.viewStyles_e.thirdPerson);
                    }

                    ResetCar();
                }
            }
            else
            {

            }


            //if (Input.GetKeyDown(KeyCode.G))
            //{
            //    if(m_gliderActive)
            //    {
            //        PutAwayGlider();
            //    }
            //    else
            //    {
            //        PullOutGlider();
            //    }
            //}
        }

        public void DisplayLocationName(string locationName)
        {
			//if (m_myCamera)
   //         {
   //             m_myCamera.DisplayLocationName(locationName);
   //         }

			Bird.HUD_Area.hudAreaData_t dataobj = new Bird.HUD_Area.hudAreaData_t();
			dataobj.m_strAreaName = locationName;
			dataobj.m_nTargetPlayerID = m_nplayerIndex;

			EventManager.m_instance.AddEvent(Events.Event.UI_HUD_SHOW_AREANAME, dataobj);

		}

        public float GetSkidIntensity()
        {
            return m_fcurSkidIntensity;
        }

		public void SetCanMove(bool newCM)
		{
			m_canIMove = newCM;
		}

        public void Explode()
        {
            SetCanMove(false);
            m_respawnTimer = 5;

            GameObject explosion = Instantiate<GameObject>(ParticleBank.singleton.explosion1);
            explosion.transform.SetParent(transform);
            explosion.transform.localPosition = Vector3.zero;
            explosion.transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Tells the car that it has fired a grapple out
        /// </summary>
        public void FireGrapple()
        {
            currentlyFiringGrapple = true;
            //myGrapple = 
        }

        public void PutAwayGrapple()
        {
            currentlyFiringGrapple = false;
        }

		//Removes control from the player
		public void HitByGrapple(GameObject grapple)
		{
			SetCanMove (false);
			grappleOnMe = grapple;
		}

		//Gives player back
		public void ReleasedByGrapple()
		{
			SetCanMove (true);
			grappleOnMe = null;
		}


        void AirControl()
        {
            if (m_gliderActive)
            {
                Vector3 controlVelocity = new Vector3(m_rewiredPlayer.GetAxisRaw("Move Vertical") * 2, m_rewiredPlayer.GetAxisRaw("Move Horizontal"), -m_rewiredPlayer.GetAxisRaw("Move Horizontal") * 1.59285f);
                GliderControl(controlVelocity);
            }
            else
            {
                //Vector3 controlVelocity = new Vector3(-Input.GetAxisRaw("Vertical" + m_strplayerInputTag), Input.GetAxisRaw("Horizontal" + m_strplayerInputTag), 0);
                //Rewired
                Vector3 controlVelocity = new Vector3(m_rewiredPlayer.GetAxisRaw("Move Vertical"), m_rewiredPlayer.GetAxisRaw("Move Horizontal"), 0);
                controlVelocity = transform.TransformDirection(controlVelocity);
                m_rb.AddTorque(controlVelocity * 3, ForceMode.Acceleration);
            }
        }

        public void PullOutGlider()
        {
            m_gliderActive = true;
        }

        public void PutAwayGlider()
        {
            m_gliderActive = false;
        }

        void GliderControl(Vector3 input)
        {
            float glideTurnSpd = 1;
            float glideFowardSpd = 1.025f;

            Vector3 localVelo = transform.InverseTransformDirection(GetVelocity());
            localVelo.x = Mathf.Lerp(localVelo.x, 0, 0.725f * Time.deltaTime);
            localVelo.y = Mathf.Lerp(localVelo.y, transform.forward.y * localVelo.y, 5 * Time.deltaTime);

            localVelo.z -= (GetVelocity().y) * 1 * Time.deltaTime;

            localVelo.z += m_gliderBuiltupSpeed * Time.deltaTime;
            m_gliderBuiltupSpeed -= Time.deltaTime;

            localVelo.z = Mathf.Clamp(localVelo.z, -10, 70 + m_gliderBuiltupSpeed);
            m_rb.velocity = transform.TransformDirection(localVelo);

            m_gliderBuiltupSpeed += -GetVelocity().y * 0.05f;

            m_gliderBuiltupSpeed = Mathf.Clamp(m_gliderBuiltupSpeed, 0, 60);

            stringForGizmoDebug += "\n Glider built up speed: " + m_gliderBuiltupSpeed;

            input = transform.TransformDirection(input);
            m_rb.AddTorque(input * glideTurnSpd, ForceMode.Acceleration);

            float upwardsPower = transform.up.y;

            m_rb.AddForce(transform.up * 10 * upwardsPower, ForceMode.Acceleration);

            m_rb.angularDrag = 0.81f;
            m_rb.drag = 0.05f;

            Vector3 dir = transform.forward;
            dir.y = 0;
            dir = dir.normalized;
            Debug.DrawLine(transform.position, transform.position + dir * 3, Color.cyan);
            Quaternion standardRot = Quaternion.LookRotation(dir, Vector3.up);
            m_rb.rotation = Quaternion.Lerp(m_rb.rotation, standardRot, 4 * Time.deltaTime);
        }

        public void ApplyHandbrake()
        {
            Debug.DrawLine(transform.position, transform.position + transform.right * skiddingRight);
            m_rb.AddForce(transform.right * skiddingRight * 15 * m_normalisedForwardVelocity * m_rb.angularVelocity.y, ForceMode.Acceleration);

            //Don't let the car just drive and handbrake
            if(m_rb.angularVelocity.magnitude<2)
            {
                m_rb.velocity = Vector3.Lerp(m_rb.velocity, Vector3.zero, 1 * Time.deltaTime);
            }

        }

        void HandleBoostStats()
        {
            float resetSpeed = 1;

            if (boostTimer <= 0)
            {
                m_boostStats.m_acceleration = Mathf.Lerp(m_boostStats.m_acceleration, 0, resetSpeed * Time.deltaTime);
                m_boostStats.m_maxSpeed = Mathf.Lerp(m_boostStats.m_maxSpeed, 0, resetSpeed * Time.deltaTime);
            }
            else
            {
                boostTimer -= Time.deltaTime;
            }
        }

        void OnDrawGizmos()
        {
#if UNITY_EDITOR
			if (m_nplayerIndex == 1)
            {
                UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, "Normalised Speed: " + m_normalisedForwardVelocity);
                UnityEditor.Handles.Label(transform.position+Vector3.up, stringForGizmoDebug);
                //UnityEditor.Handles.Label(transform.position, "CancelHori: " + m_fcancelHoriForce);

                //for (int i=0; i<4; i++)
                //{
                //    if(m_wheelIsSkidding[i])
                //    {
                //        Gizmos.DrawSphere(m_wheels[i].transform.position, 0.425f);
                //    }
                //}
            }
#endif
		}

        void MakeWheelSkid(int wheelIndex)
        {
            float skidTransitionSpeed = 6.0f;

            m_wheelInfos[wheelIndex].m_skidding = true;

            m_rb.drag += 0.07f;
            m_rb.angularDrag = 0.0f;
            m_fcancelHoriForce = Mathf.Lerp(m_fcancelHoriForce, 0, skidTransitionSpeed * Time.deltaTime);

            m_fcurSkidIntensity = Mathf.Abs(GetVelocity().magnitude);
            //Debug.Log(m_fcurSkidIntensity);
        }

        public bool IsWheelGrounded(int index)
        {
            return m_wheelInfos[index].m_grounded;
        }

        void ApplyWater()
        {
            //m_rb.velocity = Vector3.Lerp(m_rb.velocity, -Vector3.up, 5 * Time.deltaTime);
            float targetY = m_waterYPosition + Mathf.Sin(Time.timeSinceLevelLoad * 3);

            m_rb.velocity = Vector3.Lerp(m_rb.velocity, Vector3.zero, 2.5f * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetY, transform.position.z), 3 * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-Vector3.up), 1 * Time.deltaTime);

            m_rb.drag = 0.15f;
            m_rb.angularDrag = 0.1f;
            m_fcurSkidIntensity = 0;
        }

        public void AllowWallClimbing()
        {
            m_climbAllWalls = true;
        }

        public void DisableWallClimbing()
        {
            m_climbAllWalls = false;
        }

        void Movement()
        {
            float currentVelocity = m_rb.velocity.magnitude;
            m_forwardVelocity = (currentVelocity * Vector3.Dot(m_rb.velocity.normalized, transform.forward));
            m_normalisedForwardVelocity = m_forwardVelocity / GetMaxSpeed();

            m_rb.angularDrag = 0.001f;

            //if (s_playersCanMove && m_canIMove)
            {
                //Drifting
                bool driftBtnPress = m_rewiredPlayer.GetButton("Drift");
                m_handBrake = driftBtnPress && !InMidAir;

                if (m_rewiredPlayer.GetButtonDown("Drift"))
                {
                    m_soundScript.PlayOneShot(m_baseCarInfo.mySoundPack.pedalSound, 0.3f);
                }

                //Give each wheel a chance to push the car if grounded
                for (int i = 0; i < m_wheels.Length; i++)
                {
                    m_wheelInfos[i].m_grounded = PerformWheelRaycast(m_wheels[i], i);
                    bool thisWheelCanDrive = false;

                    switch (m_baseCarInfo.m_myDriveMode)
                    {
                        case CarInfo_s.driveMode_e.allWheels:
                            thisWheelCanDrive = true;
                            break;
                        case CarInfo_s.driveMode_e.rearWheels:
                            if (i <= 1)
                            {
                                thisWheelCanDrive = true;
                            }
                            break;
                        case CarInfo_s.driveMode_e.frontWheels:
                            if (i > 1)
                            {
                                thisWheelCanDrive = true;
                            }
                            break;
                    }

                    if (!IsWheelGrounded(i))
                    {
                        thisWheelCanDrive = false;
                        m_handBrake = false;
                    }
                    else
                    {
                        ////Check if surface is too steep
                        //float steepnessMax = 0.22f;

                        //if (m_wheelInfos[i].m_steepValue > steepnessMax && !m_climbAllWalls)
                        //{
                        //    thisWheelCanDrive = false;
                        //    stringForGizmoDebug = "Floor too steep here!! Steepness value: " + m_wheelInfos[i].m_steepValue;
                        //    m_rb.AddForce(m_wheelRaycasts[i].normal * 2, ForceMode.Acceleration);
                        //}
                    }

                    if (IsWheelGrounded(i))
                    {
                        PreventSkidding(i);

                        //if(CurrentlyHandbraking)
                        //{
                        //    Vector3 forwardForce = transform.InverseTransformDirection(m_rb.velocity);
                        //    forwardForce.x = 0;
                        //    forwardForce.y = 0;

                        //    forwardForce = transform.TransformDirection(forwardForce);

                        //    m_rb.AddForce(-forwardForce * 0.25f, ForceMode.Acceleration);
                        //}
                    }
                    else
                    {
                        //Adds some angular drag to prevent really quick spinning in mid air
                        m_rb.angularDrag += 0.2f;
                    }

                    //Rewired
                    //bool moving = m_rewiredPlayer.GetAxis("Accelerator") != 0 || m_rewiredPlayer.GetAxis("Brake") != 0;

                    //Steering
                    float steerAmount = 0, handbrakeTurnAmount = 0;
                    float steerControlMultiplier = 1;

                    ////Rotating while handbraking
                    if (CurrentlyHandbraking)
                    {
                        steerControlMultiplier = 0.975f;
                        //Steering(m_handBrakeTorque * 0.5f, i, out handbrakeTurnAmount);
                    }

                    //Proper steering
                    if (s_playersCanMove && m_canIMove)
                    {
                        Steering(m_rewiredPlayer.GetAxisRaw("Move Horizontal") * steerControlMultiplier, i, out steerAmount);
                    }

                    //Rotates the car to fit the terrain
                    if (IsWheelGrounded(i) && m_wheelRaycasts[0].normal.y > 0.4f)
                    {
                        m_rb.rotation = Quaternion.RotateTowards(m_rb.rotation, Quaternion.LookRotation(GetWheelDirection(i, 1), m_wheelRaycasts[i].normal), 3 * Time.deltaTime);
                    }

                    if (thisWheelCanDrive)
                    {
                        //if (moving)
                        {
                            float accelerationInput = (m_rewiredPlayer.GetAxis("Accelerator"));
                            float brakeInput = (-m_rewiredPlayer.GetAxis("Brake"));

                            float forwardMultiplier = accelerationInput + brakeInput;

                            Vector3 wheelDirection = GetWheelDirection(i, forwardMultiplier);
                            Debug.DrawLine(m_wheels[i].transform.position, m_wheels[i].transform.position + wheelDirection * 5, Color.green);

                            Vector3 accelerationForce = wheelDirection * GetAcceleration() * accelerationInput;
                            Vector3 brakeForce = wheelDirection * GetAcceleration() * Mathf.Abs(brakeInput);

                            //Allow cars a bit more acceleration when going uphill just incase
                            accelerationForce *= 1 + Mathf.Clamp(Mathf.Abs(wheelDirection.y * 20), 0, 1);
                            //stringForGizmoDebug += "\nAF = " + accelerationForce;

                            if (!s_playersCanMove || !m_canIMove)
                            {
                                accelerationForce = Vector3.zero;
                                brakeForce = Vector3.zero;
                            }

                            //This makes it easier to balance four wheel drive with two wheel drive cars
                            if (m_baseCarInfo.m_myDriveMode == CarInfo_s.driveMode_e.allWheels)
                            {
                                accelerationForce *= 0.5f;
                                brakeForce *= 0.5f;
                            }

                            float wheelSpinMultiplier = Mathf.Clamp(Mathf.Abs(1 - m_curWheelSpin), 0.1f, 1);

                            if (wheelSpinMultiplier < 0.4f)
                            {
                                MakeWheelSkid(i);
                            }

                            if (!CurrentlyHandbraking)
                            {
                                float extraStartupSpd = 1 - m_normalisedForwardVelocity;
                                accelerationForce += accelerationForce.normalized * extraStartupSpd;

                                m_rb.AddForce(accelerationForce * wheelSpinMultiplier, ForceMode.Acceleration);
                                m_rb.AddForce(brakeForce * wheelSpinMultiplier, ForceMode.Acceleration);
                            }

                            //Brakes rather than reverse
                            if (Mathf.Abs(brakeForce.magnitude) > 0.1f && m_normalisedForwardVelocity > 0.1f)
                            {
                                m_currentlyBraking = true;
                                MakeWheelSkid(i);
                            }

                            //Slows down the car when turning
                            if (!CurrentlyHandbraking)
                            {
                                float turnDragScalar = 1.2f;

                                if (m_baseCarInfo.m_myDriveMode == CarInfo_s.driveMode_e.frontWheels)
                                {
                                    if (!IsWheelSkidding(i))
                                    {
                                        turnDragScalar = 0.25f;
                                    }
                                    else
                                    {
                                        turnDragScalar = 0;
                                    }
                                }

                                m_rb.velocity = Vector3.Lerp(m_rb.velocity, m_rb.velocity * 0.5f, (steerAmount * turnDragScalar) * 0.001f);
                            }
                        }

                    }

                    //Stabiliser forces

                    //This bit helps if you're stuck on a bridge with half of your wheels on and half off
                    if (m_wheels[i].transform.localPosition.x > 0)
                    {
                        m_rb.AddForce(-transform.right * 8, ForceMode.Acceleration);
                    }
                    else
                    {
                        m_rb.AddForce(transform.right * 8, ForceMode.Acceleration);
                    }

                }

                if (s_playersCanMove && m_canIMove)
                {
                    if (CurrentlyHandbraking)
                    {
                        if (driftBtnPress)//m_rewiredPlayer.GetButtonDown("Drift"))
                        {
                            skiddingRight = Mathf.Clamp(transform.InverseTransformDirection(m_rb.velocity).x, -2, 2);
                        }

                        ApplyHandbrake();
                    }

                }
            }
        }

        public void DropOut()
        {
            GameController.s_singleton.PlayerHasDroppedOut(m_nplayerIndex);
            droppingOut = true;
            Destroy(gameObject);
        }

        Vector3 GetWheelDirection(int wheelIndex, float forwardMultiplier)
        {
            Vector3 wheelForward = transform.forward * forwardMultiplier;
            Vector3 direction = Vector3.Cross(m_wheelRaycasts[wheelIndex].normal, wheelForward);
            direction = Vector3.Cross(direction, m_wheelRaycasts[wheelIndex].normal);

            if (!currentlyFiringGrapple)
            {
                CheckForWheelSteepness(ref direction, wheelIndex);
            }

            return direction;
        }

        Vector3 CheckForWheelSteepness(ref Vector3 direction, int wheelIndex)
        {
            float steepLimit = 0.4f;

            if(m_climbAllWalls)
            {
                steepLimit = 100;
            }

            stringForGizmoDebug = "Steepness = " + direction.y;

            if (direction.y > steepLimit)
            {
                direction.y = -steepLimit;
                MakeWheelSkid(wheelIndex);
            }

            return direction;
        }

        float GetAcceleration()
        {
            return Mathf.Clamp((m_baseCarInfo.m_acceleration + m_boostStats.m_acceleration + m_surfaceStats.m_acceleration), 6, 100);
        }

        float GetMaxSpeed()
        {
            return Mathf.Clamp((m_baseCarInfo.m_maxSpeed + m_boostStats.m_maxSpeed + m_surfaceStats.m_maxSpeed), 2, 100);
        }

        float GetGrip()
        {
            //It's 0.95f here so that cars don't have completely perfect grip by default
            return 0.95f + (m_baseCarInfo.m_extraGrip + m_boostStats.m_extraGrip + m_surfaceStats.m_extraGrip);
        }

        public Transform GetSocket(Bam.CarSockets.Sockets whichSocket)
        {
            return m_mySockets.GetSocket(whichSocket);
        }

        void Steering(float dir, int index, out float steerAmount)
        {
            float steerAccelerationSpeed = 6.1915f;
            float steerAccMax = 6.5f;

            float curTorqueSpeed = 0;
            steerAmount = 0;

            //Steering
            if (index > 1)
            {
                float responsiveness = 25;
                responsiveness = 1;

                if (m_wheelInfos[index].m_skidding)
                    responsiveness *= 930;

                curTorqueSpeed = 0;

                curTorqueSpeed = (Mathf.Abs(m_wheelInfos[index].m_curSpeed)) * responsiveness;
                curTorqueSpeed = Mathf.Clamp(curTorqueSpeed, -steerAccelerationSpeed, steerAccelerationSpeed);

                if(CurrentlyReversing)
                {
                    dir = -dir;
                }

                //Slower turning at high speeds
                if (!CurrentlyHandbraking)
                {
                    float slowTurningAtHighSpeedsScalar = 1;
                    float steerSpeedMinimumNormalised = 0.95f;
                    float steerSpeedMaxReduction = 2.15f;

                    if(m_baseCarInfo.m_myDriveMode==CarInfo_s.driveMode_e.frontWheels && m_normalisedForwardVelocity>0)
                    {
                        steerSpeedMinimumNormalised = 0.55f;
                        slowTurningAtHighSpeedsScalar = 1.125f;
                    }

                    slowTurningAtHighSpeedsScalar -= Mathf.Abs(m_normalisedForwardVelocity) * (1-steerSpeedMinimumNormalised);
                    slowTurningAtHighSpeedsScalar = Mathf.Clamp(slowTurningAtHighSpeedsScalar, steerSpeedMinimumNormalised, 1);


                    float reduction = steerSpeedMaxReduction * (Mathf.Abs(m_normalisedForwardVelocity) * (1 - steerSpeedMinimumNormalised));
                    curTorqueSpeed -= reduction;
                    //curTorqueSpeed *= slowTurningAtHighSpeedsScalar;
                    curTorqueSpeed = Mathf.Clamp(curTorqueSpeed, 0, steerAccMax);
                }

                if (IsWheelGrounded(index))
                {
                    if(CurrentlyHandbraking)
                    {
                        curTorqueSpeed *= 0.5f;
                    }

                    //stringForGizmoDebug = "Torque Speed = " + curTorqueSpeed;
                    Vector3 torque = transform.up * dir * (curTorqueSpeed);

                    if (m_nplayerIndex == 1)
                    {
                        //Debug.Log("Wheel " + index + " Torque: " + torque + " and Dir: " + dir);
                    }

                    m_rb.AddTorque(torque, ForceMode.Acceleration);
                    steerAmount = torque.magnitude;

                    if(!CurrentlyHandbraking)
                    {
                        m_handBrakeTorque = dir;
                    }
                }
            }
        }

        //void ApplyHandbrakeTorque(int index, out float steerAmount)
        //{
        //    steerAmount = 0;

        //    if (CurrentlyHandbraking && index>1)
        //    {
        //        if (m_wheelIsGrounded[index])
        //        {
        //            float curTorqueSpeed = 0;

        //            curTorqueSpeed = Mathf.Abs(GetVelocity().magnitude);
        //            curTorqueSpeed = Mathf.Clamp(curTorqueSpeed, -m_baseCarInfo.m_fturnMaxSpeed, m_baseCarInfo.m_fturnMaxSpeed);

        //            Vector3 torque = m_handBrakeTorque.normalized * curTorqueSpeed;
        //            m_rb.AddTorque(torque, ForceMode.Acceleration);
        //            steerAmount = torque.magnitude;
        //        }
        //    }
        //}

        void MaintainMaxSpeed()
        {
            float maxSpeed = GetMaxSpeed();

            if(CurrentlyReversing)
            {
                maxSpeed *= 0.45f;
            }

            if (m_rb.velocity.magnitude>maxSpeed && !InMidAir)
            {              
                m_rb.velocity = m_rb.velocity.normalized * maxSpeed;
            }

            float actualMaxTurnSpeed = (m_baseCarInfo.m_turnMaxSpeed * (Mathf.Abs(m_normalisedForwardVelocity) + 0.25f));
            if (m_rb.angularVelocity.magnitude > actualMaxTurnSpeed && !InMidAir)
            {
                m_rb.angularVelocity = m_rb.angularVelocity.normalized * actualMaxTurnSpeed;
            }
        }

        void LateUpdate()
        {
            for (int i = 0; i < m_wheels.Length; i++)
            {
                if (i > 1)
                {
                    float lerpValue = 0.5f + m_rewiredPlayer.GetAxis("Move Horizontal") * 0.5f;

                    float maxWheelAngle = 25;
                    float newY = Mathf.Lerp(-maxWheelAngle, maxWheelAngle, lerpValue);

                    if(m_wheels[i].transform.localPosition.x<0)
                    {
                        newY += 180;
                    }

                    m_wheels[i].transform.localRotation = Quaternion.Lerp(m_wheels[i].transform.localRotation, Quaternion.Euler(new Vector3(m_wheels[i].localEulerAngles.x, newY, 0)), 8 * Time.deltaTime);
                }

                RotateWheel(i);
            }

            SuspensionEffects();
        }

        protected virtual void SuspensionEffects()
        {
            //Suspension
            //m_carBody.transform.position += Vector3.up * -m_rb.velocity.y * 0.1f;

            //m_carBody.transform.localPosition = Vector3.ClampMagnitude(m_carBody.transform.localPosition * 0.01f, 0.615f);
            //m_carBody.transform.localPosition = Vector3.Lerp(m_carBody.transform.localPosition, Vector3.zero, 2 * Time.deltaTime);

            Vector3 targetEuler = new Vector3(0,-180,0);
            Vector3 veloLocal = transform.InverseTransformDirection(m_rb.velocity);
            Vector3 veloEuler = veloLocal;

            veloEuler.x = veloLocal.z * 0.015f;
            veloEuler.z = veloLocal.x * 0.4f;
            veloEuler.y = veloLocal.x * 0.5f;

            veloEuler = Vector3.ClampMagnitude(veloEuler, 2);

            //m_carBody.transform.eulerAngles += veloEuler * 0.925f;
            //m_carBody.transform.localRotation = Quaternion.Lerp(m_carBody.transform.localRotation, Quaternion.Euler(targetEuler), 11 * Time.deltaTime);
            //m_carBody.transform.localPosition += Vector3.up * (Mathf.Abs(veloEuler.x * 0.01f) + Mathf.Abs(veloEuler.z * 0.01f));
        }

        IEnumerator InitialAccelerationBounce()
        {
            float timer = 0;

            while (timer < 0.1f)
            {
                //m_carBody.transform.localEulerAngles -= Vector3.right * (2 - timer * 2) * (0.5f * 0.45f);
                timer += Time.deltaTime;
                yield return new WaitForSeconds(0.01f);
            }
        }

        IEnumerator RollBackOver()
        {
            //This timer is here just in case this loop never ends naturally
            float timer = 5;

            //Plays sound effect
            m_mySoundScript.PlayOneShot(m_baseCarInfo.mySoundPack.swoosh, 0.5f);

            while (transform.up.y < 0.995f && timer > 0)
            {
                timer -= Time.deltaTime;
                m_rb.AddForce(Vector3.up * 9, ForceMode.Acceleration);
                m_rb.rotation = Quaternion.Lerp(m_rb.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 5 * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }

            m_rb.useGravity = true;
            m_fflipTimer = 0;
        }

        void OnCollisionStay()
        {
            if (CanMove)
            {
                if (!IsWheelGrounded(0) && !IsWheelGrounded(1) && m_rb.velocity.magnitude < 2 && m_fflipTimer<=3)
                {
                    m_fflipTimer += Time.deltaTime;

                    if (m_fflipTimer > 3)
                    {
                        StopCoroutine(RollBackOver());
                        StartCoroutine(RollBackOver());
                    }
                }
                else
                {
                    StopCoroutine(RollBackOver());
                    m_fflipTimer = 0;
                }
            }
        }


        void OnCollisionEnter(Collision col)
        {
            //m_carBody.transform.position += m_rb.velocity * 0.01f;
            float intensity = col.relativeVelocity.magnitude;

            //Debug.Log(gameObject.name + " has hit " + col.gameObject.name);

            if (Vector3.Dot(transform.forward, col.contacts[0].normal) < -0.2f)
            {
                //StartCoroutine("InitialAccelerationBounce");
            }
            else
            {
                //Debug.Log(rb.velocity);
            }
        }

        void RotateWheel(int index)
        {
            Transform wheel = m_wheels[index];
            float sideMultiplier = 1;

            if (wheel.localPosition.x > 0)
            {
                sideMultiplier *= -1;
            }

            wheel.Rotate(Vector3.right * m_wheelInfos[index].m_curSpeed * -1.5f * sideMultiplier);
        }

        public bool IsWheelSkidding(int index)
        {
            return m_wheelInfos[index].m_skidding;
        }


        public void ResetCar()
        {
            m_respawnTimer = 0;
            m_inWater = false;

            RaycastHit floor;

            if (Physics.Raycast(transform.position, -Vector3.up, out floor))
            {
                if (floor.distance < 25)
                {
                    Debug.DrawLine(transform.position, floor.point);
                    transform.position = floor.point;
                    
                }
            }

            /////////////////////////////////////

            m_rb.velocity = Vector3.zero;
            m_rb.angularVelocity = Vector3.zero;

            for (int i = 0; i < m_wheels.Length; i++)
            {
                m_wheelInfos[i].m_curSpeed = 0;
                m_wheelInfos[i].m_skidding = false;
            }

            if (m_myCamera)
            {
                m_myCamera.transform.position = transform.position - (transform.forward * 2);
                m_myCamera.transform.rotation = transform.rotation;
                m_myCamera.ResetAngle();

                if (m_myCamera.myTransitionScript)
                {
                    m_myCamera.myTransitionScript.EndTransition();
                }

                m_myCamera.PlayerHasReset();
            }
        }

        void PreventSkidding(int wheelIndex)
        {
            float skidTransitionSpeed = 2;

            if (IsWheelSkidding(wheelIndex) || CurrentlyHandbraking)
            {
                MakeWheelSkid(wheelIndex);
            }
            else
            {
                m_fcancelHoriForce = Mathf.Lerp(m_fcancelHoriForce, m_fcancelHoriForceTarget, skidTransitionSpeed * Time.deltaTime);
            }

            Vector3 velo = m_rb.velocity;
            Vector3 localVelo = transform.InverseTransformDirection(velo);

            float speedSimilarity = Mathf.Abs(Vector3.Dot(transform.forward, m_rb.velocity));
            float skidThreshold = 8.95f + m_curStats.m_extraGrip;

            //Makes it easier to skid at high speeds and hard to skid at slow speeds
            float lowSpeedExtraThreshold = 18.025f;
            skidThreshold += lowSpeedExtraThreshold - Mathf.Abs((velo.magnitude/GetMaxSpeed()) * lowSpeedExtraThreshold);


            float rotationSkidMultiplier = 0.15f;
            float rotationSkidThreshold = (skidThreshold * rotationSkidMultiplier) + (GetGrip() * rotationSkidMultiplier);

            //Debug.Log("ST: " + skidThreshold + " / Cur Spd: " + (localVelo));
            //Debug.Log("RST: " + rotationSkidThreshold + " / Cur Spd: " + Mathf.Abs(m_rb.angularVelocity.y));

            if (m_baseCarInfo.m_myDriveMode== CarInfo_s.driveMode_e.frontWheels)
            {
                skidThreshold *= 0.45f;
            }

            //Removes the sideways spiderman effect
            if(transform.up.y < 0.4)
            {
                //print("SS");
                //m_rb.AddForce(transform.up * 10, ForceMode.Acceleration);
            }

            if(IsWheelSkidding(wheelIndex))
            {
                rotationSkidThreshold *= 0.25f;
            }

            if ((Mathf.Abs(localVelo.x) > skidThreshold || Mathf.Abs(m_rb.angularVelocity.y) > rotationSkidThreshold || (CurrentlyHandbraking && wheelIndex<2)))
            {
                MakeWheelSkid(wheelIndex);                
            }
            else
            {
                MakeWheelStopSkidding(wheelIndex);
                //m_currentlySkidding = false;
                m_fcurSkidIntensity = Mathf.Lerp(m_fcurSkidIntensity, 0, 2.5f * Time.deltaTime);
            }

			//Applies strong horiontal friction unless a grapple is pulling me
			if ((wheelIndex>1) && !grappleOnMe)
            {
                m_fwheelTorque = localVelo.z;
                localVelo.x = Mathf.Lerp(localVelo.x, 0, m_fcancelHoriForce * (GetGrip()) * Time.deltaTime * transform.up.y);
                m_rb.velocity = transform.TransformDirection(localVelo);
            }
        }

        void MakeWheelStopSkidding(int index)
        {
            m_wheelInfos[index].m_skidding = false;
        }

        bool PerformWheelRaycast(Transform wheel, int index)
        {
            bool ret;
            bool previouslyGrounded = IsWheelGrounded(index);

            ret = Physics.SphereCast(wheel.transform.position, m_baseCarInfo.m_wheelSize * 0.15f, -transform.up, out m_wheelRaycasts[index], m_baseCarInfo.m_wheelSize * 2, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);

            Vector3 localNormal = transform.InverseTransformDirection(m_wheelRaycasts[index].normal);

            //m_wheelInfos[index].m_steepValue = 1 - Mathf.Abs(m_wheelRaycasts[index].normal.y);
            //m_wheelInfos[index].m_steepValue = Mathf.Clamp(-localNormal.z, 0, 10);
            //stringForGizmoDebug = "Steepness: " + m_wheelInfos[index].m_steepValue + " due to a LN of " + localNormal;
            //Debug.DrawLine(m_wheelRaycasts[index].point, m_wheelRaycasts[index].point + m_wheelRaycasts[index].normal * 10, Color.magenta);

            //if (m_wheelInfos[index].m_steepValue>0.75f)
            {
                //Debug.Log("Floor too steep here");
                
            }

            if (ret)
            {
                Debug.DrawLine(wheel.transform.position, m_wheelRaycasts[index].point, Color.red, 0.01f);
            }
            else
            {
                Debug.DrawLine(wheel.transform.position, wheel.transform.position - transform.up * m_baseCarInfo.m_wheelSize, Color.blue, 0.02f);
            }

            if (ret && !previouslyGrounded)
            {
                WheelHasLanded(index);
            }

            return ret;
        }

        ///<summary>Causes the car to smoothly boost forwards (for drift boosting or slipstreams etc)</summary>
        public void AccelerationBoost(float intensity = 1, float time = 1.5f)
        {
            m_boostStats.m_acceleration += intensity;
            boostTimer = time;
        }

        ///<summary>Causes the car to smoothly boost forwards (for drift boosting or slipstreams etc)</summary>
        public void SmallBoost(float intensity = 1, float time = 1.5f)
        {
            m_boostStats.m_acceleration += intensity;
            m_boostStats.m_maxSpeed += intensity;

            boostTimer = time;
        }

        ///<summary>Causes the car to boost forwards with fire coming out of the back</summary>
        public void NitroBoost(float intensity = 1, float time = 3.5f)
        {
            float nitroIntensity = 3;

            m_boostStats.m_acceleration += intensity * nitroIntensity;
            m_boostStats.m_maxSpeed += intensity * nitroIntensity;

            boostTimer = time;
        }

        void WheelHasLanded(int index)
        {
            StopCoroutine("RollBackOver");
            m_mySoundScript.WheelHasLanded();

            if (m_rb.velocity.y < -1)
                m_wheelLandParticles[index].Play();
        }
    }
}