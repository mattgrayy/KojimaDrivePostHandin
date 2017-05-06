//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Unpause button
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_BTN_Unpause : MenuButton_Listener {

		public BaseTransition m_Transition;
		public override void OnButtonPress(BaseMenuScreen parentMenu) {
			//UI_PauseMenu.ForceToggleGamePause(false);
			UI_PauseMenu.ToggleGamePause(false, -1);
			if (m_Transition) {
				m_Transition.StartTransitionReverse();
			}
		}

		public override void OnButtonSelect(BaseMenuScreen parentMenu) {
			
		}

		public override void OnButtonDeselect(BaseMenuScreen parentMenu) {
			
		}
	}
}