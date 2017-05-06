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
	public class MenuButton_TutorialScreen : MenuButton_ChangeScreen {

		public GameObject m_GameLogo;
		public bool m_ShrinkLogo = true;

		public override void OnButtonPress(BaseMenuScreen parentMenu) {
			m_GameLogo.GetComponent<RepositionLogo>().m_bShrink = m_ShrinkLogo;
			base.OnButtonPress(parentMenu);
		}
	}
}