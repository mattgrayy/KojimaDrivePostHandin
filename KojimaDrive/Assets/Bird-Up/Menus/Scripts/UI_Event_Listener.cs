//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Menu UI event system (gameplay event system doesn't allow for enough nuance)
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public abstract class UI_Transition_Listener : MonoBehaviour {
		[HideInInspector]
		public BaseTransition m_TriggeredTransition;

		public abstract void TransitionCompleted();
		public virtual void TransitionUpdate(BaseTransition transition) { }
		public abstract void TransitionInterrupted();
		public abstract void TransitionStarted();
	}
}