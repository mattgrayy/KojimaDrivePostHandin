//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Crazy Taxi style score indication
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class HUD_ScorePopup : HUDElement {
		float m_fStartPos = 0.0f;
		public float m_fDestPos = 10.0f;

		public Vector3 m_StartPos;

		public TypogenicText m_reasonText;
		Material m_reasonMat;
		public TypogenicText m_scoreText;
		Material m_scoreMat;
		int m_nColID;

		bool m_bStarted = false;

		void Start() {
			m_fStartPos = transform.localPosition.y;
			m_scoreText.gameObject.SetActive(false);
			m_scoreMat = m_scoreText.GetComponent<Renderer>().material;

			m_reasonMat = m_reasonText.GetComponent<Renderer>().material;
			m_nColID = Shader.PropertyToID("_HSVAAdjust");

			m_reasonText.gameObject.layer = gameObject.layer;
			m_bStarted = true;
		}

		public void ResetAndHide() {
			Vector3 local = transform.localPosition;
			local.y = m_fStartPos;
			transform.localPosition = local;


			m_bDisplay = false;
		}

		Vector4 m_defaultHSV = new Vector4(0.36f, -0.13f, 0f, -0.02f);
		Vector4 m_defaultHSVNegative = new Vector4(0.22f, -0.03f, 0.19f, 0.15f);
		public void Popup(string reason, int score) {
			if (score >= 0) {
				Popup(reason, score, m_defaultHSV);
			} else {
				Popup(reason, score, m_defaultHSVNegative);
			}
		}

		public float m_fCreationTime = 0.0f;
		public void Popup(string reason, int score, Vector4 hsv) {
			if(!m_bStarted) {
				Start();
			}
				

			m_fCreationTime = Time.time;

			if (score > 0) {
				m_scoreText.Text = "+" + score.ToString();
			} else {
				m_scoreText.Text = /*"-" +*/ score.ToString();
			}

			if(reason == null) {
				if (score > 0) {
					m_reasonText.Text = "GET!";
				} else {
					m_reasonText.Text = "GONE!";
				}
			} else {
				m_reasonText.Text = reason;
			}

			m_bDisplay = true;
			m_fLerpPos = 0.0f;

			m_scoreMat.SetVector(m_nColID, hsv);
			m_reasonMat.SetVector(m_nColID, hsv);
		}

		float m_fLerpPos = 0.0f;
		public float m_fSpeed = 4.0f;
		public float m_fHoldLerp = 2.0f;
		public override void UpdateHUDElement() {
			m_fLerpPos += Time.deltaTime * m_fSpeed;

			/*if (m_fLerpPos < 1.0f) */ {
				Vector3 local = transform.localPosition;
				local.y = Mathf.Lerp(m_fStartPos, m_fDestPos, Mathf.Clamp(m_fLerpPos, 0, 1));
				transform.localPosition = local;
			}

			if (m_fLerpPos >= m_fHoldLerp) {
				m_bDisplay = false;
				ResetAndHide();
			}
		}
	}
}