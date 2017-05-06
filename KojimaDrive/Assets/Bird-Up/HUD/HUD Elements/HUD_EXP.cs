//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: HUD Element that displays Drive Points
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class HUD_EXP : HUDElement {
		public Vector3 m_SlideInStart;
		Vector3 m_HUDPosition;
		public float m_fFadeOutSpeed = 1.0f;
		float m_fTimeToHide = 0.0f;
		public float m_fDisplayTime = 2.0f;
		public float m_fSlideSpeed = 1.0f;

		public class hudEXPData_t {
			public int m_nTargetPlayerID; // 0 = all players
		}

		public class hudEXPModeData_t {
			public expToDisplay_e m_Mode;
		}

		private void Start() {
			Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.UI_HUD_SHOW_EXP, ShowEXP);
			Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.UI_HUD_EXP_MODE, SetEXPMode);


			m_Text = GetComponent<TypogenicText>();
			Renderer rend = GetComponent<Renderer>();
			m_Material = rend.material; // get an instance of this material
			m_Material.EnableKeyword("GLOBAL_MULTIPLIER_ON");
			m_nColID = Shader.PropertyToID("_GlobalMultiplierColor");
			m_HUDPosition = transform.localPosition;
		}

		protected override void OnDestroy() {
			Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.UI_HUD_SHOW_EXP, ShowEXP);
			Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.UI_HUD_EXP_MODE, SetEXPMode);
		}

		void SetEXPMode(object data) {
			hudEXPModeData_t dataobj = (hudEXPModeData_t)data;
			if (dataobj == null) {
				return;
			}

			m_EXPToDisplay = dataobj.m_Mode;

			// Now force our current value to the target, for speed
			switch (m_EXPToDisplay) {
				case expToDisplay_e.HUD_EXP_PLAYER_CURRENT:
					m_nTargetTotal = m_nCurrentTotal = HF.PlayerExp.GetCurrentEXP(m_ParentController.m_nPlayer - 1);
					break;
				case expToDisplay_e.HUD_EXP_PLAYER_SESSION:
					m_nTargetTotal = m_nCurrentTotal = HF.ExperienceManager.GetSessionEXP(m_ParentController.m_nPlayer - 1);
					break;
				case expToDisplay_e.HUD_EXP_GLOBAL:
				default:
					m_nTargetTotal = m_nCurrentTotal = HF.ExperienceManager.GlobalEXP;
					break;
			}
		}

		void ShowEXP(object data) {
			hudEXPData_t dataobj = (hudEXPData_t)data;
			if(dataobj == null) {
				return;
			}

			if (m_ParentController.m_nPlayer != dataobj.m_nTargetPlayerID && dataobj.m_nTargetPlayerID != 0) {
				return;
			}


			Color col = m_Material.GetColor(m_nColID);
			
			// If we're partially faded or not faded, bring back to full brightness, reset our clock
			if (col.a > 0.0f) {
				m_fTimeToHide = Time.realtimeSinceStartup + m_fDisplayTime;
			} else {
				// If we're totally faded, slide back in
				m_fTimeToHide = Time.realtimeSinceStartup + m_fDisplayTime + 0.25f;
				transform.localPosition = m_SlideInStart;
			}

			col.a = 1.0f;
			m_Material.SetColor(m_nColID, col);
		}

		void Animate() {
			transform.localPosition = Vector3.Lerp(transform.localPosition, m_HUDPosition, Time.unscaledDeltaTime * m_fSlideSpeed);

			if (m_nTargetTotal == m_nCurrentTotal) {
				if (m_fTimeToHide <= Time.realtimeSinceStartup) {
					Color col = m_Material.GetColor(m_nColID);
					col.a = Mathf.Clamp(col.a - (m_fFadeOutSpeed * Time.unscaledDeltaTime), 0, 1.0f);
					m_Material.SetColor(m_nColID, col);
				}
			}
		}

		public enum expToDisplay_e {
			HUD_EXP_PLAYER_CURRENT,
			HUD_EXP_PLAYER_SESSION,
			HUD_EXP_GLOBAL
		}

		public expToDisplay_e m_EXPToDisplay;

		TypogenicText m_Text;
		Material m_Material;
		int m_nColID = -1;
		public string m_Prefix = "₧"; // Prefix should be the Drive Points ₧ symbol

		int m_nTargetTotal = 0;
		double m_fCurrentTotal = 0;
		int m_nCurrentTotal = 0;
		public float m_fTickUpSpeed = 1.0f;
		public override void UpdateHUDElement() {
			Animate();

			switch (m_EXPToDisplay) {
				case expToDisplay_e.HUD_EXP_PLAYER_CURRENT:
					m_nTargetTotal = HF.PlayerExp.GetCurrentEXP(m_ParentController.m_nPlayer - 1);
					break;
				case expToDisplay_e.HUD_EXP_PLAYER_SESSION:
					m_nTargetTotal = HF.ExperienceManager.GetSessionEXP(m_ParentController.m_nPlayer - 1);
					break;
				case expToDisplay_e.HUD_EXP_GLOBAL:
				default:
					m_nTargetTotal = HF.ExperienceManager.GlobalEXP;
					break;
			}

			m_fCurrentTotal = DoubleExtensions.Lerp(m_fCurrentTotal, m_nTargetTotal, Time.unscaledDeltaTime * m_fTickUpSpeed);
			int nVal = (int)System.Math.Round(m_fCurrentTotal);
			if(nVal == m_nTargetTotal && m_nCurrentTotal != m_nTargetTotal) { // We've just hit our target, set our fadeout time
				m_fTimeToHide = Time.realtimeSinceStartup + m_fDisplayTime;
			}
			m_nCurrentTotal = nVal;
			m_Text.Text = string.Format(m_Prefix + "{0:D8}", m_nCurrentTotal);
		}
	}
}