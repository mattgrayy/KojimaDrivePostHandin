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
	public abstract class MenuButton_Listener : MonoBehaviour {
		public MenuButton m_ParentButton;
		public bool m_bEnabled = true;
		public bool m_bOverrideSounds = false;

		public abstract void OnButtonPress(BaseMenuScreen parentMenu);
		public abstract void OnButtonSelect(BaseMenuScreen parentMenu);
		public abstract void OnButtonDeselect(BaseMenuScreen parentMenu);

		// These are virtual not abstract so we don't have to update all our listeners
		public virtual void OnButtonLeft(BaseMenuScreen parentMenu) { }
		public virtual void OnButtonRight(BaseMenuScreen parentMenu) { }
	}
}