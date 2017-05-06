//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Menus
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_BTN_GameStart : UI_BTN_StartKJTransition {
		public UI_PlayerStartMenu.stage_e m_eStage = UI_PlayerStartMenu.stage_e.KOJIMA;
		public UI_PlayerStartMenu m_StartMenu;

		public override void OnButtonPress(BaseMenuScreen parentMenu) {
			m_StartMenu.m_eStage = m_eStage;
			base.OnButtonPress(parentMenu);
		}

	}
}