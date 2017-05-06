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
	public class RepositionLogo : MonoBehaviour {

		Vector3 m_vecStandardSize;
		Vector3 m_vecStandardPosition;
		public Vector3 m_vecSmallSize;
		public Vector3 m_vecSmallPosition;

		public float m_fTransitionSpeed = 1.0f;

		public bool m_bShrink = false;

		// Use this for initialization
		void Start() {
			m_vecStandardSize = gameObject.transform.localScale;
			m_vecStandardPosition = gameObject.transform.localPosition;
		}

		// Update is called once per frame
		void Update() {
			if (m_bShrink) {
				Vector3 scale = gameObject.transform.localScale;
				scale = Vector3.Lerp(scale, m_vecSmallSize, Time.deltaTime * m_fTransitionSpeed);
				gameObject.transform.localScale = scale;


				Vector3 pos = gameObject.transform.localPosition;
				pos = Vector3.Lerp(pos, m_vecSmallPosition, Time.deltaTime * m_fTransitionSpeed);
				gameObject.transform.localPosition = pos;
			} else {
				Vector3 scale = gameObject.transform.localScale;
				scale = Vector3.Lerp(scale, m_vecStandardSize, Time.deltaTime * m_fTransitionSpeed);
				gameObject.transform.localScale = scale;


				Vector3 pos = gameObject.transform.localPosition;
				pos = Vector3.Lerp(pos, m_vecStandardPosition, Time.deltaTime * m_fTransitionSpeed);
				gameObject.transform.localPosition = pos;
			}
		}
	}
}