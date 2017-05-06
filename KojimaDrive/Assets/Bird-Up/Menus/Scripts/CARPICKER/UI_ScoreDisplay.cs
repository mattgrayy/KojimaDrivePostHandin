using UnityEngine;
using System.Collections;

namespace Bird {
	public class UI_ScoreDisplay : MonoBehaviour {
		public bool m_bSetOnStart = false;
		public bool m_bSetOnUpdate = false;
		public TypogenicText m_Text;

		public string m_Prefix = "$";

		void Start() {
			HF.ExperienceManager.Load();
			if (m_bSetOnStart) {
				m_Text.Text = string.Format(m_Prefix + "{0:D8}", HF.ExperienceManager.GlobalEXP);
			}
		}

		void Update() {
			if (m_bSetOnUpdate) {
				m_Text.Text = string.Format(m_Prefix + "{0:D8}", HF.ExperienceManager.GlobalEXP);
			}
		}
	}
}