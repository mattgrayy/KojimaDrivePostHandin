//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Manages the pause menu.
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;
namespace Bird {
	public class UI_PauseMenu : MonoBehaviour {
		static public bool s_bPauseAllowed = true;

		private void Start() {
			Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.UI_TOGGLE_PAUSE, Event_TogglePauseMenu);
			ObjectDB.DontDestroyOnLoad_Managed(gameObject);
			m_Menu.gameObject.SetActive(false);
		}

		private void OnDestroy() {
			Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.UI_TOGGLE_PAUSE, Event_TogglePauseMenu);
		}

		public BaseMenuScreen m_Menu;
		public TypogenicText m_PausedText;
		public BaseTransition m_Transition;

		private void OnEnable() {

		}

		void Event_TogglePauseMenu(object data) {
			pauseUIToggleData_t objdata = (pauseUIToggleData_t)data;
			if(objdata == null) {
				return;
			}

			bool bPaused = objdata.m_bState;
			if (bPaused) {
				m_Menu.m_nTargetPlayerID = objdata.m_nPlayerID + 1;
				m_Menu.gameObject.SetActive(true);
				m_Menu.m_ButtonsThisScreen[m_Menu.m_nCurrentlySelectedOption].m_bSelected = false;
				m_Menu.m_ButtonsThisScreen[0].m_bSelected = true;
				m_Menu.m_nCurrentlySelectedOption = 0;
				//m_PausedText.Text = "PLAYER " + objdata.m_nPlayerID.ToString() + " PAUSED";
				/*int ctrlID = Kojima.GameController.s_singleton.m_players[objdata.m_nPlayerID - 1].m_nControllerID + 1;*/ // Get our controller-based player ID
				m_PausedText.Text = "PLAYER " + (objdata.m_nPlayerID + 1).ToString() + " PAUSED";
				m_Transition.StartTransition();
				BaseMenuScreen.m_ActiveScreen = m_Menu;
			} else {
				m_Menu.gameObject.SetActive(false);
				BaseMenuScreen.m_ActiveScreen = null;
				m_Transition.StartTransitionReverse();
			}
		}


		public class pauseUIToggleData_t {
			public int m_nPlayerID;
			public bool m_bState;
		}

		public static void ForceToggleGamePause(bool bState) {
			if (bState) {
				s_fPreviousTimescale = Time.timeScale;
				Time.timeScale = s_fPausedTimescale;
			} else {
				Time.timeScale = s_fPreviousTimescale;
			}

			s_bPaused = bState;
		}

		public static void ToggleGamePause(bool bState, int nPlayerID) {
			if(!s_bPauseAllowed || Kojima.GameController.s_singleton == null) {
				return; // Pause isn't allowed here.
			}

			if(bState == s_bPaused) {
				if(!bState && nPlayerID != s_nPausedPlayerID) {
					ToggleGamePause(false, s_nPausedPlayerID); // Allow other players to disable pause
				}

				return;
			}

			if (bState) {
				s_nPausedPlayerID = nPlayerID;
				s_fPreviousTimescale = Time.timeScale;
				Time.timeScale = s_fPausedTimescale;

				pauseUIToggleData_t data = new pauseUIToggleData_t();

				Kojima.CarScript scr = Kojima.GameController.s_singleton.GetCarByControllerID(nPlayerID - 1);
				data.m_nPlayerID = scr.m_nControllerID;
				data.m_bState = true;
				Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_TOGGLE_PAUSE, data);
			} else {
				Time.timeScale = s_fPreviousTimescale;
				pauseUIToggleData_t data = new pauseUIToggleData_t();
				data.m_nPlayerID = 0; // Unpause all players
				data.m_bState = false;
				Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_TOGGLE_PAUSE, data);
			}

			s_bPaused = bState;
		}

		protected static float s_fPreviousTimescale;
		protected static float s_fPausedTimescale = 0.0f;
		protected static bool s_bPaused = false;
		protected static int s_nPausedPlayerID;

		public static bool GamePaused {
			get {
				return s_bPaused;
			}
		}

		public static int PausedPlayerID {
			get {
				return s_nPausedPlayerID;
			}
		}
	}
}