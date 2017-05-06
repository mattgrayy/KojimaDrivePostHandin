//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Menu UI
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace Bird {
	public class MenuButton_VolumeSwitcher : MenuButton_Listener {
		[NotNull]
		public AudioMixerGroup m_TargetMixer;
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
			float fLevel = 0.0f;
			m_TargetMixer.audioMixer.GetFloat("volume", out fLevel);
			fLevel = Mathf.Clamp(fLevel - 0.05f, 0.0f, 1.0f);
			//VolumeController.SetVolLevel(m_Channel, fLevel);

			//Settings.SetSetting("VOL_" + m_Channel.ToString(), fLevel.ToString());

			UpdateText();
			MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_TOGGLE);
		}

		public override void OnButtonRight(BaseMenuScreen parentMenu) {
			/*float fLevel = VolumeController.GetVolLevel(m_Channel);
			fLevel = Mathf.Clamp(fLevel + 0.05f, 0.0f, 1.0f);
			VolumeController.SetVolLevel(m_Channel, fLevel);

			Settings.SetSetting("VOL_" + m_Channel.ToString(), fLevel.ToString());

			UpdateText();
			MenuSounder.MenuSounds.DoMenuSound(MenuSounder.menuSounds_e.MS_TOGGLE);*/
		}

		void UpdateText() {
			if (m_Indicator != null) {
				//int nLevel = Mathf.RoundToInt(VolumeController.GetVolLevel(m_Channel) * 100);
				//m_Indicator.Text = nLevel.ToString();
			}
		}

		void UpdateArrows(bool bSelected) {
			if (m_arrows != null) {
				m_arrows.SetActive(bSelected);
			}
		}
	}
}