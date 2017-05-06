//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Act as 'car picker' for a player.
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	[ExecuteInEditMode]
	public class UI_PlayerStartMenu : MonoBehaviour {

		public enum stage_e {
			KOJIMA,
			OHIYAKU
		}

		public stage_e m_eStage = stage_e.KOJIMA;
		
		public BaseTransition m_StartGameKojima;
		public BaseTransition m_StartGameOhiyaku;

		public BaseTransition m_BackToMenu;
		public UI_BTN_CarPicker[] m_CarPickers;

		public void Back() {
			// Should we quit back to the other menu?
			bool bQuit = true;
			for(int i = 0; i < m_CarPickers.Length; i++) {
				if(m_CarPickers[i].m_ePlayerState != UI_BTN_CarPicker.playerState_e.OUT_OF_GAME) {
					bQuit = false; // One person isn't backing out, so don't back out.
				}
			}

			if(bQuit) {
				m_BackToMenu.StartTransition();
			}
		}

		public void StartGame() {
			// Clear any existing Session EXP data
			HF.ExperienceManager.ResetSessionEXPAll();

			for (int i = 0; i < m_CarPickers.Length; i++) {
				Kojima.GameController.s_PlayerCreationData[i] = new Kojima.GameController.playerCreation_t();
				if (m_CarPickers[i].m_ePlayerState == UI_BTN_CarPicker.playerState_e.READY) {
					// Find all our ready players
					Kojima.GameController.s_PlayerCreationData[i].m_bOptedIn = true;
					Kojima.GameController.s_PlayerCreationData[i].m_eChosenCar = m_CarPickers[i].m_eSelectedCar;
					Kojima.GameController.s_PlayerCreationData[i].m_nControllerID = i;
				} else {
					Kojima.GameController.s_PlayerCreationData[i].m_bOptedIn = false;
				}
			}

			Kojima.GameController.s_eLoadMode = Kojima.GameController.loadMode_e.STANDARD; // Standard load mode

			switch (m_eStage) {
				case stage_e.KOJIMA:
				default:
					m_StartGameKojima.StartTransition();
					Kojima.GameController.s_eIsland = Kojima.GameController.island_e.KOJIMA;
					break;
				case stage_e.OHIYAKU:
					m_StartGameOhiyaku.StartTransition();
					Kojima.GameController.s_eIsland = Kojima.GameController.island_e.OHIYAKU;
					break;

			}
		}

		public bool AllowedToStartGame() {
			for(int i = 0; i < m_CarPickers.Length; i++) {
				if(m_CarPickers[i].m_ePlayerState == UI_BTN_CarPicker.playerState_e.CAR_PICKER) {
					return false; // Don't allow someone to start if someone else is on the car picker screen
				}
			}

			return true;
		}

		[System.Serializable]
		public class carTex_s {
			[HideInInspector]
			public string name;
			[HideInInspector]
			public Kojima.CarSwapManager.CarType m_eType;
			public Texture2D m_Texture;
		}

		public carTex_s[] m_CarTextures;

		void OnValidate() {
			if (m_CarTextures == null || m_CarTextures.Length != (int)Kojima.CarSwapManager.CarType.count) {
				m_CarTextures = new carTex_s[(int)Kojima.CarSwapManager.CarType.count];
				for(int i = 0; i < m_CarTextures.Length; i++) {
					m_CarTextures[i] = new carTex_s();
					m_CarTextures[i].m_eType = (Kojima.CarSwapManager.CarType)i;
					m_CarTextures[i].name = m_CarTextures[i].m_eType.ToString();
				}
			}
		}

		public Texture2D GetCarTexture(Kojima.CarSwapManager.CarType eType) {
			for (int i = 0; i < m_CarTextures.Length; i++) {
				if(m_CarTextures[i].m_eType == eType) {
					return m_CarTextures[i].m_Texture;
				}
			}

			return null;
		}
		
	}
}