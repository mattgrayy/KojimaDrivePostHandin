//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Menus
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_FlashStartButton : MonoBehaviour {
		public TypogenicText m_Text;
		Material m_MatInst;
		float m_fNextChangeTime;
		public float m_fFlashTime = 1.0f;
		float m_fStandardAlpha;
		int nColID;



		private void Start() {
			m_fNextChangeTime = Time.realtimeSinceStartup + (m_fFlashTime);

			m_MatInst = m_Text.GetComponent<Renderer>().material;
			nColID = Shader.PropertyToID("_GlobalMultiplierColor");
			m_fStandardAlpha = m_MatInst.GetColor(nColID).a;
		} 

		private void Update() {
			if (m_fNextChangeTime <= Time.realtimeSinceStartup) {
				Color col = m_MatInst.GetColor(nColID);
				col.a = (col.a == m_fStandardAlpha ? 0.0f : m_fStandardAlpha);
				m_MatInst.SetColor(nColID, col);
				m_fNextChangeTime = Time.realtimeSinceStartup + m_fFlashTime;
				m_MatInst.EnableKeyword("GLOBAL_MULTIPLIER_ON");
			}
		} 
	}
}