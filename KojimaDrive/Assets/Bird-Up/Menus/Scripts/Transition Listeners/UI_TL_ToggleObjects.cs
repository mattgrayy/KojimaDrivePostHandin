//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Toggles objects
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_TL_ToggleObjects : UI_Transition_Listener {
		public GameObject[] m_GOsToDisable;
		public GameObject[] m_GOsToEnable;

		public override void TransitionCompleted() {
			if (m_GOsToDisable != null) {
				for (int i = 0; i < m_GOsToDisable.Length; i++) {
					if(m_GOsToDisable[i] != null) {
						m_GOsToDisable[i].SetActive(false);
					}
				}
			}

			if (m_GOsToEnable != null) {
				for (int i = 0; i < m_GOsToEnable.Length; i++) {
					if (m_GOsToEnable[i] != null) {
						m_GOsToEnable[i].SetActive(true);
					}
				}
			}
		}

		public override void TransitionInterrupted() {
			
		}

		public override void TransitionStarted() {
			
		}
	}
}