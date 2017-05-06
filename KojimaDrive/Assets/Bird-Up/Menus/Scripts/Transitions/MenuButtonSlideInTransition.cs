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
	public class MenuButtonSlideInTransition : MenuTransition_Listener {

		public Vector3[] m_startPosOffset;
		public GameObject[] m_targetGOs;
		private Vector3[] m_targetStartPos;
		private Vector3[] m_targetEndPos;

		public float[] m_fDelay;
		public float[] m_fSpeed;
		private float[] m_fCurLerp;
		private float[] m_fStartTime;
		public bool m_bPerformTransition = false;
		public bool m_bOffsetNotPosition = true;
		public float m_fBackDelay = -0.4f;

		[Header("Transitioning out")]
		public MenuButton_SlideTransitionChangeScreen m_ButtonToInform;
		public bool m_bLerpOut = false;

		public bool m_bAutoStart = false;
		public float m_fAutoStartDelay = 0.0f;
		float m_fExtraDelay = 0.0f;
		void Start() {
			Setup();

			if(m_bAutoStart) {
				m_fExtraDelay = m_fAutoStartDelay;
				OnTransition(null);
				m_fExtraDelay = 0.0f;
			}
		}

		bool m_bSetup = false;
		void Setup() {
			if (!m_bSetup) {
				m_bLerpOut = false;
				m_fCurLerp = new float[m_targetGOs.Length];
				m_fStartTime = new float[m_targetGOs.Length];
				m_targetEndPos = new Vector3[m_targetGOs.Length];
				m_targetStartPos = new Vector3[m_targetGOs.Length];
				for (int i = 0; i < m_targetGOs.Length; i++) {
					m_targetEndPos[i] = m_targetGOs[i].transform.localPosition;
					if (m_bOffsetNotPosition) {
						m_targetStartPos[i] = m_targetEndPos[i] + m_startPosOffset[i];
					} else {
						m_targetStartPos[i] = m_startPosOffset[i];
					}
					m_fCurLerp[i] = 0.0f;
					m_fStartTime[i] = 0.0f;
				}

				SetInitialPosition();
				m_bSetup = true;
			}
		}

		void SetInitialPosition() {
			for (int i = 0; i < m_targetGOs.Length; i++) {
				m_targetGOs[i].transform.localPosition = m_targetStartPos[i];
				m_fCurLerp[i] = 0.0f;
			}
		}

		void UpdatePositions() {
			for (int i = 0; i < m_targetGOs.Length; i++) {
				if (m_bLerpOut) {
					if (UpdateLerp(i)) {
						m_targetGOs[i].transform.localPosition = Vector3.Lerp(m_targetEndPos[i], m_targetStartPos[i], m_fCurLerp[i]);
					}
				} else {
					if (UpdateLerp(i)) {
						m_targetGOs[i].transform.localPosition = Vector3.Lerp(m_targetStartPos[i], m_targetEndPos[i], m_fCurLerp[i]);
					}
				}
			}
		}

		public override void OnTransition(BaseMenuScreen prevMenu, bool bBack = false) {
			Setup();
			SetInitialPosition();
			m_bPerformTransition = true;
			for (int i = 0; i < m_targetGOs.Length; i++) {
				if (bBack) {
					m_fStartTime[i] = Time.realtimeSinceStartup + m_fDelay[i] + m_fBackDelay + m_fExtraDelay;
				} else {
					m_fStartTime[i] = Time.realtimeSinceStartup + m_fDelay[i] + m_fExtraDelay;
				}
			}
		}

		public void TransitionOut(MenuButton_SlideTransitionChangeScreen firer, bool bIgnoreDelay = false, float fDelay = 0.0f) {
			m_ButtonToInform = firer;
			m_bLerpOut = true;
			m_bPerformTransition = true;

			for (int i = 0; i < m_targetGOs.Length; i++) {
				//m_targetGOs[i].transform.localPosition = m_targetStartPos[i];
				m_fCurLerp[i] = 0.0f;
			}

			if (!bIgnoreDelay) {
				for (int i = 0; i < m_targetGOs.Length; i++) {
					m_fStartTime[i] = Time.realtimeSinceStartup + m_fDelay[i] + fDelay;
				}
			}
		}

		bool UpdateLerp(int nIdx) {
			if (m_fStartTime[nIdx] < Time.realtimeSinceStartup) {
				m_fCurLerp[nIdx] = Mathf.Clamp(m_fCurLerp[nIdx] + m_fSpeed[nIdx] * Time.unscaledDeltaTime, 0.0f, 1.0f);
				return true;
			}

			return false;
		}

		void Update() {
			if (m_bPerformTransition) {
				UpdatePositions();

				bool bAllLerpsFinished = true;
				for (int i = 0; i < m_fCurLerp.Length; i++) {
					if (m_fCurLerp[i] < 1.0f) {
						bAllLerpsFinished = false;
						break;
					}
				}
				if (bAllLerpsFinished) {
					m_bPerformTransition = false;

					if (m_bLerpOut && m_ButtonToInform != null) {
						m_ButtonToInform.ChangeScreen();
						m_bLerpOut = false;
						m_ButtonToInform = null;
						BaseMenuScreen.m_bInputLocked = false;
					}
				}
			}
		}
	}
}