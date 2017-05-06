using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_TL_DestroyInputMgr : UI_Transition_Listener {
		public GameObject m_InputMgr;

		public override void TransitionCompleted() {
			// Clean up our menu input shit
			BaseMenuScreen.UnhookInput();
			if(m_InputMgr) {
				DestroyImmediate(m_InputMgr);
			}
		}

		public override void TransitionInterrupted() { }
		public override void TransitionStarted() { }
	}
}