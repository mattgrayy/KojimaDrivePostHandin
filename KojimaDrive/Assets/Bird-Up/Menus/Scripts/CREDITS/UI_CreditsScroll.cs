//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Credits scroller
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_CreditsScroll : MonoBehaviour {
		public Vector3 m_fMovementSpeed = Vector3.zero;
		public Vector3 m_vecTargetPosition = Vector3.zero;
		public BaseTransition m_EndTransition;

		// Update is called once per frame
		void Update() {
			transform.localPosition += m_fMovementSpeed * Time.deltaTime;
			if(transform.localPosition.x >= m_vecTargetPosition.x &&
				transform.localPosition.y >= m_vecTargetPosition.y &&
				transform.localPosition.z >= m_vecTargetPosition.z) {
				if (m_EndTransition != null && !m_EndTransition.TransitionActive) {
					m_EndTransition.StartTransition();
				}
			}
		}
	}
}