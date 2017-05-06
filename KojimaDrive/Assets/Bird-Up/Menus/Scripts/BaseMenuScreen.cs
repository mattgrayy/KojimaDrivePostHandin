//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Menu UI
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;
using Rewired;
namespace Bird {
	public class BaseMenuScreen : MonoBehaviour {
		public bool m_bAutomaticallySetAsActive = true;
		public MenuHighlighter m_HighlighterQuad;
		public MenuButton[] m_ButtonsThisScreen;
		public MenuButton_Listener m_BackListener;
		public TypogenicText m_HintText;
		public int m_nCurrentlySelectedOption;
		public MenuTransition_Listener[] m_TransitionListeners;

		public void FireTransitionListeners(BaseMenuScreen prevMenu, bool bBack = false) {
			foreach (MenuTransition_Listener mtl in m_TransitionListeners) {
				mtl.OnTransition(prevMenu, bBack);
			}
		}

		// Use this for initialization
		void Start() {
			if (m_ButtonsThisScreen.Length <= 0 || m_ButtonsThisScreen == null) {
				m_nCurrentlySelectedOption = -1;
			} else {
				m_nCurrentlySelectedOption = 0;
				m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_bSelected = true;
			}

			foreach (MenuButton butt in m_ButtonsThisScreen) {
				butt.m_ParentMenu = this;
			}

			SetupInput();
		}

		private void OnEnable() {
			if (m_bAutomaticallySetAsActive) {
				m_ActiveScreen = this;
			}
		}

		// Update is called once per frame
		void Update() {
			ProcessInput();
			UpdateMenuQuad();
		}

		void UpdateMenuQuad() {
			if (m_bInputLocked) {
				//m_HighlighterQuad.gameObject.SetActive(false);
			} else {
				if (m_HighlighterQuad) {
					m_HighlighterQuad.gameObject.SetActive(true);
				}
			}

			if (m_nCurrentlySelectedOption >= 0 && m_nCurrentlySelectedOption < m_ButtonsThisScreen.Length) {
				if (m_HighlighterQuad) {
					m_HighlighterQuad.SetSelectedButton(m_ButtonsThisScreen[m_nCurrentlySelectedOption]);
				}
				if (m_HintText != null) {
					m_HintText.Text = m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_ButtonHint;
				}
			}
		}

		static public float m_fInputCooldown = 0.05f; // Reducing this, since we're using Rewired Callbacks now
		static float m_fInputCooldownTimer = 0.0f;

		static public bool m_bInputLocked = false;

		public int m_nTargetPlayerID = 0; // 0 = allow all players

		static public bool m_bRewiredSetup = false;
		static System.Collections.Generic.List<Rewired.Player> m_RewiredPlayers = new System.Collections.Generic.List<Rewired.Player>();
		static public BaseMenuScreen m_ActiveScreen = null;

