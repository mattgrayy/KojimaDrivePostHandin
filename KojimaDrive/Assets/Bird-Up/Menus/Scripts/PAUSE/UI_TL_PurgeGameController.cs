using UnityEngine;
using System.Collections.Generic;

namespace Bird {
	public class UI_TL_PurgeGameController : UI_Transition_Listener {
		public override void TransitionCompleted() {
			Kojima.GameController.PurgeGame();
		}

		public override void TransitionInterrupted() {
		
		}

		public override void TransitionStarted() {
			
		}
	}
}