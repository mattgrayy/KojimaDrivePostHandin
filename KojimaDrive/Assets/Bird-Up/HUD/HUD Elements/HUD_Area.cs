//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: HUD Element that displays the currently entered map area
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class HUD_Area : HUDElement {

		public Vector3 m_SlideInStart;
		Vector3 m_HUDPosition;
		public float m_fFadeOutSpeed = 1.0f;
		float m_fTimeToHide = 0.0f;
		public float m_fDisplayTime = 2.0f;

		public float m_fSlideSpeed = 1.0f;

		TypogenicText m_Text;
		Material m_Material;
		int m_nColID = -1;

		private void Start() {
			Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.UI_HUD_SHOW_AREANAME, ShowArea);


			m_Text = GetComponent<TypogenicText>();
			Renderer rend = GetComponent<Renderer>();
			m_Material = rend.material;
			m_Material.EnableKeyword("GLOBAL_MULTIPLIER_ON");
			m_nColID = Shader.PropertyToID("_GlobalMultiplierColor");
			m_HUDPosition = transform.localPosition;
		}

		protected override void OnDestroy() {
			Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.UI_HUD_SHOW_AREANAME, ShowArea);
			base.OnDestroy();
		}

		public class hudAreaData_t {
			public int m_nTargetPlayerID;
			public string m_strAreaName;
		}

		void ShowArea(object data) {
			hudAreaData_t dataobj = (hudAreaData_t)data;
			if (dataobj == null) {
				return;
			}

			if (m_ParentController.m_nPlayer != dataobj.m_nTargetPlayerID) {
				return;
			}

			Color col = m_Material.GetColor(m_nColID);


			// If we're changing name, force reset our animation
			if (m_Text.Text != dataobj.m_strAreaName) {
				m_Text.Text = dataobj.m_strAreaName;
				col.a = 0.0f;
			}

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
			if (m_fTimeToHide <= Time.realtimeSinceStartup) {
				Color col = m_Material.GetColor(m_nColID);
				col.a = Mathf.Clamp(col.a - (m_fFadeOutSpeed * Time.unscaledDeltaTime), 0, 1.0f);
				m_Material.SetColor(m_nColID, col);
			} else {
				transform.localPosition = Vector3.Lerp(transform.localPosition, m_HUDPosition, Time.unscaledDeltaTime * m_fSlideSpeed);
			}
		}

		public override void UpdateHUDElement() { 
			Animate();
		}
	}
}