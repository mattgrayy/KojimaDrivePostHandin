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
	public class MenuSounder : MonoBehaviour {

		#region Singleton
		public static MenuSounder s_pMenuSounder;
		void SetupSingleton() {
			if (s_pMenuSounder == null) {
				//ObjectDB.DontDestroyOnLoad_Managed(gameObject);
				s_pMenuSounder = this;
			} else if (s_pMenuSounder != this) {
				Debug.Log("More than one menu sounder detected. You only need one in the scene.");
				//Destroy(gameObject);
			}
		}

		public static MenuSounder MenuSounds {
			get {
				return s_pMenuSounder;
			}
		}
		#endregion

		// Using OnEnable instead of Start since that happens first!
		void OnEnable() {
			SetupSingleton(); // Setup our singleton
		}

		public enum menuSounds_e {
			MS_MOVE_UP,
			MS_MOVE_DOWN,
			MS_SELECT,
			MS_BACK,
			MS_TOGGLE,
			MS_ERROR,

			MS_LAST
		}

		[NotNull]
		public AudioSource m_Src;
		[NotNull]
		public AudioClip m_MoveClipUp;
		[NotNull]
		public AudioClip m_MoveClipDown;
		[NotNull]
		public AudioClip m_SelectClip;
		[NotNull]
		public AudioClip m_BackClip;
		[NotNull]
		public AudioClip m_ToggleClip;
		[NotNull]
		public AudioClip m_ErrorClip;

		public void DoMenuSound(menuSounds_e snd) {
			switch (snd) {
				case menuSounds_e.MS_MOVE_UP:
					m_Src.PlayOneShot(m_MoveClipUp);
					break;
				case menuSounds_e.MS_MOVE_DOWN:
					m_Src.PlayOneShot(m_MoveClipDown);
					break;
				case menuSounds_e.MS_SELECT:
					m_Src.PlayOneShot(m_SelectClip);
					break;
				case menuSounds_e.MS_BACK:
					m_Src.PlayOneShot(m_BackClip);
					break;
				case menuSounds_e.MS_TOGGLE:
					m_Src.PlayOneShot(m_ToggleClip);
					break;
				case menuSounds_e.MS_ERROR:
					m_Src.PlayOneShot(m_ErrorClip);
					break;
			}
		}
	}
}