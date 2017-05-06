//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Generic text HUD element
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	[System.Obsolete]
	public class HUD_GenericText : HUDElement {
		public bool m_bTemporaryText = false; // Should this be deleted when hiding?
		protected float m_fFadePosition;
		public float m_fFadeSpeed = 1.0f;
		protected float m_fCurrentFadeSpeed;

		public TypogenicText m_Text;
		Material m_Material;
		int m_nColID = -1;

		public void Init(float fFadeSpeed, string strText) {
			m_Text.Text = strText;
		}

		void Start() {
			m_Material = m_Text.GetComponent<Renderer>().material; // Get a material instance
			m_fCurrentFadeSpeed = m_fFadeSpeed;
		}


		void UpdateFade() {

		}

		public override void UpdateHUDElement() {

		}
	}
}