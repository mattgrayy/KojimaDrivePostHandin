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
	public class MenuButton_ChangeScreen : MenuButton_Listener {

		public bool m_bChangeState = false;

		public GameObject m_NextScreen;
		public override void OnButtonPress(BaseMenuScreen parentMenu) {
			if (m_NextScreen) {
				parentMenu.gameObject.SetActive(false);
				m_NextScreen.SetActive(true);
			}
		}
		public override void OnButtonSelect(BaseMenuScreen parentMenu) {
			// Nothing
		}
		public override void OnButtonDeselect(BaseMenuScreen parentMenu) {
			// Nothing
		}
	}
}