//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Triggers another KJD transition
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_TL_TriggerKJDTransition : UI_Transition_Listener {
		public BaseTransition m_FireTransitionOnCompletedReverse;
		public bool m_bReverse1;

		public BaseTransition m_FireTransitionOnCompleted;
		public bool m_bReverse2;


		public override void TransitionCompleted() {
			if (((Transition_Generic)m_TriggeredTransition).m_bPlayInReverse) {
				if(m_bReverse1) {
					m_FireTransitionOnCompletedReverse.StartTransitionReverse();
				} else {
					m_FireTransitionOnCompletedReverse.StartTransition();
				}
			} else {
				if (m_bReverse2) {
					m_FireTransitionOnCompleted.StartTransitionReverse();
				} else {
					m_FireTransitionOnCompleted.StartTransition();
				}
			}
			
		}

		public override void TransitionInterrupted() {
			
		}

		public override void TransitionStarted() {
			
		}
	}
}