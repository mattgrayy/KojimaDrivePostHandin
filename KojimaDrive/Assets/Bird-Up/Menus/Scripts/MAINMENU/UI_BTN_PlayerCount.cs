//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Player Count toggle
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_BTN_PlayerCount : MenuButton_Listener {
		public TypogenicText m_Indicator;
		public TypogenicText m_arrowLeft;
		public TypogenicText m_arrowRight;
		public static int s_nPlayerCount = 1;

		void Start() {
			UpdateText();
			UpdateArrows(m_ParentButton == null ? false : m_ParentButton.m_bSelected);
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
			s_nPlayerCount = Mathf.Clamp(s_nPlayerCount - 1, 1, Kojima.GameController.s_nMaxPlayers);

			UpdateText();
			MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_TOGGLE);

			UpdateArrows(m_ParentButton.m_bSelected);
		}

		public override void OnButtonRight(BaseMenuScreen parentMenu) {
			s_nPlayerCount = Mathf.Clamp(s_nPlayerCount + 1, 1, Kojima.GameController.s_nMaxPlayers);

			UpdateText();
			MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_TOGGLE);

			UpdateArrows(m_ParentButton.m_bSelected);
		}

		void UpdateText() {
			if (m_Indicator != null) {
				int nLevel = s_nPlayerCount;
				m_Indicator.Text = nLevel.ToString();
			}
		}

		public Color m_EnabledColor = Color.white;
		public Color m_DisabledColor = Color.grey;

		void UpdateArrows(bool bSelected) {
			if (m_arrowLeft != null) {
				m_arrowLeft.gameObject.SetActive(bSelected);
				if (s_nPlayerCount != 1) {
					m_arrowLeft.ColorTopLeft = m_EnabledColor;
				} else {
					m_arrowLeft.ColorTopLeft = m_DisabledColor;
				}
			}

			if (m_arrowRight != null) {
				m_arrowRight.gameObject.SetActive(bSelected);
				if(s_nPlayerCount != Kojima.GameController.s_nMaxPlayers) {
					m_arrowRight.ColorTopLeft = m_EnabledColor;
				} else {
					m_arrowRight.ColorTopLeft = m_DisabledColor;
				}
			}
		}
	}
}