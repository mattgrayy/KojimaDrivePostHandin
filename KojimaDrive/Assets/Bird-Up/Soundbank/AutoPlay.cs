//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: AutoPlays a sound from a Soundbank
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class AutoPlay : MonoBehaviour {
		public string m_BankName;
		public string m_SoundName;
		private AudioSource m_source = null;
		public bool m_bOneshot = false;
		public float m_fDelay = 0.0f;

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

		void Play() {
			if (m_bPlayOnSelf) {
				if (m_source == null) {
					m_source = gameObject.AddComponent<AudioSource>();
				}

				AudioClip clip = Soundbank.GetAudioclip(m_BankName, m_SoundName);
				if (clip != null) {
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
				bnk.StopSound();
				bnk.PlaySound(m_SoundName);
			}
		}
	}
}