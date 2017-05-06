using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Bam
{
    public class LobbyManagerScript : MonoBehaviour
    {
        public static LobbyManagerScript singleton;
        public string m_strCurrentGame;
        public GameObject[] m_lobbies;

        bool m_hasLoaded, m_smallIsland;
        public string m_strLobbyName, m_strCurIsland;

        void Awake()
        {
            singleton = this;
        }

        // Use this for initialization
        void Start()
        {
			//ObjectDB.DontDestroyOnLoad_Managed(gameObject);
			//m_strLobbyName = "SmallIslandLobby";
			//m_strCurIsland = "KojimaIsland";

			SetupLobbyHUD();
			m_Transitioning = false;
		}

		public bool m_Transitioning = false;

		public void SetupLobbyHUD() {
			// In lobby/freeroam, we don't want most HUD elements
			// We just want any EXP popups, the area name display and the EXP counter

			// Hide all HUDElements
			Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_HIDE_ALL_ELEMENTS);

			// Unhide the elements we want (this event accepts both hudElementToggleData_t and hudElementToggleMultiData_t - multi accepts an array of types)
			Bird.HUDController.hudElementToggleMultiData_t dataobject = new Bird.HUDController.hudElementToggleMultiData_t();
			dataobject.m_nPlayerID = 0; // Target player ID 0 = all players (otherwise, it's 1 - 4)
			dataobject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.ENABLE;
			dataobject.m_ArrayTypes = new System.Type[] { typeof(Bird.HUD_EXP), typeof(Bird.HUD_ScorePopupMgr), typeof(Bird.HUD_Area) };

			// This will enable the exp display, timer, score popup and race position
			Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, dataobject);

			// Set up our EXP mode
			Bird.HUD_EXP.hudEXPModeData_t expdata = new Bird.HUD_EXP.hudEXPModeData_t();
			expdata.m_Mode = Bird.HUD_EXP.expToDisplay_e.HUD_EXP_PLAYER_SESSION;
			Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_EXP_MODE, expdata);
		}

        // Changes:
        // - Moved actual loadgame/returntolobby functionality to internal functions
        // - Added transition support

        protected string m_strLoadGameScene;
        public Bird.BaseTransition m_InTransition;
        public Bird.BaseTransition m_OutTransition;
		public Bird.BaseTransition m_IslandTransition;

        public void LoadGame(string gameName)
        {
			if(m_Transitioning) {
				return;
			}

            m_strLoadGameScene = gameName;
            Bird.UI_TL_LobbyMgr.s_CommandToExecute = Bird.UI_TL_LobbyMgr.lobbyMgrCommand_e.LOAD_GAMEMODE;
            if (m_OutTransition != null)
            {
                if (!m_OutTransition.TransitionActive)
                {
                    m_OutTransition.StartTransition();
					m_Transitioning = true;

				}
            }
            else {
                ____LoadGameInternal(); // If we have no transition, just load immediately
            }
        }

        public void ____LoadGameInternal()
        {
            if (!m_hasLoaded)
            {
				// EXP mode to Current
				Bird.HUD_EXP.hudEXPModeData_t expdata = new Bird.HUD_EXP.hudEXPModeData_t();
				expdata.m_Mode = Bird.HUD_EXP.expToDisplay_e.HUD_EXP_PLAYER_CURRENT;
				Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_EXP_MODE, expdata);

				ToggleLobbies(true);
                LocationZoneScript.DisableOrEnableLocationZones(false);
                Kojima.CameraManagerScript.singleton.SetupThirdPersonForAllPlayers();
                SceneManager.LoadScene(m_strLoadGameScene, LoadSceneMode.Additive);
                m_hasLoaded = true;
				m_Transitioning = false;

			}
        }

        public void ____ReturnToLobbyInternal()
        {
            LocationZoneScript.DisableOrEnableLocationZones(true);
            SceneManager.UnloadScene(m_strCurrentGame);
            ToggleLobbies(false);
            m_strCurrentGame = "";
            m_hasLoaded = false;
            Kojima.CameraManagerScript.singleton.SetupThirdPersonForAllPlayers();

            // Clear the checkpoints
            Bird.CheckpointManager.CM.ClearCheckpoints();

			SetupLobbyHUD();
			Kojima.GameController.s_singleton.AllCarsCanMove(true);
			m_Transitioning = false;
		}

        public void ReturnToLobby()
        {
			if(m_Transitioning) {
				return;
			}

            Bird.UI_TL_LobbyMgr.s_CommandToExecute = Bird.UI_TL_LobbyMgr.lobbyMgrCommand_e.RETURN_TO_LOBBY;
            if (m_InTransition != null)
            {
                if (!m_InTransition.TransitionActive)
                {
                    m_InTransition.StartTransition();
					m_Transitioning = true;

				}
            }
            else {
                ____ReturnToLobbyInternal(); // If we have no transition, just load immediately
            }
        }

        public void SetCurGame(string gameName)
        {
            m_strCurrentGame = gameName;
        }

        void ToggleLobbies(bool hide)
        {
            if (hide)
            {
                for (int i = 0; i < m_lobbies.Length; i++)
                {
                    m_lobbies[i].SetActive(false);
					AddonManager.m_instance.destroyAllAddons ();
                }
            }
            else
            {
                for (int i = 0; i < m_lobbies.Length; i++)
                {
                    m_lobbies[i].SetActive(true);
                }
            }
        }

       public void ChangeIsland(string newIslandScene, string lobbyScene)
        {
			if(m_Transitioning) {
				return;
			}

			m_strNewIslandScene = newIslandScene;
			m_strLobbySceneToLoad = lobbyScene;

			if (m_IslandTransition != null) {
				if (!m_IslandTransition.TransitionActive) {
					m_IslandTransition.StartTransition();
					m_Transitioning = true;
				}
			} else {
				___ChangeIslandInternal();
			}
		}

		string m_strNewIslandScene;
		string m_strLobbySceneToLoad;
		
		public void ___ChangeIslandInternal() {
			// We need to purge ALL current world objects before
			// loading a new island, so do that now -sam
			Kojima.GameController.PurgeGame();
			Kojima.GameController.s_eLoadMode = Kojima.GameController.loadMode_e.ISLAND_TRANSITION;

			m_strLobbyName = m_strLobbySceneToLoad;
			m_strCurIsland = m_strNewIslandScene;

			SceneManager.LoadScene(m_strNewIslandScene);
			SceneManager.LoadScene(m_strLobbySceneToLoad, LoadSceneMode.Additive);

			if (m_strNewIslandScene == "OhiyakuBay") {
				SceneManager.LoadScene("Ohiaku_LocationZones", LoadSceneMode.Additive);
				Kojima.GameController.s_eIsland = Kojima.GameController.island_e.OHIYAKU;
			} else {
				Kojima.GameController.s_eIsland = Kojima.GameController.island_e.KOJIMA;
			}
		}   
    }
}
