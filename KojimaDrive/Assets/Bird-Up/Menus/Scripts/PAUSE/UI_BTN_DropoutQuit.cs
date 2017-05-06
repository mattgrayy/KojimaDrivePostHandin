//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Dropout/Quit Game/Return to lobby button
// Namespace: Bird
//
//===============================================================================//

//#define DROPOUT_ENABLED // Dropin/out is currently disabled!

using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_BTN_DropoutQuit : MenuButton_Listener {
		public TypogenicText m_Text;

		public enum btnMode_e {
			RETURN_TO_MENU, // If only a single player
			DROP_OUT,		// If more than one player
			RETURN_TO_LOBBY // If in a gamemode
		}

		public btnMode_e m_eMode = btnMode_e.RETURN_TO_MENU;

		void OnEnable() {
			Update();
			UpdateText();
		}

		private void Update() {
			btnMode_e newMode;

			if (Kojima.GameModeManager.m_instance != null && Kojima.GameModeManager.m_instance.m_currentMode != Kojima.GameModeManager.GameModeState.FREEROAM) {
				// No quitters in gamemodes; it'll screw things up
				newMode = btnMode_e.RETURN_TO_LOBBY;
			} else {
				// If we're in freeroam, allow dropping out
#if DROPOUT_ENABLED
				if (Kojima.GameController.s_ncurrentPlayers > 1) {
					newMode = btnMode_e.DROP_OUT;
				} else
#endif
				{
					newMode = btnMode_e.RETURN_TO_MENU;
				}
			}

			if(newMode != m_eMode) {
				UpdateText();
				m_eMode = newMode;
			}
		}
		public BaseTransition m_EndGameTransition;

		private void UpdateText() {
			switch(m_eMode) {
				case btnMode_e.RETURN_TO_MENU:
					m_Text.Text = "QUIT GAME";
					break;
				case btnMode_e.DROP_OUT:
					m_Text.Text = "DROP OUT";
					break;
				case btnMode_e.RETURN_TO_LOBBY:
					m_Text.Text = "RETURN TO LOBBY";
					break;
				default:
					m_Text.Text = "NULL";
					break;

			}
		}

		public override void OnButtonPress(BaseMenuScreen parentMenu) {
			switch (m_eMode) {
				case btnMode_e.RETURN_TO_MENU:
					UI_PauseMenu.ToggleGamePause(false, -1);
					m_EndGameTransition.StartTransition();
					break;
				case btnMode_e.DROP_OUT:
					UI_PauseMenu.ToggleGamePause(false, -1);
					Kojima.GameController.s_singleton.m_players[parentMenu.m_nTargetPlayerID - 1].DropOut();
					break;
				case btnMode_e.RETURN_TO_LOBBY:
					UI_PauseMenu.ToggleGamePause(false, -1);
					Kojima.GameModeManager.m_instance.m_currentGameMode.EndGame();
					break;
				default:
					break;
			}
		}

		public override void OnButtonSelect(BaseMenuScreen parentMenu) {

		}

		public override void OnButtonDeselect(BaseMenuScreen parentMenu) {

		}
	}
}