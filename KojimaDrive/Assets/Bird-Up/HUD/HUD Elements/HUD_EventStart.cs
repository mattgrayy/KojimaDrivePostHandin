//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: HUD Element that displays the current event name, then fades out
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public class HUD_EventStart : HUDElement {
		public float m_fFadeOutSpeed = 1.0f;
		float m_fTimeToHide = 0.0f;
		public float m_fDisplayTime = 2.0f;
		public float m_fSlideSpeed = 1.0f;

		public override bool m_bDisplay {
			get {
				return m_bDisplayActual;
			}
			set {
				// Swallow any attempts to turn us off, we're a master of our own destiny!
				m_bDisplayActual = true;
			}
		}

		public TypogenicText m_Text;
		Material m_Material;

		public TypogenicText m_TextByline;
		Material m_MaterialByline;

		int m_nColID = -1;

		public class hudEventStartData_t {
			public string m_strEventName;
			public string m_strEventDesc;
		}

		private void Start() {
			Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.UI_HUD_SHOW_EVENTSTART, StartEvent);

			Renderer rend = m_Text.GetComponent<Renderer>();
			m_Material = rend.material;
			m_Material.EnableKeyword("GLOBAL_MULTIPLIER_ON");

			Renderer rend2 = m_TextByline.GetComponent<Renderer>();
			m_MaterialByline = rend2.material;
			m_MaterialByline.EnableKeyword("GLOBAL_MULTIPLIER_ON");

			m_nColID = Shader.PropertyToID("_GlobalMultiplierColor");
		}

		protected override void OnDestroy() {
			Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.UI_HUD_SHOW_EVENTSTART, StartEvent);
		}

		void StartEvent(object data) {
			hudEventStartData_t dataobj = (hudEventStartData_t)data;
			if (dataobj == null) {
				return;
			}

			Color col = m_Material.GetColor(m_nColID);

			m_fTimeToHide = Time.realtimeSinceStartup + m_fDisplayTime;

			m_Text.Text = dataobj.m_strEventName;
			m_TextByline.Text = dataobj.m_strEventDesc;

			col.a = 1.0f;
			m_Material.SetColor(m_nColID, col);
			m_MaterialByline.SetColor(m_nColID, col);
		}

		void Animate() {
			if (m_fTimeToHide <= Time.realtimeSinceStartup) {
				Color col = m_Material.GetColor(m_nColID);
				col.a = Mathf.Clamp(col.a - (m_fFadeOutSpeed * Time.unscaledDeltaTime), 0, 1.0f);
				m_Material.SetColor(m_nColID, col);
				m_MaterialByline.SetColor(m_nColID, col);
			}
		}

		public override void UpdateHUDElement() {
			Animate();
		}
	}
}