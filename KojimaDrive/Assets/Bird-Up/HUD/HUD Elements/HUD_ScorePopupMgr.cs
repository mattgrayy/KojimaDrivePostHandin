//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Manages score popup objects (which are pooled)
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections.Generic;

namespace Bird {
	public class HUD_ScorePopupMgr : HUDController {
		[NotNull]
		public GameObject m_ScorePopupPrefab;
		public int m_nScorePopupsToPool = 10;
		AudioSource m_AudioSrc;
		public AudioClip m_ScoreGain;
		public AudioClip m_ScoreLose;
		//public List<HUD_ScorePopup> m_ScorePopups;

		protected override void Start() {
			m_AudioSrc = GetComponent<AudioSource>();

			m_nPlayer = m_ParentController.m_nPlayer;
			m_nLayer = m_ParentController.m_nLayer;

			for(int i = 0; i < m_nScorePopupsToPool; i++) {
				HUD_ScorePopup popup = Instantiate(m_ScorePopupPrefab).GetComponent<HUD_ScorePopup>();
				popup.gameObject.layer = m_nLayer;
				popup.m_bDisplay = false;
				AddHUDElement(popup);
				popup.transform.SetParent(transform);
				popup.transform.localPosition = popup.m_StartPos;
				popup.transform.localRotation = Quaternion.Euler(0, 0, 0);
			}

			Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.UI_HUD_SHOW_EXP_POPUP, ShowScorePopup);
		}

		protected override void OnDestroy() {
			Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.UI_HUD_SHOW_EXP_POPUP, ShowScorePopup);
			base.OnDestroy();
		}

		public override void UpdateHUDElement() {
			base.UpdateHUDElement();
		}

		public class hudScorePopupData_t {
			public int m_nTargetPlayerID; // 0 = all players
			public int m_nXP; // Score to display
			public string m_strReason; // Why are they getting this score?

			public bool m_bUseCustomColours; // Use the below HSV val?
			public Vector4 m_HSV; // Custom colours

			public bool m_bPlaySound;

			public float m_fSpeed;
			public float m_fHoldTime;
		}

		public void ShowScorePopup(object data) {
			hudScorePopupData_t dataobj = (hudScorePopupData_t)data;
			if(dataobj == null) {
				return;
			}

			if (m_ParentController.m_nPlayer != dataobj.m_nTargetPlayerID && dataobj.m_nTargetPlayerID != 0) {
				return;
			}

			// If we're not enabled, skip and don't display
			if(!m_bDisplayActual) {
				return;
			}

			if(dataobj.m_bPlaySound) {
				if(dataobj.m_nXP >= 0) {
					m_AudioSrc.PlayOneShot(m_ScoreGain);
				} else {
					m_AudioSrc.PlayOneShot(m_ScoreLose);
				}
			}

			float fSpeed = dataobj.m_fSpeed;
			if(fSpeed == -1) {
				fSpeed = 4.0f;
			}

			float fHoldTime = dataobj.m_fHoldTime;
			if (fHoldTime == -1) {
				fHoldTime = 2.0f;
			}

			// Find a dead popup
			for (int i = 0; i < m_HUDElements.Count; i++) {
				if(!m_HUDElements[i].m_bDisplay) {
					if (dataobj.m_bUseCustomColours) {
						((HUD_ScorePopup)m_HUDElements[i]).Popup(dataobj.m_strReason, dataobj.m_nXP, dataobj.m_HSV);
					} else {
						((HUD_ScorePopup)m_HUDElements[i]).Popup(dataobj.m_strReason, dataobj.m_nXP);
					}

					((HUD_ScorePopup)m_HUDElements[i]).m_fSpeed = fSpeed;
					((HUD_ScorePopup)m_HUDElements[i]).m_fHoldLerp = fHoldTime;
					return; // Job has been jobbed.
				}
			}

			// Uhoh... we found no popup to display!
			// Find the oldest one and repurpose it
			HUD_ScorePopup oldest = (HUD_ScorePopup)m_HUDElements[0];
			for (int i = 1; i < m_HUDElements.Count; ++i) {
				if (((HUD_ScorePopup)m_HUDElements[i]).m_fCreationTime < oldest.m_fCreationTime) {
					oldest = (HUD_ScorePopup)m_HUDElements[i];
				}
			}

			if (dataobj.m_bUseCustomColours) {
				oldest.Popup(dataobj.m_strReason, dataobj.m_nXP, dataobj.m_HSV);
			} else {
				oldest.Popup(dataobj.m_strReason, dataobj.m_nXP);
			}

			oldest.m_fSpeed = fSpeed;
			oldest.m_fHoldLerp = fHoldTime;
		}
	}
}
