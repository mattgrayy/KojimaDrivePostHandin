//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Install menu callbacks
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class InstallMenuCallbacks : MonoBehaviour {
		public Rewired.InputManager m_InputMgr;

		private void Start() {
			try {
				Bird.BaseMenuScreen.SetupInput();
			} catch {
				// Set up on the first update instead
			}
		}

		private void OnDestroy() {
			Bird.BaseMenuScreen.UnhookInput();
		}

		private void Update() {
			if(!Bird.BaseMenuScreen.m_bRewiredSetup) {
				Bird.BaseMenuScreen.SetupInput();
			}
		} 
	}
}