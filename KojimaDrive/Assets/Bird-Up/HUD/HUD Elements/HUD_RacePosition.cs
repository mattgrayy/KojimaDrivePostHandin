//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Editor script for the Screenspace UI objects.
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class HUD_RacePosition : HUDElement {
		public TypogenicText m_Number;
		public TypogenicText m_Letters;
		Material m_NumMat;
		Material m_LettersMat;
		int nColID;
		int nSatID;
		int nConID;
		int nBriID;

		// Having to do this as I can't find an easier access route! :C
		public Kojima.RaceScript m_RaceScript = null;

		void Start() {
			m_NumMat = m_Number.GetComponent<Renderer>().material;
			m_LettersMat = m_Letters.GetComponent<Renderer>().material;
			nColID = Shader.PropertyToID("_HSVAAdjust");
			nSatID = Shader.PropertyToID("_SaturateAmount");
			nConID = Shader.PropertyToID("_ContrastAmount");
			nBriID = Shader.PropertyToID("_BrightnessAmount");

			m_Letters.gameObject.layer = gameObject.layer;
		}

		[System.Serializable]
		public struct positionSettings_s {
			public positionSettings_s(string suffix, Vector4 hsv, float sat, float bri, float cont) {
				m_strNumericalSuffix = suffix;
				m_HSV = hsv;
				m_Saturation = sat;
				m_Contrast = cont;
				m_Brightness = bri;
			}
			public string m_strNumericalSuffix;
			public Vector4 m_HSV;
			public float m_Saturation, m_Contrast, m_Brightness;
		}

		positionSettings_s[] m_PositionSettings = new positionSettings_s[] {
			new positionSettings_s("st", new Vector4(0.34f, 0f, 0f, 0f), 0.0f, 0.1f, 0.0f),			// 1st  - Gold
			new positionSettings_s("nd", new Vector4(0.34f, 0f, 0f, 0f), -1.0f, 0.19f, 0.21f),		// 2nd  - Silver
			new positionSettings_s("rd", new Vector4(0.27f, 0f, 0f, 0f), -0.08f, 0.05f, -0.15f),	// 3rd  - Bronze
			new positionSettings_s("th", new Vector4(0.23f, 0f, 0f, 0f), -0.12f, 0.03f, 0.05f),		// 4th+ - Red
		};

		int m_nPositionLastUpdate = 0;
		//public int m_nTempPosition = 1;
		public override void UpdateHUDElement() {
			//int nPosition = m_nTempPosition;
			if(m_RaceScript == null) {
				return;
			}

			int nPosition = m_RaceScript.GetPlayerPosition(m_ParentController.m_nPlayer - 1);

			if (m_nPositionLastUpdate != nPosition) {
				int nAccessPos = nPosition - 1;
				if (nPosition >= m_PositionSettings.Length) {
					nAccessPos = m_PositionSettings.Length - 1;
				}

				// Update our text
				m_Letters.Text = m_PositionSettings[nAccessPos].m_strNumericalSuffix;
				m_Number.Text = nPosition.ToString();

				// Update our materials
				m_LettersMat.SetVector(nColID, m_PositionSettings[nAccessPos].m_HSV);
				m_NumMat.SetVector(nColID, m_PositionSettings[nAccessPos].m_HSV);

				m_LettersMat.SetFloat(nSatID, m_PositionSettings[nAccessPos].m_Saturation);
				m_NumMat.SetFloat(nSatID, m_PositionSettings[nAccessPos].m_Saturation);

				m_LettersMat.SetFloat(nConID, m_PositionSettings[nAccessPos].m_Contrast);
				m_NumMat.SetFloat(nConID, m_PositionSettings[nAccessPos].m_Contrast);

				m_LettersMat.SetFloat(nBriID, m_PositionSettings[nAccessPos].m_Brightness);
				m_NumMat.SetFloat(nBriID, m_PositionSettings[nAccessPos].m_Brightness);
			}

			m_nPositionLastUpdate = nPosition;
		}
	}
}