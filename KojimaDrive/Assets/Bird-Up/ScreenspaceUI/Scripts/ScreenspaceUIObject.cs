//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Screenspace UI objects that automatically adjust their scale or
//			position during aspect ratio changes.
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class ScreenspaceUIObject : MonoBehaviour {
		[NotNull, Name("Master Camera")]
		public ScreenspaceUICamera m_Master;

		[System.Flags]
		public enum adjustMode_e {
			ADJUST_NONE = 0,
			ADJUST_POSITION = 1,
			ADJUST_SCALE = 2,
			UNIFORM_SCALE = 4
		}
		public adjustMode_e m_eAdjustMode;

		[Name("Adjust Typogenic Word Wrap")]
		public bool m_bAdjustTypogenicWordwrap = false;
		public bool m_bAdjustTypogenicCharacterSize = false;

		void Awake() {
			if (m_Master) {
				m_Master.RegisterObject(this);
			} else {
				Debug.Log("ScreenspaceUIObject '" + gameObject.name + "' is orphaned!");
			}
		}

		void OnDestroy() {
			if (m_Master) {
				m_Master.UnregisterObject(this);
			} else {
				Debug.Log("ScreenspaceUIObject '" + gameObject.name + "' is orphaned!");
			}
		}

		Vector3 m_CurScale;
		Vector3 m_CurPos;
		Vector3 m_CurLocPos;
		Transform m_CurParent;

		public void SaveCurrentState(Transform appropriateParent) {
			// Save off our abs values
			m_CurScale = transform.localScale;
			m_CurParent = transform.parent;
			m_CurLocPos = transform.localPosition;
			m_CurPos = transform.position;
		}

		public void AdjustObject(ScreenspaceUICamera controllingCam) {
			if ((m_eAdjustMode & adjustMode_e.ADJUST_SCALE) == 0) {
				m_CurScale.x /= (controllingCam.m_ScaleChange.x);
				m_CurScale.y /= (controllingCam.m_ScaleChange.y);
				transform.localScale = m_CurScale;
			} else if ((m_eAdjustMode & adjustMode_e.UNIFORM_SCALE) == adjustMode_e.UNIFORM_SCALE) {
				m_CurScale.y *= (controllingCam.m_ScaleChange.x);
				transform.localScale = m_CurScale;
			}

			if ((m_eAdjustMode & adjustMode_e.ADJUST_POSITION) == 0) {
				Vector2 posChange;
				posChange.x = m_CurPos.x / (controllingCam.m_ScaleChange.x);
				posChange.y = m_CurPos.y / (controllingCam.m_ScaleChange.y); // Why bother? Y never changes...
				transform.position = posChange;
			}

			// Fix up our z axis
			Vector3 posFixedup = transform.localPosition;
			posFixedup.z = m_CurLocPos.z;
			transform.localPosition = posFixedup;

			TypogenicText[] typos = GetComponents<TypogenicText>();
			for(int i = 0; i < typos.Length; i++) {
				if (m_bAdjustTypogenicWordwrap) {
					typos[i].WordWrap = typos[i].WordWrap * controllingCam.m_ScaleChange.x;
				}
				if(m_bAdjustTypogenicCharacterSize) {
					typos[i].Size = typos[i].Size * controllingCam.m_ScaleChange.x;
				}
			}
		}
	}
}