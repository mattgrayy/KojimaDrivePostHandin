//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Active PostFX Viewer
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;

namespace Bird {
	[System.Serializable]
	public class PostFXStackManager : EditorWindow {
		[MenuItem("Kojima Drive/Open PostFX Manager...")]
		public static void ShowWindow() {
			EditorWindow.GetWindow(typeof(PostFXStackManager));
		}

		public void OnEnable() {
			titleContent.text = "PostFX Stack Manager";
			titleContent.tooltip = "View all active PostFXStacks.";
		}

		private void Update() {
			Repaint();
		}

		Vector2 m_scrollPos = new Vector2(0, 0);
		void OnGUI() {
			PostFXStack[] pfxstacks = GameObject.FindObjectsOfType<PostFXStack>();

			m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos);
			EditorGUILayout.LabelField("Active PostFX Stacks:", EditorStyles.largeLabel);
			if (pfxstacks == null || pfxstacks.Length == 0) {
				EditorGUILayout.HelpBox("No active PostFX Stacks", MessageType.Info);
				EditorGUILayout.EndScrollView();
				return;
			}

			for (int i = 0; i < pfxstacks.Length; i++) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(pfxstacks[i].m_strGlobalName, EditorStyles.boldLabel);
				EditorGUILayout.ObjectField(pfxstacks[i], typeof(PostFXStack), true);
				EditorGUILayout.EndHorizontal();

				Camera cam = pfxstacks[i].GetComponent<Camera>();
				if (cam) {
					EditorGUILayout.LabelField("Render Layer: " + cam.depth);
					EditorGUILayout.LabelField("Render Rect: " + cam.rect.ToString());
				} else {
					EditorGUILayout.LabelField("Render Layer: UNKNOWN (no camera found)");
					EditorGUILayout.LabelField("Render Rect: UNKNOWN (no camera found)");
				}
			
				PostFXStack_Editor.DrawPostFXStackList(pfxstacks[i], 10.0f);
				if (i != pfxstacks.Length - 1) {
					GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
				}
			}

			EditorGUILayout.EndScrollView();
		}
	}
}