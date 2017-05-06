//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Quits the game.
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_TL_QuitApplication : UI_Transition_Listener {

		public override void TransitionCompleted() {
			Application.Quit();
		}

		public override void TransitionInterrupted() {
			
		}

		public override void TransitionStarted() {
			
		}
	}
}