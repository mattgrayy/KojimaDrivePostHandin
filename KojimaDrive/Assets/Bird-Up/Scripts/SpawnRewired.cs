//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Will spawn a Rewired Input Manager if required.
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class SpawnRewired : MonoBehaviour {
		public GameObject m_RewiredPrefab;

		private void Awake() {
			Rewired.InputManager input = FindObjectOfType<Rewired.InputManager>();

			if(input == null) {
				GameObject newMgr = Instantiate(m_RewiredPrefab);
			}
		}
	}
}