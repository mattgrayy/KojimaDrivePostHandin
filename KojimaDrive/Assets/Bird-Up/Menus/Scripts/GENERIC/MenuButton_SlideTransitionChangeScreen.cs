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
	public class MenuButton_SlideTransitionChangeScreen : MenuButton_Listener {

		public bool m_bChangeState = false;

		public GameObject m_NextScreen;

		private BaseMenuScreen m_ParentMenu;

		public MenuButtonSlideInTransition m_TransitionController;

		public float m_fDelay = 0.0f;
		public bool m_bBack = false;

		public string m_VoiceClipToPlayOnPress = "";


		public override void OnButtonPress(BaseMenuScreen parentMenu) {
			m_ParentMenu = parentMenu;
			m_TransitionController.TransitionOut(this, false, m_fDelay);
			BaseMenuScreen.m_bInputLocked = true;
		}

		public void ChangeScreen() {
			if (m_NextScreen) {
				m_ParentMenu.gameObject.SetActive(false);
				m_NextScreen.SetActive(true);
				BaseMenuScreen menuto = m_NextScreen.GetComponent<BaseMenuScreen>();
				if (menuto) {
					menuto.FireTransitionListeners(m_ParentMenu, m_bBack);
				}
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