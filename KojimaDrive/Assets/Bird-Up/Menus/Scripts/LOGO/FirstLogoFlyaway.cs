using UnityEngine;
using System.Collections;

namespace Bird {
	public class FirstLogoFlyaway : MonoBehaviour {
		public void TriggerFlyaway() {
			if(!m_bFlyawayStarted) {
				m_Crashsnd.Play();
			}
				

			m_bFlyawayStarted = true;
		}

		bool m_bFlyawayStarted = false;

		private void Update() {
			if(m_bFlyawayStarted) {
				transform.localPosition += m_movement * Time.deltaTime;
				transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + (m_rotation * Time.deltaTime));
			}
		}

		public Vector3 m_rotation;
		public Vector3 m_movement;
		public AudioSource m_Crashsnd;
	}
}