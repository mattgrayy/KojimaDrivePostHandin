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
	public class MenuButton_PlayerCountToggle : MenuButton_Listener {
		public TypogenicText m_Indicator;
		public GameObject m_arrows;

		void Start() {
			UpdateText();
		}

		public override void OnButtonPress(BaseMenuScreen parentMenu) {

		}

		public override void OnButtonSelect(BaseMenuScreen parentMenu) {
			UpdateArrows(true);
		}

		public override void OnButtonDeselect(BaseMenuScreen parentMenu) {
			UpdateArrows(false);
		}

		public override void OnButtonLeft(BaseMenuScreen parentMenu) {
			CarMaker.numberOfPlayers = Mathf.Clamp(CarMaker.numberOfPlayers - 1, 1, 4);

			UpdateText();
			MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_TOGGLE);
		}

		public override void OnButtonRight(BaseMenuScreen parentMenu) {
			CarMaker.numberOfPlayers = Mathf.Clamp(CarMaker.numberOfPlayers + 1, 1, 4);

			UpdateText();
			MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_TOGGLE);
		}

		void UpdateText() {
			if (m_Indicator != null) {
				int nLevel = CarMaker.numberOfPlayers;
				m_Indicator.Text = nLevel.ToString();
			}
		}

		void UpdateArrows(bool bSelected) {
			if (m_arrows != null) {
				m_arrows.SetActive(bSelected);
			}
		}
	}
}