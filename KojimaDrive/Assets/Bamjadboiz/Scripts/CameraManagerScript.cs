using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Kojima
{
    public class CameraManagerScript : MonoBehaviour
    {

        //These are for convenience when you set up a new view
        public static bool[] FollowPlayerOne, FollowPlayerTwo, FollowPlayerThree, FollowPlayerFour, FollowAll;

        public static bool s_currentlyTransitioning;
        screenSetup_s upcomingScreenSetup;

        public static CameraManagerScript singleton;

        [System.Serializable]
        public struct screenSetup_s
        {
            public int cameras;
            public Bam.PlayerCameraScript.CameraInfo[] camInfos;

            public screenSetup_s(int howManyCameras)
            {
                cameras = howManyCameras;
                camInfos = new Bam.PlayerCameraScript.CameraInfo[cameras];

                
                for (int i = 0; i < camInfos.Length; i++)
                {
                    camInfos[i].m_followThesePlayers = new bool[4];
                }
            }
        }

        public GameObject playerCameraPrefab;
        public Bam.PlayerCameraScript[] playerCameras;

        public Image transitionImg;

        //QUICK SETUPS
        /// <summary>
        /// This puts the game in the mode where the camera is overhead and tries to keep all players on screen.
        /// </summary>
        public void SetupLobbyCamera()
        {
            //screenSetup_s newSS = new screenSetup_s();
            //newSS.cameras = 1;
            //newSS.camInfos = new Bam.PlayerCameraScript.CameraInfo[4];

            //newSS.camInfos[0].m_followThesePlayers = new bool[4] { true, true, true, true };
            //newSS.camInfos[0].m_viewStyle = Bam.PlayerCameraScript.viewStyles_e.overhead;
            //newSS.camInfos[0].m_positionOnScreen = Bam.PlayerCameraScript.screenPositions_e.fullScreen;

            //NewScreenSetup(newSS);

            SetupFullscreenOverheadView(true, true, true, true);
        }

        /// <summary>
        /// This is basically the lobby camera where the camera is overhead but here you can change which players stay on-screen.
        /// </summary>
        /// <param name="followPlayerOne"></param>
        /// <param name="followPlayerTwo"></param>
        /// <param name="followPlayerThree"></param>
        /// <param name="followPlayerFour"></param>
        public void SetupFullscreenOverheadView(bool followPlayerOne = true, bool followPlayerTwo = true, bool followPlayerThree = true, bool followPlayerFour = true)
        {
            screenSetup_s newSS = new screenSetup_s();
            newSS.cameras = 1;
            newSS.camInfos = new Bam.PlayerCameraScript.CameraInfo[4];

            newSS.camInfos[0].m_nmainPlayer = 0;
            newSS.camInfos[0].m_followThesePlayers = new bool[4] { followPlayerOne, followPlayerTwo, followPlayerThree, followPlayerFour };
            newSS.camInfos[0].m_viewStyle = Bam.PlayerCameraScript.viewStyles_e.overhead;
            newSS.camInfos[0].m_positionOnScreen = Bam.PlayerCameraScript.screenPositions_e.fullScreen;

            NewScreenSetup(newSS);
        }

        /// <summary>
        /// Transitions the game into having four screens, one for each player and each one in standard third person view. You can change a particular camera's ViewStyle afterwards.
        /// </summary>
        public void SetupThirdPersonForAllPlayers()
        {
            screenSetup_s newSS = new screenSetup_s();
            newSS.cameras = Mathf.Clamp(GameController.s_ncurrentPlayers, 1, 4);

            newSS.camInfos = new Bam.PlayerCameraScript.CameraInfo[4];

            for (int i = 0; i < 4; i++)
            {
                newSS.camInfos[i].m_nmainPlayer = i + 1;
                newSS.camInfos[i].m_followThesePlayers = new bool[4];

                //for (int fTP = 0; fTP < 4; fTP++)
                //{
                //    newSS.camInfos[i].m_followThesePlayers[fTP] = fTP == i;
                //}

                newSS.camInfos[i].m_positionOnScreen = (Bam.PlayerCameraScript.screenPositions_e)i;
            }

            if (GameController.s_ncurrentPlayers == 3)
            {
                //newSS.camInfos[0].m_positionOnScreen = Bam.PlayerCameraScript.screenPositions_e.topHalf;
                newSS.camInfos[2].m_positionOnScreen = Bam.PlayerCameraScript.screenPositions_e.bottomHalf;
            }

            if (GameController.s_ncurrentPlayers == 2)
            {
                newSS.camInfos[0].m_positionOnScreen = Bam.PlayerCameraScript.screenPositions_e.topHalf;
                newSS.camInfos[1].m_positionOnScreen = Bam.PlayerCameraScript.screenPositions_e.bottomHalf;
            }

            if (GameController.s_ncurrentPlayers == 1)
            {
                newSS.camInfos[0].m_positionOnScreen = Bam.PlayerCameraScript.screenPositions_e.fullScreen;
            }

            NewScreenSetup(newSS);
        }

        /// <summary>
        /// Allows you to change the ViewStyle of a particular player's camera. For example, this is how to have one player's camera be overhead while others are in third person.
        /// </summary>
        /// <param name="playerIndex"> Set this to 1 to affect player 1's camera (or at least, the camera in the top left)</param>
        /// <param name="newViewStyle"></param>
        public void ChangeViewStyle(int playerIndex, Bam.PlayerCameraScript.viewStyles_e newViewStyle)
        {
            if (!s_currentlyTransitioning)
            {
                playerCameras[playerIndex - 1].SwitchViewStyle(newViewStyle, playerIndex);
            }
            else
            {
                upcomingScreenSetup.camInfos[playerIndex - 1].m_viewStyle = newViewStyle;
            }
        }

        //OTHER PUBLIC FUNCTIONS
        /// <summary>
        /// Pass a customised, filled in ScreenSetup to this to properly transition to a new ScreenSetup
        /// </summary>
        /// <param name="newSetup"></param>
        public void NewScreenSetup(screenSetup_s newSetup)
        {
            StopCoroutine("Transition");
            upcomingScreenSetup = newSetup;
            StartCoroutine("Transition", newSetup);
        }


        void Awake()
        {
            singleton = this;
            transitionImg.fillAmount = 1;
            transitionImg.color = Color.white;
        }

        // Use this for initialization
        void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            playerCameras = CreateCameras(GameController.s_ncurrentPlayers);
            //SetupThirdPersonForAllPlayers();

            FollowPlayerOne = new bool[4] { true, false, false, false };
            FollowPlayerTwo = new bool[4] { false, true, false, false };
            FollowPlayerThree = new bool[4] { false, false, true, false };
            FollowPlayerFour = new bool[4] { false, false, false, true };
            FollowAll = new bool[4] { true, true, true, true };

            transitionImg.fillAmount = 1;
            transitionImg.color = Color.white;

            for (int i = 0; i < GameController.s_ncurrentPlayers; i++)
            {
                if (Bam.MainHUDScript.singleton)
                {
                    if (Bam.MainHUDScript.singleton.playerHUDs[i])
                    {
                        Bam.MainHUDScript.singleton.playerHUDs[i].GetComponent<Canvas>().worldCamera = playerCameras[i].GetUICameraComponent();
                        Bam.MainHUDScript.singleton.playerHUDs[i].GetComponent<Canvas>().planeDistance = 0.5f;
                    }
                }
            }

            SetupThirdPersonForAllPlayers();
        }

		private void OnDestroy() {
			// Cleaning this up
			UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
		}

		private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
        {
            SetupThirdPersonForAllPlayers();
        }

        IEnumerator Transition()
        {
            float speed = 4.5f;
            s_currentlyTransitioning = true;

            if (Time.timeSinceLevelLoad > 1)
            {
                transitionImg.fillAmount = 0;
                transitionImg.color = new Color(1, 1, 1, 0);
            }

            while (transitionImg.fillAmount < 1)
            {
                transitionImg.fillClockwise = true;
                transitionImg.fillAmount += Time.deltaTime * speed;
                //transitionImg.color = Color.Lerp(transitionImg.color, Color.white, speed * 3 * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            //Disable all cameras 
            for (int i = 0; i < playerCameras.Length; i++)
            {
                if (playerCameras[i])
                    playerCameras[i].gameObject.SetActive(false);
            }

            if (playerCameras.Length > 0)
            {
                for (int i = 0; i < upcomingScreenSetup.cameras; i++)
                {
                    if (playerCameras[i])
                    {
                        playerCameras[i].gameObject.SetActive(true);
                        playerCameras[i].SetupCamera(upcomingScreenSetup.camInfos[i]);
                    }
                }
            }

            upcomingScreenSetup = new screenSetup_s();

            if (Bam.MainHUDScript.singleton)
            {
                Bam.MainHUDScript.singleton.ToggleHUDLights(upcomingScreenSetup.cameras != 1);
            }

            for (float delay = 1; delay > 0; delay -= Time.deltaTime)
            {
                yield return new WaitForEndOfFrame();
            }

            while (transitionImg.fillAmount > 0)
            {               
                transitionImg.fillClockwise = false;
                transitionImg.fillAmount -= Time.deltaTime * speed;

                yield return new WaitForEndOfFrame();
            }

            s_currentlyTransitioning = false;
        }

        // Update is called once per frame
        void Update()
        {
            DebugStuff();
        }

        Bam.PlayerCameraScript[] CreateCameras(int number)
        {
            Bam.PlayerCameraScript[] newCams = new Bam.PlayerCameraScript[number];

            for (int i = 0; i < number; i++)
            {
                GameObject cam = Instantiate(playerCameraPrefab, transform.position, transform.rotation) as GameObject;
                newCams[i] = cam.GetComponent<Bam.PlayerCameraScript>();
                ObjectDB.DontDestroyOnLoad_Managed(cam);
            }

            return newCams;
        }

        void DebugStuff()
        {
            if (Input.GetKeyDown(KeyCode.L))
                SetupLobbyCamera();

            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetupThirdPersonForAllPlayers();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetupLobbyCamera();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetupFullscreenOverheadView(true, true, false, false);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetupThirdPersonForAllPlayers();
                ChangeViewStyle(1, Bam.PlayerCameraScript.viewStyles_e.overhead);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                screenSetup_s newSS = new screenSetup_s(2);
                newSS.camInfos[0].m_viewStyle = Bam.PlayerCameraScript.viewStyles_e.overhead;
                newSS.camInfos[1].m_viewStyle = Bam.PlayerCameraScript.viewStyles_e.thirdPerson;

                newSS.camInfos[0].m_positionOnScreen = Bam.PlayerCameraScript.screenPositions_e.topHalf;
                newSS.camInfos[1].m_positionOnScreen = Bam.PlayerCameraScript.screenPositions_e.bottomHalf;

                newSS.camInfos[0].m_followThesePlayers = FollowAll;
                newSS.camInfos[1].m_nmainPlayer = 1;

                NewScreenSetup(newSS);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SetupThirdPersonForAllPlayers();
                ChangeViewStyle(4, Bam.PlayerCameraScript.viewStyles_e.spectator);
            }
        }

        public void ResetPlayerCameraFocus(int playerIndex)
        {
            playerCameras[playerIndex].AssignNewMainPlayer(GameController.s_singleton.m_players[playerIndex]);
        }
    }
}