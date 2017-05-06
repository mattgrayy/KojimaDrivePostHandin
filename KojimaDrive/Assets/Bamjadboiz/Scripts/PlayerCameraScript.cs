//Author:       TMS
//Description:  Script that handles a player's camera.
//Last Edit:    Yams @ 14/01/2017  

using UnityEngine;
using System.Collections;
using Rewired;

namespace Bam
{
    public class PlayerCameraScript : MonoBehaviour
    {
        bool orthographicMode = true;

        [System.Serializable]
        public struct CameraInfo
        {
            public viewStyles_e m_viewStyle;
            public screenPositions_e m_positionOnScreen;

            public int m_nmainPlayer;
            public bool[] m_followThesePlayers;
        }

        CameraInfo m_curCamInfo;

        public LayerMask m_baseLayerMask;

        [System.Serializable]
        public enum screenPositions_e { topLeft, topRight, bottomLeft, bottomRight, topHalf, bottomHalf, fullScreen }

        public Camera Cam { get { return m_cam; } }
        Camera m_cam;
        public Camera[] m_SupplimentaryCameras;
		public Camera m_PostFXCamera;

        public enum viewStyles_e { thirdPerson, overhead, driving, bonnet, spectator, caravan, inGameZone }
        viewStyles_e m_currentViewStyle = viewStyles_e.driving;

        //public CarScript[] myPlayers;
        public Kojima.CarScript m_mainPlayer;
        public bool[] m_followingThesePlayers;

        float m_turnSpeed = 190;
        float m_distanceFromPlayer = 7.5f;
        float m_distanceFromPlayerOffset = 0;



        //Overhead
        public Vector3 m_curPos;
        public GameObject m_OHeadCanvasPrefab;

        //Third person
        [SerializeField]
        float m_freeCamTimer = 0;

        bool m_onWall = false;
        Vector3 m_wallPos;
        Vector3 m_inputVelo = Vector3.zero;

        bool m_whiskerWalling = false;

        public Vector3 m_wallOffset;
        Vector3 m_skidOffset;
        Vector3 m_veloOffset;
        Vector3 m_landOffset;

        float m_curSpd = 0;

        float m_targetFOV = 65;

        //HUD
        public GameObject myCanvasPrefab;
        Canvas myCanvas;
        LocationNameScript myLocationHUD;
        BamRespawnHUDScript myRespawnHUD;
        public PlayerScreenTransitionScript myTransitionScript;

        //Other
        //bool stopFollowing = false;
        bool readyToReset = false;

        //FX
        [SerializeField]
        Bird.PostFX_Generic speedBlur, saturation;

        static float blackNWhiteSat = -0.95f, standardSat = 0.0f;
        static bool blackNWhiteWhenCannotMove = true;

        // Use this for initialization
        void Awake()
        {
            m_followingThesePlayers = new bool[4];
            m_cam = GetComponent<Camera>();
        }

        void Start()
        {
            if (m_mainPlayer)
            {
                m_curPos = -m_mainPlayer.transform.eulerAngles;
                transform.position = m_mainPlayer.transform.position;
            }

            SetupMyCanvas();
        }

        void SetupMyCanvas()
        {
            int uiLayer = m_mainPlayer.m_nplayerIndex + 7;

            myCanvas = GameObject.Instantiate<GameObject>(myCanvasPrefab).GetComponent<Canvas>();
            myCanvas.gameObject.transform.SetParent(transform);
            myCanvas.worldCamera = m_SupplimentaryCameras[0];
            myCanvas.gameObject.layer = uiLayer;

            for (int i = 0; i < myCanvas.transform.childCount; i++)
            {
                myCanvas.transform.GetChild(i).gameObject.layer = uiLayer;
            }

            myLocationHUD = gameObject.GetComponentInChildren<LocationNameScript>();
            myTransitionScript = gameObject.GetComponentInChildren<PlayerScreenTransitionScript>();

            myRespawnHUD = gameObject.GetComponentInChildren<BamRespawnHUDScript>();
            myRespawnHUD.myCam = this;
        }

        public void PlayerHasReset()
        {
            m_wallOffset = Vector3.zero;

            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ResetThirdPersonAngle());
            }

            readyToReset = true;

