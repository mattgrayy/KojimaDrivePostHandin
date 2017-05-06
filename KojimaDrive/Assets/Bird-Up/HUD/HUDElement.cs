//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: HUD Controller Script
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System;
using System.Collections.Generic;

namespace Bird {
	public abstract class HUDElement : MonoBehaviour {
		//public bool m_bDisplay = false;
		[SerializeField]
		protected bool m_bDisplayActual = true;
		public virtual bool m_bDisplay {
			get {
				return m_bDisplayActual;
			}
			set {
				if (m_bDisplayActual != value) {
					if(value) {
						OnDisplay();
					} else {
						OnHide();
					}

					m_bDisplayActual = value;
				}
			}
		}
		public HUDController m_ParentController;

		protected virtual void OnDisplay() {

		}

		protected virtual void OnHide() {

		}

		protected virtual void OnDestroy() {
			if(m_ParentController) {
				m_ParentController.RemoveHUDElement(this);
			}
		}

		public abstract void UpdateHUDElement();
	}
}
