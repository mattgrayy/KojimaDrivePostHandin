//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Static class that manages active transitions
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bird {
	public static class TransitionController {
		static public Dictionary<string, BaseTransition> s_CurrentlyActiveTransitions;

		public static BaseTransition GetActiveTransition(string channel) {
			if(s_CurrentlyActiveTransitions.ContainsKey(channel)) {
				return s_CurrentlyActiveTransitions[channel];
			}

			return null;
		}

		public static void StopAllTransitions(bool bInterrupt = false) {
			foreach (KeyValuePair<string, BaseTransition> entry in s_CurrentlyActiveTransitions) {
				if (bInterrupt) {
					entry.Value.InterruptTransition();
				} else {
					entry.Value.StopTransition();
				}
			}
		}
	}
}