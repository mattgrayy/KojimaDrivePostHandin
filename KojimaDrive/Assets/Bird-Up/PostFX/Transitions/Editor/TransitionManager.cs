//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Transition Viewer
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;

namespace Bird {
	[System.Serializable]
	public class TransitionManager : EditorWindow {
		[MenuItem("Kojima Drive/Open Transition Manager...")]
		public static void ShowWindow() {
			EditorWindow.GetWindow(typeof(TransitionManager));
		}

		public void OnEnable() {
			titleContent.text = "Transition Manager";
			titleContent.tooltip = "View all active Transitions and Transition Channels.";
		}

		private void Update() {
			Repaint();	
		}

		Vector2 m_scrollPos = new Vector2(0, 0);
		void OnGUI() {
			m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos);
			if (!Application.isPlaying) {
				EditorGUILayout.HelpBox("Game is not running - transitions cannot be active in Edit Mode.", MessageType.Info);
				EditorGUILayout.EndScrollView();
				return;
			}

			if (TransitionController.s_CurrentlyActiveTransitions == null) {
				if (Application.isPlaying) {
					EditorGUILayout.HelpBox("No transitions have been started - No info to display", MessageType.Info);
				}
				EditorGUILayout.EndScrollView();
				return;
			}

			EditorGUILayout.LabelField("Currently active transition channels:", EditorStyles.largeLabel);
			foreach (KeyValuePair<string, BaseTransition> entry in TransitionController.s_CurrentlyActiveTransitions) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(entry.Key);
				EditorGUILayout.ObjectField(entry.Value, typeof(BaseTransition), true);
				if (entry.Value == null) {
					GUI.enabled = false;
				}
				if (GUILayout.Button("Stop")) {
					entry.Value.StopTransition();
					EditorGUILayout.EndScrollView();
					return;
				}
				GUI.enabled = true;
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndScrollView();
		}
	}
}