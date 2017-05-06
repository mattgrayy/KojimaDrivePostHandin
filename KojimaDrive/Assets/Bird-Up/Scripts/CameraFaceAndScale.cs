//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Used for in-world UI objects that must maintain a consistent on-screen scale and rotation towards the camera lens
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

namespace Bird {
	public class CameraFaceAndScale : MonoBehaviour {
		[Name("Camera To Face")]
		public Camera m_CamToFace;
		public float m_fMaxDistanceFromCamera = 5.0f;
		private void Update() {
			if (m_CamToFace != null) {
				// Keep within range of the camera!
				transform.position = (transform.parent.transform.position - m_CamToFace.transform.position).normalized * m_fMaxDistanceFromCamera + m_CamToFace.transform.position;
				transform.rotation = Quaternion.LookRotation(transform.position - m_CamToFace.transform.position, m_CamToFace.transform.up);
			}
		}
	}
}