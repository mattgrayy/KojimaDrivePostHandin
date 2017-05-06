//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Sequentially plays audio from a Soundbank
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class SequentialPlayer : MonoBehaviour {
		public string m_BankName;
		public string m_SoundName;
		private AudioSource m_source = null;
		public bool m_bOneshot = false;
		public float m_fDelay = 0.0f;
		float m_fNextPlayTime = 0.0f;

		public float m_fTrackGap = 0.5f;

		public bool m_bAwakeNotStart = false;
		public bool m_bPlayOnSelf = false;

		void Awake() {
			if (m_bAwakeNotStart) {
				Play();
			}
		}

		void Start() {
			if (!m_bAwakeNotStart) {
				Play();
			}
		}

		AudioSource m_activeSource;
		void Play() {
			if (m_bPlayOnSelf) {
				if (m_source == null) {
					m_source = gameObject.AddComponent<AudioSource>();
					m_activeSource = m_source;
				}

				AudioClip clip = Soundbank.GetAudioclip(m_BankName, m_SoundName);
				if (clip != null) {
					m_fNextPlayTime = clip.length + Time.realtimeSinceStartup + m_fTrackGap;
					if (m_bOneshot) {
						m_source.PlayOneShot(clip);
					} else {
						m_source.clip = clip;
						if (m_fDelay <= 0.0f) {
							m_source.Play();
						} else {
							m_source.PlayDelayed(m_fDelay);
						}
					}
				}
			} else {
				Soundbank bnk = Soundbank.GetStaticSoundbank(m_BankName);
				AudioClip clip = bnk.GetAudioclip(m_SoundName);
				bnk.StopSound();
				bnk.PlaySound(clip);
				m_fNextPlayTime = clip.length + Time.realtimeSinceStartup + m_fTrackGap;
				m_activeSource = bnk.GetAudioSource();
			}

			m_bPlaying = true;
		}

		void Update() {
			if (m_bPlaying && /*!m_activeSource.isPlaying*/ m_fNextPlayTime <= Time.realtimeSinceStartup) {
				Play();
			}
		}

		float m_fTimeOnPause;
		bool m_bPlaying = false;
		void Pause() {
			m_fTimeOnPause = Time.realtimeSinceStartup;
			m_activeSource.Pause();
			m_bPlaying = false;
		}

		void UnPause() {
			m_fNextPlayTime += Time.realtimeSinceStartup - m_fTimeOnPause;
			m_bPlaying = true;
			m_activeSource.Play();
		}

		void Stop() {
			m_fNextPlayTime = 0;
			m_bPlaying = false;
			m_activeSource.Stop();
		}
	}
}