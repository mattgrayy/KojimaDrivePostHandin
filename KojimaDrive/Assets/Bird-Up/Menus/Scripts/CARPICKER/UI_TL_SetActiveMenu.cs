using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_TL_SetActiveMenu : UI_Transition_Listener {


		public BaseMenuScreen m_TargetMenu;

		public override void TransitionCompleted() {
			BaseMenuScreen.m_ActiveScreen = m_TargetMenu;
		}

		public override void TransitionInterrupted() {
			
		}

		public override void TransitionStarted() {
			
		}
	}
}