using UnityEngine;
using System.Collections;

// Hello! Sam (SpAMCAN) again. Just gonna go ahead and rework this to be a static class; it has no reason to be a gameobject component. 18/04/2017
// CHANGES:
// - This is now a static class. This never needed to be a gameobject component.
// - Generalized functionality into separate functions.
// - Added load function.
// - Individual player score doesn't need to be saved across sessions, only the global score.

/// <summary>Empty class so as not to break scenes that have this in.</summary>
class ExperienceManager : MonoBehaviour {

}

namespace HF {
	/// <summary>Session/global experience manager.</summary>
	public static class ExperienceManager {
		
		// Per-player score, carried over between game types. This is NOT carried over between sessions.
		private static int[] m_nSessionEXPStore = new int[Kojima.GameController.s_nMaxPlayers];

		// Global pool of points used to unlock things - carries over between sessions.
		private static int m_nGlobalEXPStore = 0;

		public static int[] SessionEXP {
			get {
				return m_nSessionEXPStore;
			}
			set {
				m_nSessionEXPStore = value;
			}
		}

		public static int GlobalEXP {
			set {
				m_nGlobalEXPStore = value;
				if(m_nGlobalEXPStore < 0) {
					m_nGlobalEXPStore = 0; // We don't want negative scores.
				}
				Save();
			}
			get {
				return m_nGlobalEXPStore;
			}
		}

		// I honestly don't know how to feel about using PlayerPrefs here - it saves to the Windows Registry...
		static string m_strGlobalEXPPrefsName = "GLOBAL_EXP";
		public static void Load() {
			if (PlayerPrefs.HasKey(m_strGlobalEXPPrefsName)) {
				m_nGlobalEXPStore = PlayerPrefs.GetInt(m_strGlobalEXPPrefsName);
				if(m_nGlobalEXPStore < 0) {
					m_nGlobalEXPStore = 0; // Prevent negative EXP values
				}
			}
		}

		public static void ResetGlobalEXP() {
			m_nGlobalEXPStore = 0;
			Save();
		}

		public static void Save() {
			PlayerPrefs.SetInt(m_strGlobalEXPPrefsName, m_nGlobalEXPStore);
		}

		public static int GetSessionEXP(int nPlayer) {
			return m_nSessionEXPStore[nPlayer];
		}

		public static void ResetSessionEXP(int nPlayer) {
			m_nSessionEXPStore[nPlayer] = 0;
		}

		public static void ResetSessionEXPAll() {
			for(int i = 0; i < Kojima.GameController.s_nMaxPlayers; i++) {
				m_nSessionEXPStore[i] = 0;
			}
		}

		public static void AddToSessionEXP(int nPlayer, int nScore, bool bAddToGlobal = true) {
			int nOld = m_nSessionEXPStore[nPlayer];
			m_nSessionEXPStore[nPlayer] += nScore;

			if (bAddToGlobal) {
				int nDiff = m_nSessionEXPStore[nPlayer] - nOld;
				m_nGlobalEXPStore += nDiff;
				Save();
			}
		}

	}
}