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
	public class UI_BTN_StartKJTransition : MenuButton_Listener {
		public BaseTransition m_TransitionToFire;
		public bool m_bReverseTransition = false;
		public override void OnButtonPress(BaseMenuScreen parentMenu) {
			if(m_TransitionToFire) {
				if (m_bReverseTransition) {
					m_TransitionToFire.StartTransitionReverse();
				} else {
					m_TransitionToFire.StartTransition();
				}
			}
		}

		public override void OnButtonSelect(BaseMenuScreen parentMenu) { }

		public override void OnButtonDeselect(BaseMenuScreen parentMenu) { }

		public override void OnButtonLeft(BaseMenuScreen parentMenu) { }

		public override void OnButtonRight(BaseMenuScreen parentMenu) { }
	}
}