            m_curSpd = 1;
        }

        // Update is called once per frame
        void Update()
        {
            float rate = 0.4f;
            m_curSpd = Mathf.MoveTowards(m_curSpd, 1, rate * Time.deltaTime);

            m_inputVelo = Vector3.Lerp(m_inputVelo, Vector3.zero, 4 * Time.deltaTime);


            //Handles offsets
            if (!m_onWall && !m_whiskerWalling)
            {
                m_wallOffset = Vector3.Lerp(m_wallOffset, Vector3.zero, 5 * Time.deltaTime);
            }

            //m_landOffset = Vector3.Lerp(m_landOffset, Vector3.zero, 3 * Time.deltaTime);
            m_veloOffset = Vector3.Lerp(m_veloOffset, Vector3.zero, 1 * Time.deltaTime);
        }

        public Camera GetCameraComponent()
        {
            return m_cam;
        }

        public Camera GetUICameraComponent(int nIndex = 0)
        {
            return m_SupplimentaryCameras[nIndex];
        }

		public Camera GetPostFXCamera() {
			return m_PostFXCamera;
		}

		public Bird.PostFXStack GetPostFXStack() {
			return m_PostFXCamera.GetComponent<Bird.PostFXStack>();
		}

        public void DisplayLocationName(string locationName)
        {
            myLocationHUD.DisplayLocation(locationName);
        }

        public void SetupCamera(CameraInfo newInfo)
        {
            if (m_mainPlayer)
            {
                m_mainPlayer.ClearCamera();
            }

            m_mainPlayer = null;
            m_curCamInfo = newInfo;

            m_followingThesePlayers = newInfo.m_followThesePlayers;
            SwitchViewStyle(newInfo.m_viewStyle);

            switch (newInfo.m_positionOnScreen)
            {
                case screenPositions_e.bottomLeft:
                    MoveScreenToHere(new Vector2(0, 0), new Vector2(0.5f, 0.5f));
                    break;
                case screenPositions_e.bottomRight:
                    MoveScreenToHere(new Vector2(0.5f, 0), new Vector2(0.5f, 0.5f));
                    break;
                case screenPositions_e.topLeft:
                    MoveScreenToHere(new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f));
                    break;
                case screenPositions_e.topRight:
                    MoveScreenToHere(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
                    break;

                case screenPositions_e.topHalf:
                    MoveScreenToHere(new Vector2(0.0f, 0.5f), new Vector2(1f, 0.5f));
                    break;
                case screenPositions_e.bottomHalf:
                    MoveScreenToHere(new Vector2(0.0f, 0.0f), new Vector2(1f, 0.5f));
                    break;

                case screenPositions_e.fullScreen:
                    MoveScreenToHere(new Vector2(0.0f, 0.0f), new Vector2(1f, 1f));
                    break;
            }

            HandleCullingMask(newInfo);
			HandleCameraDepth();

		}



        public void Land(float landIntensity)
        {
            if (enabled && gameObject.activeInHierarchy)
            {
                if (landIntensity >= 4)
                {
                    StopCoroutine("LandAnimation");
                    StartCoroutine("LandAnimation", landIntensity);
                }
            }
            //m_landOffset -= Vector3.up * landIntensity * 0.05f;
        }

        IEnumerator LandAnimation(float landIntensity)
        {
            float spd = 1.25f;
            landIntensity *= 0.025f;

            while (m_landOffset.y > -landIntensity)
            {
                //Debug.Log(m_landOffset.y);
                //m_landOffset.y = Mathf.Lerp(m_landOffset.y, -landIntensity * 2, 4 * Time.deltaTime);

                m_landOffset.y -= Time.deltaTime * spd;
                spd += 25 * Time.deltaTime;
                //m_landOffset = Vector3.Lerp(m_landOffset, -Vector3.up * landIntensity * 1.5f, 3 * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }

            while (m_landOffset.y < 0.05f)
            {
                //m_landOffset.y += 10 * Time.deltaTime * spd;
                //spd += 2 * Time.deltaTime;
                m_landOffset = Vector3.Lerp(m_landOffset, Vector3.zero, 3 * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }

            m_landOffset = Vector3.zero;
        }

        void HandleCullingMask(CameraInfo newInfo)
        {
            Cam.cullingMask = m_baseLayerMask;

            // Moved this to UI Camera. -sam
            if (m_mainPlayer)
            {
                Cam.cullingMask |= 1 << (m_mainPlayer.m_nplayerIndex + 14);
            }

            if (m_SupplimentaryCameras.Length > 0 && m_mainPlayer != null)
            {
                m_SupplimentaryCameras[0].cullingMask = 0;
                m_SupplimentaryCameras[0].cullingMask |= 1 << (m_mainPlayer.m_nplayerIndex + 7);
            }

			if (m_mainPlayer) {
				m_PostFXCamera.cullingMask = 0; // Don't render anything, we're just a post processor
				Bird.PostFXStack stack = m_PostFXCamera.GetComponent<Bird.PostFXStack>();
				stack.GlobalName = "PostFX_P" + m_mainPlayer.m_nplayerIndex.ToString();
			}
		}

		void HandleCameraDepth() {
			float fCurDepth = Cam.depth + 1.0f;
			m_PostFXCamera.depth = fCurDepth++; // PostFX should be before any kind of UI

			for (int i = 0; i < m_SupplimentaryCameras.Length; i++) {
				m_SupplimentaryCameras[i].depth = fCurDepth++;
			}
		}

        public void GiveBlurVal(float blurVal)
        {
            
        }

        public void ResetSpd()
        {
            m_curSpd = 0;
        }

        public void SwitchViewStyle(viewStyles_e newViewStyle, int newMainPlayerIndex = 0)
        {
            if (newViewStyle != m_currentViewStyle)
            {
                m_curSpd = 0;
            }

            m_currentViewStyle = newViewStyle;

            switch (newViewStyle)
            {
                case viewStyles_e.overhead:
                    m_cam.orthographic = true;
                    if(myRespawnHUD)
                    myRespawnHUD.gameObject.SetActive(false);
                    //SetupPlayerIDCanvas();
                    break;
                case viewStyles_e.thirdPerson:
                case viewStyles_e.caravan:
                    m_cam.orthographic = false;
                    if(myRespawnHUD)
                    myRespawnHUD.gameObject.SetActive(true);
                    break;
                case viewStyles_e.driving:
                    m_cam.orthographic = false;
                    break;
                case viewStyles_e.spectator:
                    m_cam.orthographic = false;
                    m_mainPlayer = null;
                    m_curCamInfo.m_nmainPlayer = 0;
                    return;
                    break;
            }

            if (newMainPlayerIndex > 0)
            {
                m_curCamInfo.m_nmainPlayer = newMainPlayerIndex;
            }

            speedBlur.m_ShaderProps[0].m_fVal = 0.0f;
            saturation.m_ShaderProps[0].m_fVal = standardSat;

            if (m_curCamInfo.m_nmainPlayer > 0)
            {
                m_mainPlayer = Kojima.GameController.s_singleton.m_players[m_curCamInfo.m_nmainPlayer - 1];

                if (m_mainPlayer)
                {
                    m_mainPlayer.GiveCamera(this);
                    m_distanceFromPlayer = m_mainPlayer.m_baseCarInfo.m_cameraDistance;
                }
                else
                {
                    Debug.LogError("Camera for player " + newMainPlayerIndex + " couldn't find a car to follow! Switching to spectator mode");
                    SwitchViewStyle(viewStyles_e.spectator);
                }

                ResetAngle();
            }

            //if (m_curCamInfo.m_nmainPlayer!=0)
            //{
            //    for (int i = 0; i < m_followingThesePlayers.Length; i++)
            //    {
            //        //int numberFollowing = 0;
            //        //if (m_followingThesePlayers[i])
            //        //{
            //        //    m_mainPlayer = Kojima.GameController.s_singleton.m_players[i];
            //        //    Debug.Log("setting topdown main to " + m_mainPlayer.name);
            //        //    StartCoroutine("ResetThirdPersonAngle");
            //        //    numberFollowing++;
            //        //    Debug.Log(numberFollowing);
            //        //}

            //        //if(numberFollowing==4)
            //        //{
            //        //    Debug.Log("resetting main");
            //        //    m_mainPlayer = null;
            //        //}
            //    }
            //}
            //else
            //{
            //    m_mainPlayer = Kojima.GameController.s_singleton.m_players[m_curCamInfo.m_nmainPlayer];
            //    Debug.Log("setting PROPERLY main to " + m_mainPlayer.name);
            //    if (gameObject.activeInHierarchy)
            //    {
            //        StartCoroutine("ResetThirdPersonAngle");
            //    }
            //}
        }

        void SetupPlayerIDCanvas()
        {
            for(int i = 0; i < m_followingThesePlayers.Length; i++)
            {
                if(m_followingThesePlayers[i])
                {
                    GameObject overheadCanvas = Instantiate(m_OHeadCanvasPrefab);
                    overheadCanvas.GetComponent<Bam.SabotagePlayerIDCanvas>().SetPlayerID(Kojima.GameController.s_singleton.m_players[i],
                        this);
                }
            }
        }

        public void ResetAngle()
        {
            if (m_mainPlayer && gameObject.activeInHierarchy)
            {
                StopCoroutine(ResetThirdPersonAngle());
                StartCoroutine(ResetThirdPersonAngle());
                m_freeCamTimer = 0;
            }
        }

        IEnumerator ResetThirdPersonAngle()
        {
            if (m_mainPlayer)
            {
                float targetY = m_mainPlayer.transform.eulerAngles.y;
                float amount = Mathf.Abs(targetY - m_curPos.y);
                float speed = 5;

                for (float timer = 0; timer < amount - 0.15f; timer += Time.deltaTime)
                {
                    m_curPos.x = Mathf.LerpAngle(m_curPos.x, 6, speed * Time.deltaTime);
                    m_curPos.y = Mathf.LerpAngle(m_curPos.y, targetY, speed * Time.deltaTime);

                    timer = Mathf.Lerp(timer, amount, speed * Time.deltaTime);
                    yield return new WaitForEndOfFrame();
                }
            }
            //Debug.Log("DONE");
        }

        Vector3 GetTargetPos(Quaternion rot, Vector3 backwards)
        {
            return m_mainPlayer.transform.position + (rot * backwards);
        }

        //void DriveCam(Vector3 input)
        //{
        //    float camSpeed = 7, camRotationSpeed = 5.5f;
        //    float backDistance = 4.25f;
        //    float upDistance = 1.15f;

        //    Vector3 offset = input;
        //    Quaternion finalRotation = transform.rotation;
        //    Vector3 forward = Vector3.forward;

        //    Vector3 target = m_mainPlayer.transform.position - (m_mainPlayer.transform.forward * backDistance) + m_mainPlayer.transform.up * upDistance;

        //    target = DetectWalls(target, m_mainPlayer.transform.position + (Vector3.up * 1.0f));

        //    if (!m_mainPlayer.InMidAir)
        //    {
        //        forward = Quaternion.LookRotation(m_mainPlayer.transform.forward + (m_mainPlayer.transform.up * 0.2f), Vector3.up).eulerAngles;
        //        finalRotation = Quaternion.Euler(forward - input * 45);
        //    }
        //    else
        //    {
        //        target = m_mainPlayer.transform.position + (-transform.forward * backDistance) + Vector3.up * upDistance;
        //        forward = Quaternion.LookRotation(m_mainPlayer.transform.forward, Vector3.up).eulerAngles;
        //        finalRotation = Quaternion.LookRotation((m_mainPlayer.transform.position - transform.position).normalized);
        //        camRotationSpeed = 2;
        //        camSpeed = 4;
        //    }


        //    transform.rotation = Quaternion.Lerp(transform.rotation, finalRotation, camRotationSpeed * Time.deltaTime);

        //    transform.position = Vector3.Lerp(transform.position, target, camSpeed * Time.deltaTime);
        //    AllowBehindCam();
        //}

        bool WhiskerCast(Vector3 targetPos, Vector3 startPos)
        {
            RaycastHit rH;

            Vector3 dir = targetPos - startPos;
            //Debug.DrawRay(startPos, dir, Color.cyan);

            float radius = 1.1f;

            if (Physics.SphereCast(startPos, radius, dir, out rH, Vector3.Distance(targetPos, startPos) - (radius * 0.5f), LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
            {
                m_onWall = true;

                Vector3 newOffset = rH.point - targetPos;
                newOffset += rH.normal * 0.2f;

                m_wallOffset = Vector3.Lerp(m_wallOffset, newOffset, 3 * Time.deltaTime);
                //Debug.Log(rH.collider.name);
                //Debug.DrawLine(startPos, rH.point, Color.cyan);

                return true;
            }

            return false;
        }

        Vector3 DetectWalls(Vector3 targetPos, Vector3 startPos)
        {
            Vector3 returnVector = targetPos;

            RaycastHit rH;

            Debug.DrawLine(startPos + (Vector3.up * 1.0f), targetPos, Color.red);

            if (Physics.Linecast(startPos, targetPos, out rH, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    Debug.Log(rH.collider.gameObject.name);
                    Debug.Break();
                }


                Vector3 point = rH.point;

                m_wallPos = point + rH.normal * 0.9f;

                Vector3 newWallOffset = m_wallPos - targetPos;


                //Immediately goes to the position of a wall if necessary
                if (newWallOffset.sqrMagnitude > m_wallOffset.sqrMagnitude)
                {
                    m_wallOffset = Vector3.Lerp(m_wallOffset, newWallOffset, 13 * Time.deltaTime);
                }
                else
                {
                    m_wallOffset = Vector3.Lerp(m_wallOffset, newWallOffset, 0.5f * Time.deltaTime);
                }

                Debug.DrawLine(targetPos, m_wallPos, Color.magenta);

                m_onWall = true;
                //m_freeCamTimer = 0.1f;
            }
            else
            {
                m_whiskerWalling = WhiskerCast(targetPos, startPos);

                if (m_onWall && !m_whiskerWalling)
                {
                    //Gradually go from wall stuff to normal
                    //m_wallPos = Vector3.zero;
                    m_onWall = false;
                }
            }

            return returnVector;
        }

        void AllowBehindCam()
        {
            Vector3 target;

            if (!m_mainPlayer.isActiveAndEnabled)
            {
                return;
            }

            //Looking behind
            if (m_mainPlayer.GetRewiredPlayer().GetButton("Look Behind"))
            {
                StopCoroutine(ResetThirdPersonAngle());
                m_freeCamTimer = 1;
                target = m_mainPlayer.transform.position + m_mainPlayer.transform.forward * 9.5f + m_mainPlayer.transform.up * 3;

                transform.position = DetectWalls(target, m_mainPlayer.transform.position + (Vector3.up * 1.0f));
                transform.rotation = Quaternion.LookRotation(-m_mainPlayer.transform.forward + Vector3.up * 0.155f);
                //m_curPos.y = Mathf.LerpAngle(m_curPos.y, m_mainPlayer.transform.eulerAngles.y, 25 * Time.deltaTime);
            }

            if (m_mainPlayer.GetRewiredPlayer().GetButtonUp("Look Behind"))
            {
                StartCoroutine(ResetThirdPersonAngle());
                transform.position = m_mainPlayer.transform.position - m_mainPlayer.transform.forward * 5 + Vector3.up * 2;
                transform.rotation = Quaternion.LookRotation(m_mainPlayer.transform.forward, m_mainPlayer.transform.up);
            }
        }

        void ThirdPerson(Vector3 input)
        {
            if (!m_mainPlayer)
            {
                return;
            }

            Vector3 upwardsOffset = Vector3.up * 1.25f;

            m_inputVelo += input * Time.deltaTime * 5;
            m_inputVelo = Vector3.ClampMagnitude(m_inputVelo, 5);

            m_targetFOV = (60 + (m_curPos.x * 0.45f)) + m_mainPlayer.m_normalisedForwardVelocity * (0.15f);

            m_curPos += -m_inputVelo * m_turnSpeed * Time.deltaTime;
            m_curPos.x = Mathf.Clamp(m_curPos.x, 10, 60);

            if (input.magnitude > 0.1f)
            {
                StopCoroutine(ResetThirdPersonAngle());
                m_freeCamTimer = 1.2f;
            }

            //Handles the "automatic" third person camera
            if (m_freeCamTimer <= 0 && (!m_mainPlayer.InMidAir || m_mainPlayer.CurrentlyGliding) && !m_onWall && m_mainPlayer.IsMoving)
            {
                Vector3 targetCurPos = m_mainPlayer.transform.eulerAngles;

                //if (m_mainPlayer.GetSkidIntensity() > 0.5f)
                //{
                //    Vector3 veloDir = Quaternion.LookRotation(m_mainPlayer.GetVelocity()).eulerAngles;
                //    m_curPos.y = Mathf.LerpAngle(m_curPos.y, veloDir.y, 0.1f * Time.deltaTime);
                //}
                //else
                {
                    m_curPos.y = Mathf.LerpAngle(m_curPos.y, Quaternion.Euler(targetCurPos).eulerAngles.y, 2 * Time.deltaTime);
                }

                if (!m_mainPlayer.CurrentlyGliding)
                {
                    m_curPos.x = Mathf.LerpAngle(m_curPos.x, 8, 1 * Time.deltaTime);
                }
            }
            else
            {
                m_freeCamTimer -= Time.deltaTime;
            }

            float distanceMultiplier = 0.65f + ((m_curPos.x - 10) * 0.0105f);

            if (m_currentViewStyle == viewStyles_e.caravan || m_mainPlayer.CurrentlyInWater)
            {
                distanceMultiplier = 1.75815f;
                upwardsOffset *= 0.5f;
            }

            if(m_distanceFromPlayer>=10)
            {
                distanceMultiplier = 1.1f;
                upwardsOffset *= 0.95f;
                
            }

            if(m_currentViewStyle == viewStyles_e.inGameZone)
            {
                distanceMultiplier = 2;
                upwardsOffset *= 1.1f;
            }

            //Have a further camera angle when gliding
            if (m_mainPlayer.CurrentlyGliding)
            {
                m_distanceFromPlayerOffset = Mathf.Lerp(m_distanceFromPlayerOffset, 10f, Time.deltaTime);
            }
            else
            {
                m_distanceFromPlayerOffset = Mathf.Lerp(m_distanceFromPlayerOffset, 0f, Time.deltaTime);
            }

            if (m_mainPlayer.AllWheelsGrounded)
            {
                //distanceMultiplier += Mathf.Abs(m_mainPlayer.transform.up.x * 1.25f);
            }

            Vector3 targetPos = Vector3.zero;

            m_veloOffset = Vector3.Lerp(m_veloOffset, -m_mainPlayer.GetVelocity() * 0.085f, 1 * Time.deltaTime);

            Vector3 backwards = new Vector3(0, 0, -((m_distanceFromPlayer + m_distanceFromPlayerOffset) * distanceMultiplier));
            Quaternion rot = Quaternion.Euler(m_curPos);

            targetPos = m_mainPlayer.transform.position + (rot * backwards);
            targetPos -= m_mainPlayer.GetVelocity() * Time.deltaTime;

            //TODO: Rewired
            if (m_mainPlayer.isActiveAndEnabled)
            {
                if (m_mainPlayer.GetRewiredPlayer().GetButtonDown("Reset Camera"))
                {
                    StartCoroutine(ResetThirdPersonAngle());
                }
            }

            targetPos += upwardsOffset;
            targetPos = DetectWalls(targetPos, m_mainPlayer.transform.position + (Vector3.up * 1.0f));

            if (!m_mainPlayer.CurrentlyInWater)
            {
                PositionSelfHere(targetPos);
            }
            else
            {
                transform.position += Vector3.up * Time.deltaTime * 4;
            }
            //transform.position = Vector3.Lerp(transform.position, targetPos + m_mainPlayer.GetVelocity() * Time.deltaTime, GetSpeed(9) * Time.deltaTime);

            if (transform.position != m_mainPlayer.transform.position)
            {
                if (!readyToReset)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((m_mainPlayer.transform.position - transform.position).normalized), m_curSpd);
                }
                else
                {
                    transform.rotation = Quaternion.LookRotation((m_mainPlayer.transform.position - transform.position).normalized);
                }
                //transform.LookAt(m_mainPlayer.transform.position);
                transform.Rotate(-Vector3.right * upwardsOffset.y * 19.5f * m_curSpd);
            }

            //transform.position += m_landOffset;
            transform.eulerAngles += Vector3.right * m_landOffset.y * -18;

            AllowBehindCam();
        }

        void PositionSelfHere(Vector3 targetPos)
        {
            //targetPos += m_skidOffset;
            targetPos += m_wallOffset;
            //targetPos += m_veloOffset;
            targetPos += m_landOffset;

            transform.position = Vector3.Lerp(transform.position, targetPos, 35 * m_curSpd * Time.deltaTime);

            if(readyToReset)
            {
                transform.position = targetPos;
                readyToReset = false;
            }
        }

        float GetSpeed(float spd = 8)
        {
            return spd * m_curSpd;
        }

        Vector3 GetAveragePosition(Kojima.CarScript[] players)
        {
            Vector3 pos = Vector3.zero;

            Vector3[] playerPos = new Vector3[players.Length];
            int playersToFollow = 0;

            for (int i = 0; i < playerPos.Length; i++)
            {
                if (players[i] && m_followingThesePlayers[i])
                {
                    playerPos[i] = players[i].transform.position;
                    playersToFollow++;
                }
            }

            pos = GetAveragePosition(playerPos, playersToFollow);

            return pos;
        }

        Vector3 GetAveragePosition(Vector3[] positions, float divide = 0)
        {
            Vector3 pos = Vector3.zero;

            if (divide == 0)
                divide = positions.Length;

            float multiplier = 1 / (float)divide;

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] *= multiplier;
                pos += positions[i];
            }

            return pos;
        }

        void Overhead(Vector3 input)
        {
            Vector3 pos = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.O))
            {
                orthographicMode = !orthographicMode;
            }

            if (!m_mainPlayer)
                pos = GetAveragePosition(Kojima.GameController.s_singleton.m_players);
            else
                pos = m_mainPlayer.transform.position;


            Vector3 basePos = pos;
            float height = 13;
            float distance = 0;

            for (int i = 0; i < Kojima.GameController.s_singleton.m_players.Length - 1; i++)
            {
                if (i < Kojima.GameController.s_singleton.m_players.Length)
                {
                    if (Kojima.GameController.s_singleton.m_players[i] && Kojima.GameController.s_singleton.m_players[i + 1] && m_followingThesePlayers[i])
                    {
                        distance += Vector3.Distance(Kojima.GameController.s_singleton.m_players[i].transform.position, Kojima.GameController.s_singleton.m_players[i + 1].transform.position);
                    }
                }
            }

            m_targetFOV = 100 + distance * 0.35f;
            m_targetFOV = Mathf.Clamp(m_targetFOV, 80, 120);

            m_cam.orthographic = orthographicMode;

            if (orthographicMode)
            {
                m_cam.orthographicSize = 28 + distance * 0.10025f;
                distance = 10;
                pos += Vector3.up * m_cam.orthographicSize;
            }
            else
            {
                distance *= 0.25f;
                distance += 0.1f;
            }

            height += distance;

            pos += Vector3.up * height;

            //pos = DetectWalls(pos, basePos);

            transform.position = Vector3.Lerp(transform.position, pos, 8 * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(-Vector3.up);



            if (!orthographicMode)
            {
                ApplyOverheadAngle();
            }
        }

        void ApplyOverheadAngle()
        {
            transform.position -= (Vector3.forward + Vector3.up) * 0.5f;
            transform.eulerAngles = new Vector3(75, 0, 0);

            PositionSelfHere(transform.position);
        }

        void BonnetCam()
        {
            //if(m_mainPlayer.bonnetPos)
            {
                //transform.position = Vector3.Lerp(transform.position, m_mainPlayer.bonnetPos.position, 45 * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, DetectWalls(m_mainPlayer.GetSocket(CarSockets.Sockets.Bonnet).position + m_mainPlayer.transform.forward, m_mainPlayer.transform.position + (Vector3.up * 1.0f)), m_curSpd);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(m_mainPlayer.transform.forward, m_mainPlayer.transform.up), m_curSpd);
            }
            //transform.position = Vector3.Lerp(transform.position, m_mainPlayer.transform.position + m_mainPlayer.transform.forward * 3 + m_mainPlayer.transform.up * 1.5f, 10 * Time.deltaTime);

        }

        void TakeInput(out Vector3 stickInput)
        {
            stickInput = Vector3.zero;

            if (m_mainPlayer)
            {
                Rewired.Player player = ReInput.players.GetPlayer(m_mainPlayer.m_nplayerIndex - 1);
                stickInput = new Vector3(player.GetAxisRaw("Look Vertical"), -player.GetAxisRaw("Look Horizontal"), 0);

                if (player.GetButtonDown("Change Camera"))
                {
                    switch (m_currentViewStyle)
                    {
                        case viewStyles_e.bonnet:
                            //Skip the drive cam for now until it's better
                            //SwitchViewStyle(viewStyles_e.driving);
                            SwitchViewStyle(viewStyles_e.thirdPerson);
                            break;
                        case viewStyles_e.driving:
                            SwitchViewStyle(viewStyles_e.thirdPerson);
                            break;
                        case viewStyles_e.thirdPerson:
                            SwitchViewStyle(viewStyles_e.caravan);
                            break;
                        case viewStyles_e.caravan:
                            SwitchViewStyle(viewStyles_e.bonnet);
                            break;
                    }
                }
            }
        }

        void FixedUpdate()
        {
            Vector3 stickInput = Vector3.zero;

            TakeInput(out stickInput);

            switch (m_currentViewStyle)
            {
                case viewStyles_e.thirdPerson:
                case viewStyles_e.caravan:
                case viewStyles_e.inGameZone:
                    ThirdPerson(stickInput);
                    break;
                case viewStyles_e.overhead:
                    Overhead(stickInput);
                    break;
                case viewStyles_e.driving:
                    //DriveCam(stickInput);
                    break;
                case viewStyles_e.bonnet:
                    BonnetCam();
                    break;
            }

            float targetSaturation = standardSat;

            if (m_mainPlayer)
            {
                if (m_mainPlayer.CurrentlyBoosting)
                {
                    m_targetFOV += 5;
                }

                speedBlur.m_bEnabled = m_mainPlayer.m_normalisedForwardVelocity > 0.0f;
                float targetBlur = 0.0f;

                if(m_mainPlayer.m_normalisedForwardVelocity==1)
                {
                    targetBlur = 0.4f;
                }

                targetBlur += (m_mainPlayer.m_boostStats.m_acceleration + m_mainPlayer.m_boostStats.m_maxSpeed) * 0.25f * m_mainPlayer.m_normalisedForwardVelocity;

                speedBlur.m_ShaderProps[0].m_fVal = Mathf.Lerp(speedBlur.m_ShaderProps[0].m_fVal, targetBlur, 4 * Time.deltaTime);

                if(!m_mainPlayer.CanMove && !m_mainPlayer.CurrentlyInWater)
                {
                    targetSaturation = blackNWhiteSat;
                }
            }

            if (blackNWhiteWhenCannotMove)
            {
                float spd = 2.0f;

                saturation.m_ShaderProps[0].m_fVal = Mathf.Lerp(saturation.m_ShaderProps[0].m_fVal, targetSaturation, spd * Time.deltaTime);

                float targetContrast = 0.0f;

                if (targetSaturation < 0)
                {
                    targetContrast = -targetSaturation * 1.5f;
                }

                saturation.m_ShaderProps[2].m_fVal = Mathf.Lerp(saturation.m_ShaderProps[2].m_fVal, targetContrast, spd * 2 * Time.deltaTime);
            }

            m_cam.fieldOfView = Mathf.Lerp(m_cam.fieldOfView, m_targetFOV + saturation.m_ShaderProps[2].m_fVal * 10, 5 * Time.deltaTime);

            // Update supplimentary cameras
            for (int i = 0; i < m_SupplimentaryCameras.Length; i++)
            {
                m_SupplimentaryCameras[i].fieldOfView = m_cam.fieldOfView;
            }

            float extraDistanceFromWater = 5.5f;
            if(transform.position.y<Kojima.CarScript.m_waterYPosition + extraDistanceFromWater)
            {
                transform.position = new Vector3(transform.position.x, Kojima.CarScript.m_waterYPosition + extraDistanceFromWater, transform.position.z);
            }
        }

        public void MoveScreenToHere(Vector2 _newPos, Vector2 _size)
        {
            Rect newRect = m_cam.rect;
            newRect.position = _newPos;
            newRect.size = _size;

            m_cam.rect = newRect;
            // Update supplimentary cameras
            for (int i = 0; i < m_SupplimentaryCameras.Length; i++)
            {
                m_SupplimentaryCameras[i].rect = m_cam.rect;
            }
			m_PostFXCamera.rect = m_cam.rect; // Keep our PostFX local

		}

        public void AssignNewMainPlayer(Kojima.CarScript newPlayer)
        {
            if (newPlayer)
            {
                //Debug.Log("Camera now following player " + newPlayer.m_nplayerIndex);
                m_mainPlayer = newPlayer;
                m_mainPlayer.GiveCamera(this);
                m_distanceFromPlayer = m_mainPlayer.m_baseCarInfo.m_cameraDistance;

                transform.position = m_mainPlayer.transform.position;
            }
        }

    }

}