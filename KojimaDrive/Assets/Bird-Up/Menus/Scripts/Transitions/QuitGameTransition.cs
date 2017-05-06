//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Menu UI
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class QuitGameTransition : MenuTransition_Listener {
		public override void OnTransition(BaseMenuScreen prevMenu, bool bBack = false) {
			// Slightly hacky to use this transition controller to quit the game but I'm trying to get this made quick, alright? -sam 13/01/2017
			Application.Quit();
		}
	}
}