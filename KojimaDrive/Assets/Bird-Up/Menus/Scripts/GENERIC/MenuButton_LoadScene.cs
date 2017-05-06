//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Menu UI
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Bird {
	public class MenuButton_LoadScene : MenuButton_ChangeScreen {

		public float m_fFadeSpeed = 1.0f;
		public string m_strSceneToLoad;
		bool m_bButtonPressed = false;
		BaseMenuScreen parent;

		public TypogenicText[] m_FadeableFonts;
		public Renderer m_QuadFade;

		public BaseTransition m_TransitionPrefab;

		public override void OnButtonPress(BaseMenuScreen parentMenu) {
			m_bButtonPressed = true;
			BaseMenuScreen.m_bInputLocked = true;
			parent = parentMenu;

			
		}

		public override void OnButtonSelect(BaseMenuScreen parentMenu) { }

		public override void OnButtonDeselect(BaseMenuScreen parentMenu) { }
	}
}