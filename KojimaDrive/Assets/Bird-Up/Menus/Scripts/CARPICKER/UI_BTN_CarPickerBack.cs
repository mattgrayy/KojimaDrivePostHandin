//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Act as 'car picker' for a player.
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_BTN_CarPickerBack : MenuButton_Listener {
		public UI_PlayerStartMenu m_ControllingMenu;
		public UI_BTN_CarPicker m_CarPickerScript;

		public override void OnButtonPress(BaseMenuScreen parentMenu) {
			if (m_CarPickerScript.m_ePlayerState == UI_BTN_CarPicker.playerState_e.OUT_OF_GAME) {
				// Quit back to the menu
				MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_BACK);
				m_ControllingMenu.Back();
			} else {
				MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_MOVE_UP);
				m_CarPickerScript.HandleStateChange(m_CarPickerScript.m_ePlayerState - 1);
			}
		}

		public override void OnButtonSelect(BaseMenuScreen parentMenu) { }

		public override void OnButtonDeselect(BaseMenuScreen parentMenu) { }
	}
}