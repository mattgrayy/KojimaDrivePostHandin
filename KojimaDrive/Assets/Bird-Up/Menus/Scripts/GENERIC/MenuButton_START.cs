//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Menu UI
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class MenuButton_START : MenuButton_Listener {
		public BaseTransition m_TransitionToPlay;

		public override void OnButtonPress(BaseMenuScreen parentMenu) {
			m_TransitionToPlay.StartTransition();
		}

		public override void OnButtonSelect(BaseMenuScreen parentMenu) {
			// Nothing
		}
		public override void OnButtonDeselect(BaseMenuScreen parentMenu) {
			// Nothing
		}
	}
}