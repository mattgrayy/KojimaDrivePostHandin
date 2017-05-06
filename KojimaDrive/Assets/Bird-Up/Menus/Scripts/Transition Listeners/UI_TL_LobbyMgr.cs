//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Transition listener for the lobby scene
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_TL_LobbyMgr : UI_Transition_Listener {
		public enum lobbyMgrCommand_e {
			NULL,
			LOAD_GAMEMODE,
			RETURN_TO_LOBBY
		}
		public static lobbyMgrCommand_e s_CommandToExecute;

		public override void TransitionCompleted() {
			switch(s_CommandToExecute) {
				case lobbyMgrCommand_e.LOAD_GAMEMODE:
					Bam.LobbyManagerScript.singleton.____LoadGameInternal();
					break;
				case lobbyMgrCommand_e.RETURN_TO_LOBBY:
					Bam.LobbyManagerScript.singleton.____ReturnToLobbyInternal();
					break;
				case lobbyMgrCommand_e.NULL:
				default:
					return;
			}
		}

		public override void TransitionInterrupted() { }
		public override void TransitionStarted() { }
	}
}