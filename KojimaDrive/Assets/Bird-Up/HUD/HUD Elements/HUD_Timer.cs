//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Gamemode timer
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class HUD_Timer : HUDElement {
		TypogenicText m_Text;
		Material m_Mat;
		int nColID;
		void Start() {
			m_Text = GetComponent<TypogenicText>();
			Renderer rend = GetComponent<Renderer>();
			m_Mat = rend.material;
			nColID = Shader.PropertyToID("_HSVAAdjust");
		}


		public Vector4 m_HSVStart = new Vector4(0.48f, -0.04f, -0.1f, 0.03f);
		public Vector4 m_HSVEnd = new Vector4(0.21f, 0.05f, 0.19f, 0.15f);

		public override void UpdateHUDElement() {
			if(Kojima.GameModeManager.m_instance.m_currentGameMode == null) {
				if(m_Text == null) {
					m_Text = GetComponent<TypogenicText>();
					Renderer rend = GetComponent<Renderer>();
					m_Mat = rend.material;
					nColID = Shader.PropertyToID("_HSVAAdjust");
				}
				m_Text.Text = "";
				return;
			}

			// Set HSV 
			Vector4 curHSV = Vector4.Lerp(m_HSVEnd, m_HSVStart, (Kojima.GameModeManager.m_instance.m_currentGameMode.GetPhaseLength() - Kojima.GameModeManager.m_instance.m_currentGameMode.GetTimeFloat()));
			m_Mat.SetVector(nColID, curHSV);

			m_Text.Text = Kojima.GameModeManager.m_instance.m_currentGameMode.GetTime();
		}
	}
}