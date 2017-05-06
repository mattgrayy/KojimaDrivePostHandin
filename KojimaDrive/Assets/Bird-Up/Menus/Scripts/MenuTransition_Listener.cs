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
	public abstract class MenuTransition_Listener : MonoBehaviour {
		public abstract void OnTransition(BaseMenuScreen prevMenu, bool bBack = false);
	}
}