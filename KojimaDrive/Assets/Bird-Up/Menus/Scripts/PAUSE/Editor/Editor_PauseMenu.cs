//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Toggle pause within the editor
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;

namespace Bird {
	[System.Serializable]
	public class Editor_PauseMenu : EditorWindow {
		[MenuItem("Kojima Drive/Open Pause Window...")]
		public static void ShowWindow() {
			EditorWindow.GetWindow(typeof(Editor_PauseMenu));
		}

		public void OnEnable() {
			titleContent.text = "Pause";
			titleContent.tooltip = "Alter paused state of the game.";
		}

		private void Update() {
			Repaint();
		}

		Vector2 m_scrollPos = new Vector2(0, 0);
		int nScore = 0;
		int nPlayer = 0;
		string nScoreStr = "";
		float fSpeed = 4.0f;
		float fHoldTheLerpPlease = 2.0f;
		void OnGUI() {
			m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos);
			//EditorGUILayout.LabelField("", EditorStyles.largeLabel);
			for (int i = 0; i < Kojima.GameController.s_nMaxPlayers; i++) {
				if (GUILayout.Button("TOGGLE PAUSE (PLAYER " + (i + 1).ToString() + ")")) {
					UI_PauseMenu.ToggleGamePause(!UI_PauseMenu.GamePaused, i + 1);
				}
			}

			EditorGUILayout.LabelField("Current Timescale: " + Time.timeScale.ToString());

			EditorGUILayout.LabelField("SCORING DEBUG:");
			nScore = EditorGUILayout.IntField("Score To Give", nScore);
			nPlayer = EditorGUILayout.IntField("To Player (0 - 3)", nPlayer);
			nScoreStr = EditorGUILayout.TextField("Reason", nScoreStr);
			fSpeed = EditorGUILayout.FloatField("Speed", fSpeed);
			fHoldTheLerpPlease = EditorGUILayout.FloatField("Hold Lerp", fHoldTheLerpPlease);

			if (GUILayout.Button("GIVE SCORE")) {
				HF.PlayerExp.AddEXP(nPlayer, nScore, true, true, nScoreStr, true, fSpeed, fHoldTheLerpPlease);
			}
			EditorGUILayout.EndScrollView();
		}
	}
}