		public static void SetupInput() {
			if (m_bRewiredSetup) {
				return;
			}

			for (int i = 0; i < Kojima.GameController.s_nMaxPlayers; i++) {
				//m_RewiredPlayers.Add(Rewired.ReInput.players.GetPlayer(i));
				Rewired.Player plyr = Rewired.ReInput.players.GetPlayer(i);
				plyr.AddInputEventDelegate(Callback_Input_Select, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Select");
				plyr.AddInputEventDelegate(Callback_Input_Cancel, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Back");
				plyr.AddInputEventDelegate(Callback_Input_Pause, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Pause");
				plyr.AddInputEventDelegate(Callback_Input_Up, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Vertical");
				plyr.AddInputEventDelegate(Callback_Input_Down, UpdateLoopType.Update, InputActionEventType.NegativeButtonJustReleased, "Vertical");
				plyr.AddInputEventDelegate(Callback_Input_Right, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Horizontal");
				plyr.AddInputEventDelegate(Callback_Input_Left, UpdateLoopType.Update, InputActionEventType.NegativeButtonJustReleased, "Horizontal");
			}

			m_bRewiredSetup = true;
		}

		public static void UnhookInput() {
			if(!m_bRewiredSetup) {
				return;
			}

			for (int i = 0; i < Kojima.GameController.s_nMaxPlayers; i++) {
				if(Rewired.ReInput.players == null) {
					break; // Just don't bother if we've got no manager to call home
				}

				Rewired.Player plyr = Rewired.ReInput.players.GetPlayer(i);
				if (plyr != null) {
					try {
						Rewired.ReInput.players.GetPlayer(i).RemoveInputEventDelegate(Callback_Input_Select, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Select");
						Rewired.ReInput.players.GetPlayer(i).RemoveInputEventDelegate(Callback_Input_Cancel, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Back");
						Rewired.ReInput.players.GetPlayer(i).RemoveInputEventDelegate(Callback_Input_Pause, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Pause");
						Rewired.ReInput.players.GetPlayer(i).RemoveInputEventDelegate(Callback_Input_Up, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Vertical");
						Rewired.ReInput.players.GetPlayer(i).RemoveInputEventDelegate(Callback_Input_Down, UpdateLoopType.Update, InputActionEventType.NegativeButtonJustReleased, "Vertical");
						Rewired.ReInput.players.GetPlayer(i).RemoveInputEventDelegate(Callback_Input_Right, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Horizontal");
						Rewired.ReInput.players.GetPlayer(i).RemoveInputEventDelegate(Callback_Input_Left, UpdateLoopType.Update, InputActionEventType.NegativeButtonJustReleased, "Horizontal");
					} catch {
						// We have no callbacks, so we should be fine, just continue
					}
				}
			}


			m_RewiredPlayers.Clear();

			m_bRewiredSetup = false;
		}

		public bool m_bAllowInputWhileInactive = false;
		bool CheckInputAllowed() {
			return !m_bInputLocked && m_fInputCooldownTimer < Time.realtimeSinceStartup && (m_bAllowInputWhileInactive ? true : gameObject.activeInHierarchy);
		}

		static bool CheckActiveScreen(Rewired.InputActionEventData data) {
			return m_ActiveScreen && (m_ActiveScreen.m_nTargetPlayerID == data.playerId + 1 || m_ActiveScreen.m_nTargetPlayerID == 0);
		}

		static void Callback_Input_Pause(Rewired.InputActionEventData data) {
			if (!m_bInputLocked && m_fInputCooldownTimer < Time.realtimeSinceStartup) {
				// Find the target player
				int nTargetPlayer = data.playerId + 1;
				UI_PauseMenu.ToggleGamePause(!UI_PauseMenu.GamePaused, nTargetPlayer);
			}
		}

		// Callbacks
		static void Callback_Input_Up(Rewired.InputActionEventData data) {
			if (CheckActiveScreen(data)) {
				m_ActiveScreen.Up(data.playerId + 1);
			}
		}

		static void Callback_Input_Down(Rewired.InputActionEventData data) {
			if (CheckActiveScreen(data)) {
				m_ActiveScreen.Down(data.playerId + 1);
			}
		}

		public virtual void Up(int nPlayerID) {
			if (CheckInputAllowed()) {
				int nPrevious = m_nCurrentlySelectedOption;
				m_nCurrentlySelectedOption = Mathf.Clamp(m_nCurrentlySelectedOption - 1, 0, m_ButtonsThisScreen.Length - 1);

				if (nPrevious == m_nCurrentlySelectedOption || m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener == null) {
					return; // Don't bother continuing if we're not actually moving!
				}

				if (!m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener.m_bEnabled) {
					m_nCurrentlySelectedOption = nPrevious;
				}

				if (m_ButtonsThisScreen[nPrevious].m_EventListener != null) {
					m_ButtonsThisScreen[nPrevious].m_EventListener.OnButtonDeselect(this);
					m_ButtonsThisScreen[nPrevious].m_bSelected = false;
				}

				if (m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener != null) {
					m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener.OnButtonSelect(this);
				}

				m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_bSelected = true;

				if (m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener == null || !m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener.m_bOverrideSounds) {
					MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_MOVE_UP);
				}

				m_fInputCooldownTimer = Time.realtimeSinceStartup + (m_fInputCooldown);
			}
		}

		public virtual void Down(int nPlayerID) {
			if (CheckInputAllowed()) {
				int nPrevious = m_nCurrentlySelectedOption;
				m_nCurrentlySelectedOption = Mathf.Clamp(m_nCurrentlySelectedOption + 1, 0, m_ButtonsThisScreen.Length - 1);

				if (nPrevious == m_nCurrentlySelectedOption || m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener == null) {
					return; // Don't bother continuing if we're not actually moving!
				}

				if(!m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener.m_bEnabled) {
					m_nCurrentlySelectedOption = nPrevious;
				}

				if (m_ButtonsThisScreen[nPrevious].m_EventListener != null) {
					m_ButtonsThisScreen[nPrevious].m_EventListener.OnButtonDeselect(this);
					m_ButtonsThisScreen[nPrevious].m_bSelected = false;
				}

				if (m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener != null) {
					m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener.OnButtonSelect(this);
				}

				m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_bSelected = true;

				if (m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener == null || !m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener.m_bOverrideSounds) {
					MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_MOVE_DOWN);
				}

				m_fInputCooldownTimer = Time.realtimeSinceStartup + (m_fInputCooldown);
			}
		}

		static void Callback_Input_Right(Rewired.InputActionEventData data) {
			if (CheckActiveScreen(data)) {
				m_ActiveScreen.Right(data.playerId + 1);
			}
		}

		static void Callback_Input_Left(Rewired.InputActionEventData data) {
			if (CheckActiveScreen(data)) {
				m_ActiveScreen.Left(data.playerId + 1);
			}
		}

		public virtual void Left(int nPlayerID) {
			if (CheckInputAllowed()) {
				if (m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener != null) {
					m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener.OnButtonLeft(this);
					m_fInputCooldownTimer = Time.realtimeSinceStartup + (m_fInputCooldown);
				}
			}
		}

		public virtual void Right(int nPlayerID) {
			if (CheckInputAllowed()) {
				if (m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener != null) {
					m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener.OnButtonRight(this);
					m_fInputCooldownTimer = Time.realtimeSinceStartup + (m_fInputCooldown);
				}
			}
		}

		static void Callback_Input_Select(Rewired.InputActionEventData data) {
			if (CheckActiveScreen(data)) {
				m_ActiveScreen.Select(data.playerId + 1);
			}
		}

		public virtual void Select(int nPlayerID) {
			if (CheckInputAllowed()) {
				if (m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener != null) {
					m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener.OnButtonPress(this);

					if (!m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener.m_bOverrideSounds) {
						MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_SELECT);
					}
				} else {
					MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_ERROR);
				}
				m_fInputCooldownTimer = Time.realtimeSinceStartup + (m_fInputCooldown);
			}
		}

		static void Callback_Input_Cancel(Rewired.InputActionEventData data) {
			if (CheckActiveScreen(data)) {
				m_ActiveScreen.Cancel(data.playerId + 1);
			}
		}

		public virtual void Cancel(int nPlayerID) {
			if (CheckInputAllowed() && m_BackListener != null) {
				m_BackListener.OnButtonPress(this);
				if (m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener == null || !m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener.m_bOverrideSounds) {
					MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_BACK);
				}
				m_fInputCooldownTimer = Time.realtimeSinceStartup + (m_fInputCooldown);
			}
		}

		void ProcessInput() {
			/*if (m_bInputLocked) {
				return;
			}
			
			if (m_fInputCooldownTimer < Time.realtimeSinceStartup) {

				if (Input.GetAxis("Vertical1") < 0) {
					int nPrevious = m_nCurrentlySelectedOption;
					m_nCurrentlySelectedOption = Mathf.Clamp(m_nCurrentlySelectedOption + 1, 0, m_ButtonsThisScreen.Length - 1);

					if (nPrevious == m_nCurrentlySelectedOption) {
						return; // Don't bother continuing if we're not actually moving!
					}

					m_ButtonsThisScreen[nPrevious].m_EventListener.OnButtonDeselect(this);
					m_ButtonsThisScreen[nPrevious].m_bSelected = false;

					if (m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener != null) {
						m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener.OnButtonSelect(this);
					}

					m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_bSelected = true;

					MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_MOVE);

					m_fInputCooldownTimer = Time.realtimeSinceStartup + (m_fInputCooldown / ** Time.timeScale* /);
				}

				if (Input.GetAxis("Horizontal1") < 0) {
					// Left
					m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener.OnButtonLeft(this);
					m_fInputCooldownTimer = Time.realtimeSinceStartup + (m_fInputCooldown / ** Time.timeScale* /);
				}

				if (Input.GetAxis("Horizontal1") > 0) {
					// Right
					m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener.OnButtonRight(this);
					m_fInputCooldownTimer = Time.realtimeSinceStartup + (m_fInputCooldown / ** Time.timeScale* /);
				}

				if (Input.GetButtonDown("Submit")) {
					if (m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener != null) {
						m_ButtonsThisScreen[m_nCurrentlySelectedOption].m_EventListener.OnButtonPress(this);
					}

					MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_SELECT);

					m_fInputCooldownTimer = Time.realtimeSinceStartup + (m_fInputCooldown / ** Time.timeScale* /);
				}

				if (m_BackListener != null && Input.GetButtonDown("Cancel")) {
					m_BackListener.OnButtonPress(this);

					MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_BACK);

					m_fInputCooldownTimer = Time.realtimeSinceStartup + (m_fInputCooldown / ** Time.timeScale* /);
				}
			}*/
		}
	}
}