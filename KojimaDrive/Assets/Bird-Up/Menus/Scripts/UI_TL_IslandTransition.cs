using UnityEngine;
using System.Collections;
namespace Bird {
	public class UI_TL_IslandTransition : UI_Transition_Listener {
		public override void TransitionCompleted() {
			Bam.LobbyManagerScript.singleton.___ChangeIslandInternal();
		}

		public override void TransitionInterrupted() {
			
		}

		public override void TransitionStarted() {
			
		}
	}
}