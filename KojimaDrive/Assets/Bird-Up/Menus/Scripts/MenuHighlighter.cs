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
	public class MenuHighlighter : MonoBehaviour {
		MenuButton m_SelectedButton;
		public float m_fWidthPadding = 1.0f;
		public float m_fTransitionSpeed = 20.0f;

		public void SetSelectedButton(MenuButton button) {
			m_SelectedButton = button;
		}

		void UpdateLerp() {
			Bounds bnds = m_SelectedButton.m_Text.GetComponent<Renderer>().bounds;
			Transform quadtrans = GetComponent<Transform>();
			Vector3 ls = quadtrans.localScale;
			ls.x = Mathf.Lerp(ls.x, bnds.size.x + m_fWidthPadding, Time.unscaledDeltaTime * m_fTransitionSpeed);
			quadtrans.localScale = ls;
			quadtrans.position = Vector3.Lerp(quadtrans.position, bnds.center, Time.unscaledDeltaTime * m_fTransitionSpeed);
		}

		// Update is called once per frame
		void Update() {
			// Lerp
			if (m_SelectedButton && m_SelectedButton.isActiveAndEnabled) {
				//UpdateLerp();

			}
		}
	}
}