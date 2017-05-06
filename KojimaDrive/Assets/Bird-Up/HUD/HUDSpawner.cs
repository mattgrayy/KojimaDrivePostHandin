//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Spawns the HUD
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections.Generic;

namespace Bird {
	public class HUDSpawner : MonoBehaviour {

		[NotNull]
		public GameObject m_HUDPrefab;
		[NotNull]
		public GameObject m_WorldHUDPrefab;
		[NotNull]
		public GameObject m_FullscreenHUDPrefab;

		static public List<GameObject> s_HUD = new List<GameObject>();
		static public List<GameObject> s_WorldHUDs = new List<GameObject>();
		public static GameObject s_FullscreenUI;

		void Start() {
			for(int i = 0; i < Kojima.GameController.s_ncurrentPlayers; i++) {
				GameObject newHUD = Instantiate(m_HUDPrefab);
				HUDController ctrl = newHUD.GetComponent<HUDController>();
				ctrl.m_nPlayer = i + 1;
				ctrl.m_nLayer = LayerMask.NameToLayer("UI");
				Vector3 pos = Vector3.zero;
				pos.y = i * 30.0f; // Space the huds out
				newHUD.transform.position = pos;
				newHUD.GetComponent<Camera>().rect = Kojima.CameraManagerScript.singleton.playerCameras[i].Cam.rect;
				Kojima.GameController.s_singleton.m_players[i].m_PlayerHUD = ctrl;


				GameObject newWorldHud = Instantiate(m_WorldHUDPrefab);
				WorldHUD worldhud = newWorldHud.GetComponent<WorldHUD>();
				worldhud.m_nPlayer = i + 1;
				worldhud.m_nLayer = LayerMask.NameToLayer("InWorldUIP" + worldhud.m_nPlayer.ToString());

				// Hook 'em up
				ctrl.AddHUDElement(worldhud);

				s_HUD.Add(newHUD);
				s_WorldHUDs.Add(newWorldHud);
			}

			s_FullscreenUI = Instantiate(m_FullscreenHUDPrefab);
			Vector3 pos2 = Vector3.zero;
			pos2.y = -30.0f;
			s_FullscreenUI.transform.position = pos2;
		}
	}
}