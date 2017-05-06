//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Dropout/Quit Game button
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;
namespace Bird {
	[System.Obsolete]
	public class UI_BTN_QuitGamemode : MenuButton_Listener {
		
		// Update is called once per frame
		void Update() {
			m_bEnabled = Kojima.GameModeManager.m_instance.m_currentMode != Kojima.GameModeManager.GameModeState.FREEROAM;
		}

		public override void OnButtonPress(BaseMenuScreen parentMenu) {
			UI_PauseMenu.ToggleGamePause(false, -1);
			Kojima.GameModeManager.m_instance.m_currentGameMode.EndGame();
		}

		public override void OnButtonSelect(BaseMenuScreen parentMenu) {
			
		}

		public override void OnButtonDeselect(BaseMenuScreen parentMenu) {
			
		}
	}
}