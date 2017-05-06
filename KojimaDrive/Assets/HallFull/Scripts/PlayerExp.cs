using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// NOTE:
// Hi! Sam (SpAMCAN) here. I'm rewriting parts of this so that it can tie in to the HUD and fit the game. 18/04/2017
// CHANGES:
// - Commented out milestones, as we're not using them for anything at the moment
// - Changed "passExpToXXX" functions into a single static function and a single local function
//	 Previously, this was basically set up as a set of four static functions that did weird unnecessary stuff with loops...
// - Removed leveling, as this is not used and isn't required.
// - Added static accessors

namespace HF {
	public class PlayerExp : MonoBehaviour {
		// Experience points in this game mode
		[SerializeField]
		private int m_nCurrentExperience = 0;

		public int CurrentEXP {
			get {
				return m_nCurrentExperience;
			}
		}

		private Kojima.CarScript m_Player = null;

		void Start() {
			m_Player = GetComponent<Kojima.CarScript>();
		}

		public void AddEXP(int nScore, bool bNotifyHUD = true, bool bShowPopup = true, string strScoreReason = null, bool bPlaySound = true, float fSpeed = -1, float fHoldTime = -1) {
			m_nCurrentExperience += nScore;

			// For some reason m_nplayerIndex is 1-4 not 0-3...
			ExperienceManager.AddToSessionEXP(m_Player.m_nplayerIndex - 1, nScore, true);

			if(bNotifyHUD) {
				Bird.HUD_EXP.hudEXPData_t data = new Bird.HUD_EXP.hudEXPData_t();
				data.m_nTargetPlayerID = m_Player.m_nplayerIndex;
				Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_SHOW_EXP, data);

				Bird.HUD_ScorePopupMgr.hudScorePopupData_t data2 = new Bird.HUD_ScorePopupMgr.hudScorePopupData_t();
				data2.m_bUseCustomColours = false;
				data2.m_HSV = Vector4.zero;
				data2.m_nTargetPlayerID = m_Player.m_nplayerIndex;
				data2.m_nXP = nScore;
				data2.m_strReason = strScoreReason;
				data2.m_bPlaySound = bPlaySound;
				data2.m_fSpeed = fSpeed;
				data2.m_fHoldTime = fHoldTime;
				Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_SHOW_EXP_POPUP, data2);
			}
		}

		public void ResetEXP() {
			m_nCurrentExperience = 0;
		}


		#region Static Accessors
		public static PlayerExp GetPlayerEXP(int nPlayer) {
			// This is a slightly tortured way of going about all this, but it was quickest to write...
			Kojima.CarScript car = Kojima.GameController.s_singleton.m_players[nPlayer];
			if (car != null) {
				return car.PlayerEXP;
			}

			return null;
		}

		// Returns -1 if player/score is not found
		public static int GetCurrentEXP(int nPlayer) {
			PlayerExp exptracker = GetPlayerEXP(nPlayer);
			if(exptracker) {
				return exptracker.m_nCurrentExperience;
			}

			return -1;
		}

		public static void AddEXP(int nPlayer, int nScore, bool bNotifyHUD = true, bool bShowPopup = true, string strScoreReason = null, bool bPlaySound = true, float fSpeed = -1, float fHoldTime = -1) {
			PlayerExp exptracker = GetPlayerEXP(nPlayer);
			if (exptracker) {
				exptracker.AddEXP(nScore, bNotifyHUD, bShowPopup, strScoreReason, bPlaySound, fSpeed, fHoldTime);
			}
		}

		public static void ResetCurrentEXP() {
			for (int i = 0; i < Kojima.GameController.s_ncurrentPlayers; i++) {
				PlayerExp exptracker = GetPlayerEXP(i);
				if (exptracker) {
					exptracker.ResetEXP();
				}
			}
		}
		#endregion
	}
}

