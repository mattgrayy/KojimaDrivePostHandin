//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Shows the title on the splashscreen
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_TL_ShowTitle : UI_Transition_Listener {
		public UI_Background_Stripes m_Logo;
		public GameObject m_StartButton;
		public AudioSource m_Music;

		public override void TransitionCompleted() {
			m_StartButton.SetActive(true);
			m_Music.Play();
		}

		public override void TransitionInterrupted() { }

		public override void TransitionStarted() {
			m_StartButton.SetActive(false);
		}

		public override void TransitionUpdate(BaseTransition transition) {
			Transition_Generic gen = (Transition_Generic)transition;
			if(gen) {
				if(gen.m_fLerpState >= 0.75f) {
					m_Logo.m_bGo = true;
				}

				/*if (gen.m_fLerpState >= 0.5f) {
					if(!m_Music.isPlaying) {
						m_Music.Play();
					}
						
					m_Music.volume = (gen.m_fLerpState - 0.5f) * 2;
				}*/
			}
		}
	}
}