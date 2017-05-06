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
	public class UI_TL_VolumeLerp : UI_Transition_Listener {
		public override void TransitionCompleted() { }

		public float m_fMultiplier = 1.0f;
		public AudioSource m_AudioSrc;
		public override void TransitionUpdate(BaseTransition transition) {
			Transition_Generic gen = (Transition_Generic)transition;
			if (gen) {
				if (m_AudioSrc) {
					m_AudioSrc.volume = m_fMultiplier * gen.m_fLerpState;
				}
			}
		}

		public override void TransitionInterrupted() { }

		public override void TransitionStarted() { }
	}
}