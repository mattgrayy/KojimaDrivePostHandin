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
	public class UI_BTN_CarPicker : MenuButton_Listener {
		private void Start() {
			HF.ExperienceManager.Load(); // Load our global EXP value from over all sessions to allow for unlocking of cars
			m_CarMat = m_CarRenderer.material;

			UpdateCarSelectorDisplay();
			HandleStateChange(playerState_e.OUT_OF_GAME);
		} 

		public enum playerState_e {
			OUT_OF_GAME = 0, // 'press button to enter game'
			CAR_PICKER = 1, // 'pick car'
			READY // 'ready to play'
		}

		public UI_PlayerStartMenu m_ControllingMenu;

		public playerState_e m_ePlayerState = playerState_e.OUT_OF_GAME;

		// OUT_OF_GAME - Press button to enter screen
		public TypogenicText m_PressButtonToEnter;

		// CAR_PICKER - Car picker
		public Renderer m_CarRenderer;
		protected Material m_CarMat;
		public GameObject m_LockedGraphic; // If this car is locked
		public Kojima.CarSwapManager.CarType m_eSelectedCar;
		public Kojima.CarSwapManager.carStats_e m_SelectedCarStats;
		public TypogenicText m_CarName;
		public TypogenicText m_CarPrice;
		public GameObject m_Arrows;

		// READY - Ready screen
		public TypogenicText m_ReadyText;

		public override void OnButtonPress(BaseMenuScreen parentMenu) {
			switch(m_ePlayerState) {
				case playerState_e.OUT_OF_GAME:
				default:
					HandlePress_OutOfGame(parentMenu);
					break;
				case playerState_e.CAR_PICKER:
					HandlePress_CarPicker(parentMenu);
					break;
				case playerState_e.READY:
					HandlePress_Ready(parentMenu);
					break;
			}
		}

		public void HandlePress_OutOfGame(BaseMenuScreen parentMenu) {
			// Join the game
			MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_MOVE_DOWN);
			HandleStateChange(playerState_e.CAR_PICKER);
		}

		public bool CheckCheatMode(BaseMenuScreen parentMenu) {
			// Are we allowed to cheat?
			int nInputTarget = 4;
			Rewired.Player plyr = Rewired.ReInput.players.GetPlayer(parentMenu.m_nTargetPlayerID - 1);
			int nInput = (plyr.GetButton("Cheat1_Controller") ? 1 : 0) +
				(plyr.GetButton("Cheat2_Controller") ? 1 : 0) +
				(plyr.GetButton("Cheat3_Controller") ? 1 : 0) +
				(plyr.GetButton("Cheat4_Controller") ? 1 : 0) +
				(plyr.GetButton("Cheat_KB") ? 4 : 0);

			return nInputTarget <= nInput;
		}

		public void HandlePress_CarPicker(BaseMenuScreen parentMenu) {
			// Progress to ready, but only if the car we've selected is purchasable
			if (HF.ExperienceManager.GlobalEXP < m_SelectedCarStats.m_nEXPPrice && !CheckCheatMode(parentMenu)) {
				// Uh oh, we can't afford it :C
				MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_ERROR);
				return;
			} else {
				MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_MOVE_DOWN);
				HandleStateChange(playerState_e.READY);
			}
		}

		public void HandlePress_Ready(BaseMenuScreen parentMenu) {
			// Let's see if everyone is ready; don't progress if someone is still picking their car!
			if (m_ControllingMenu.AllowedToStartGame()) {
				// Ready! Press A to start game.
				MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_SELECT);
				m_ControllingMenu.StartGame();
			} else {
				// Not everyone is ready. Let's play an error sound.
				MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_ERROR);
			}
		}

		// If we add new things, add them to this list
		public void HandleStateChange(playerState_e newState) {
			m_ePlayerState = newState;
			bool bState = m_ePlayerState == playerState_e.OUT_OF_GAME;
			m_PressButtonToEnter.gameObject.SetActive(bState);

			bState = m_ePlayerState == playerState_e.CAR_PICKER;
			m_LockedGraphic.gameObject.SetActive(false);
			m_CarName.gameObject.SetActive(bState);
			m_CarPrice.gameObject.SetActive(bState);
			m_Arrows.gameObject.SetActive(bState);
			if(bState) {
				UpdateCarSelectorDisplay();
			}

			bState = m_ePlayerState == playerState_e.READY;
			m_ReadyText.gameObject.SetActive(bState);

			bState = m_ePlayerState == playerState_e.READY || m_ePlayerState == playerState_e.CAR_PICKER;
			m_CarRenderer.gameObject.SetActive(bState);
		}

		public override void OnButtonSelect(BaseMenuScreen parentMenu) {
			
		}

		public override void OnButtonDeselect(BaseMenuScreen parentMenu) {
			
		}

		public override void OnButtonLeft(BaseMenuScreen parentMenu) {
			if(m_ePlayerState != playerState_e.CAR_PICKER) {
				return;
			}

			Kojima.CarSwapManager.CarType nNewCar = m_eSelectedCar - 1;
			if(nNewCar <= Kojima.CarSwapManager.CarType.NA) {
				// Loop back around
				nNewCar = Kojima.CarSwapManager.CarType.count - 1;
			}

			m_eSelectedCar = nNewCar;

			UpdateCarSelectorDisplay();

			MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_TOGGLE);
		}

		public override void OnButtonRight(BaseMenuScreen parentMenu) {
			if (m_ePlayerState != playerState_e.CAR_PICKER) {
				return;
			}

			Kojima.CarSwapManager.CarType nNewCar = m_eSelectedCar + 1;
			if (nNewCar >= Kojima.CarSwapManager.CarType.count) {
				// Loop back around
				nNewCar = Kojima.CarSwapManager.CarType.NA + 1;
			}

			m_eSelectedCar = nNewCar;

			UpdateCarSelectorDisplay();

			MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_TOGGLE);
		}

		public void UpdateCarSelectorDisplay() {
			Texture2D carTex = m_ControllingMenu.GetCarTexture(m_eSelectedCar);
			m_CarMat.mainTexture = carTex;

			m_SelectedCarStats = Kojima.CarSwapManager.GetCarStats(m_eSelectedCar);

			m_CarName.Text = m_SelectedCarStats.name;
			// m_CarPrice.Text = string.Format("$" + "{0:D5}", m_SelectedCarStats.m_nEXPPrice);
			m_CarPrice.Text = "$" + m_SelectedCarStats.m_nEXPPrice.ToString();
			m_LockedGraphic.SetActive(HF.ExperienceManager.GlobalEXP < m_SelectedCarStats.m_nEXPPrice);
		}
	}
}