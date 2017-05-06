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
	public class MenuButton : MonoBehaviour {

		public TypogenicText m_Text;
		public MenuButton_Listener m_EventListener;

		public BaseMenuScreen m_ParentMenu;

		public TypogenicText m_ButtonBackground;
		private Color m_InitColourBackground;
		public Color m_BackgroundColourHighlighted;

		public Color m_OutlineColourHighlighted;
		private Color m_InitColour;

		[TextArea]
		public string m_ButtonHint = "";

		private bool m_bSelectedLastFrame = false;
		public bool m_bSelected = false;
		float m_fLerp = 0.0f;

		public float m_fSpeed = 1.0f;

		// Use this for initialization
		void Start() {
			m_InitColour = m_Text.ColorTopLeft;
			if (m_ButtonBackground) {
				m_InitColourBackground = m_ButtonBackground.ColorTopLeft;
			}

			if (m_EventListener != null) {
				m_EventListener.m_ParentButton = this;
			}
		}



		// Update is called once per frame
		void Update() {
			if (m_bSelectedLastFrame != m_bSelected) {
				m_fLerp = 0.0f;
			}
			m_bSelectedLastFrame = m_bSelected;

			if (m_bSelected) {
				Color col = m_Text.ColorTopLeft;
				col = Color.Lerp(col, m_OutlineColourHighlighted, m_fLerp);
				m_Text.ColorTopLeft = col;

				if (m_ButtonBackground) {
					Color backcol = m_ButtonBackground.ColorTopLeft;
					backcol = Color.Lerp(m_InitColourBackground, m_BackgroundColourHighlighted, m_fLerp);
					m_ButtonBackground.ColorTopLeft = backcol;
				}
				UpdateLerpClock();
			} else {
				Color col = m_Text.ColorTopLeft;
				col = Color.Lerp(col, m_InitColour, m_fLerp);
				m_Text.ColorTopLeft = col;

				if (m_ButtonBackground) {
					Color backcol = m_ButtonBackground.ColorTopLeft;
					backcol = Color.Lerp(m_BackgroundColourHighlighted, m_InitColourBackground, m_fLerp);
					m_ButtonBackground.ColorTopLeft = backcol;
				}

				UpdateLerpClock();
			}
		}

		void UpdateLerpClock() {
			// Update lerp clock
			m_fLerp += (Time.unscaledDeltaTime * m_fSpeed);
		}
	}
}