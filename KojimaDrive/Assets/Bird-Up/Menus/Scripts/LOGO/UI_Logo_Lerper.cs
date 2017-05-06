

using UnityEngine;
using System.Collections;
namespace Bird {
	public class UI_Logo_Lerper : MonoBehaviour {
		public Vector3 m_StartPos;
		public Vector3 m_EndPos;
		public float m_fSpeed;
		float m_fLerpPos;
		public float m_fSoundDelay = 1.0f;
		public float m_fDelay = 0.5f;
		public bool m_bRunning = false;
		public BaseTransition m_TransListener;

		public float m_fTimeToFlyFirstLogo = 1.5f;
		public FirstLogoFlyaway m_Flyaway;

		public AudioSource m_Snd;
		bool m_bSoundPlayed = false;

		private void Update() {
			if(m_bRunning) {
				m_fLerpPos = Mathf.Clamp(m_fLerpPos + (m_fSpeed * Time.deltaTime), 0.0f, (m_fSoundDelay + m_fDelay) + 1.0f);
				if(!m_bSoundPlayed && m_fLerpPos >= m_fSoundDelay) {
					m_bSoundPlayed = true;
					m_Snd.Play();
				}

				if (m_fLerpPos == (m_fSoundDelay + m_fDelay) + 1.0f) {
					m_bRunning = false;
					if(m_TransListener) {
						m_TransListener.StartTransition();
					}
				}

				if (m_fLerpPos >= (m_fSoundDelay + m_fDelay)) {
					transform.localPosition = Vector3.Lerp(m_StartPos, m_EndPos, m_fLerpPos - (m_fSoundDelay + m_fDelay));

					if(m_fLerpPos - (m_fSoundDelay + m_fDelay) >= m_fTimeToFlyFirstLogo) {
						m_Flyaway.TriggerFlyaway();
					}
				}
			}
		}
	}
}