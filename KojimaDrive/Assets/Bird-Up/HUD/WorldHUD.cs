//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Sets up the in-world HUD stuff.
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bird {
	public class WorldHUD : HUDController {
		public Bam.CarSockets.Sockets m_Socket = Bam.CarSockets.Sockets.RightDoor;

		protected override void Start() {
			m_nPlayer = m_ParentController.m_nPlayer;
			AttachToCar(Kojima.GameController.s_singleton.m_players[m_nPlayer - 1]);
			base.Start();

			for (int i = 0; i < m_HUDElements.Count; i++) {
				if (m_HUDElements[i] == null) {
					continue;
				}
				m_HUDElements[i].gameObject.layer = m_nLayer;
			}
		}

		public override void UpdateHUDElement() {
			base.UpdateHUDElement();
		}

		protected override void OnDestroy() {
			DetachFromCar(m_AttachedPlayer);
			base.OnDestroy();
		}

		Kojima.CarScript m_AttachedPlayer;

		public void AttachToCar(Kojima.CarScript player) {
			CameraFaceAndScale camfacer = GetComponent<CameraFaceAndScale>();
			if (camfacer != null) {
				// That's a tortured bit of access going on there, huh?
				camfacer.m_CamToFace = Kojima.CameraManagerScript.singleton.playerCameras[m_nPlayer - 1].GetUICameraComponent();
			}

			// Grab our socket
			Transform socket = Kojima.GameController.s_singleton.m_players[m_nPlayer - 1].GetSocket(m_Socket);
			if (socket != null) {
				transform.SetParent(socket);
				transform.localPosition = Vector3.zero;
				Quaternion quat = Quaternion.Euler(0, 0, 0);
				transform.localRotation = quat;
				//transform.SetParent(Kojima.GameController.s_singleton.m_players[m_nPlayer - 1].GetCarBody().transform, true); // give us some wobble
			}

			player.m_WorldUI = this;
			m_AttachedPlayer = player;

			// Setup layermask
			m_nLayer = m_nPlayer + 7;
		}

		public void DetachFromCar(Kojima.CarScript player) {
			if(player != m_AttachedPlayer) {
				Debug.LogError("WorldHUD::DetachFromCar - Uh... We're trying to detach from the wrong player!!");
				return; // What?
			}

			// Clear our parentage
			transform.SetParent(null);

			CameraFaceAndScale camfacer = GetComponent<CameraFaceAndScale>();
			if (camfacer != null) {
				camfacer.m_CamToFace = null;
			}

			player.m_WorldUI = null;
			m_AttachedPlayer = null;
		}
	}
}