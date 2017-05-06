//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Editor script for the PostFX Stack
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace Bird {
	[CustomEditor(typeof(PostFXStack))]
	public class PostFXStack_Editor : Editor {
		void OnEnable() {
			
		}

		static float m_fArrowWidth = 25.0f;

		public static void DrawPostFXStackList(PostFXStack pfxStack, float fPadding = 0.0f) {
			for (int i = 0; i < pfxStack.m_PostFXStack.Count; i++) {
				EditorGUILayout.BeginHorizontal();
				if (fPadding != 0.0f) {
					EditorGUILayout.LabelField("", GUILayout.Width(fPadding));
				}

				string strToggle = " ";
				if (pfxStack.m_PostFXStack[i].m_UniqueName != "") {
					strToggle += pfxStack.m_PostFXStack[i].m_UniqueName;
				} else {
					strToggle += "(Shader:" + pfxStack.m_PostFXStack[i].m_Shader.name + ")";
				}

				if (i != 0) {
					if (GUILayout.Button("^", GUILayout.Width(m_fArrowWidth))) {
						PostFXObject swapper = pfxStack.m_PostFXStack[i];
						pfxStack.m_PostFXStack[i] = pfxStack.m_PostFXStack[i - 1];
						pfxStack.m_PostFXStack[i - 1] = swapper;
						return;
					}
				} else {
					GUI.enabled = false;
					GUILayout.Button("^", GUILayout.Width(m_fArrowWidth));
					GUI.enabled = true;
				}

				if (i != pfxStack.m_PostFXStack.Count - 1) {
					if (GUILayout.Button("v", GUILayout.Width(m_fArrowWidth))) {
						PostFXObject swapper = pfxStack.m_PostFXStack[i];
						pfxStack.m_PostFXStack[i] = pfxStack.m_PostFXStack[i + 1];
						pfxStack.m_PostFXStack[i + 1] = swapper;
						return;
					}
				} else {
					GUI.enabled = false;
					GUILayout.Button("v", GUILayout.Width(m_fArrowWidth));
					GUI.enabled = true;
				}

				if (GUILayout.Button("x", GUILayout.Width(m_fArrowWidth))) {
					pfxStack.RemovePostFX(pfxStack.m_PostFXStack[i]);
					return;
				}

				pfxStack.m_PostFXStack[i].m_bEnabled = GUILayout.Toggle(pfxStack.m_PostFXStack[i].m_bEnabled, strToggle);
				EditorGUILayout.EndHorizontal();
			}
		}

		public override void OnInspectorGUI() {
			PostFXStack pfxStack = (PostFXStack)target;

			pfxStack.m_strGlobalName = EditorGUILayout.TextField("Static Name", pfxStack.m_strGlobalName);
			pfxStack.m_bDontDestroyOnLoad = EditorGUILayout.Toggle("Don't Destroy On Load", pfxStack.m_bDontDestroyOnLoad);
			pfxStack.m_bOverwriteClashedInstance = EditorGUILayout.Toggle("Overwrite Clashed Instances", pfxStack.m_bOverwriteClashedInstance);
			pfxStack.m_bSetupOnAwake = EditorGUILayout.Toggle("Setup On Awake", pfxStack.m_bSetupOnAwake);

			if (pfxStack.m_PostFXStack == null) {
				pfxStack.m_PostFXStack = new List<PostFXObject>();
			}

			if(pfxStack.m_PostFXStack.Count == 0) {
				EditorGUILayout.HelpBox("No PostFXObjects in PostFXStack!", MessageType.Error);
			}

			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("PostFXObjects (in order of render)", EditorStyles.boldLabel);
			DrawPostFXStackList(pfxStack);
			/*for(int i = 0; i < pfxStack.m_PostFXStack.Count; i++) {
				EditorGUILayout.BeginHorizontal();
				string strToggle = " ";
				if(pfxStack.m_PostFXStack[i].m_UniqueName != "") {
					strToggle += pfxStack.m_PostFXStack[i].m_UniqueName;
				} else {
					strToggle += "(Shader:" + pfxStack.m_PostFXStack[i].m_Shader.name + ")";
				}

				if (i != 0) {
					if (GUILayout.Button("^", GUILayout.Width(m_fArrowWidth))) {
						PostFXObject swapper = pfxStack.m_PostFXStack[i];
						pfxStack.m_PostFXStack[i] = pfxStack.m_PostFXStack[i - 1];
						pfxStack.m_PostFXStack[i - 1] = swapper;
						return;
					}
				} else {
					GUI.enabled = false;
					GUILayout.Button("^", GUILayout.Width(m_fArrowWidth));
					GUI.enabled = true;
				}

				if (i != pfxStack.m_PostFXStack.Count - 1) {
					if (GUILayout.Button("v", GUILayout.Width(m_fArrowWidth))) {
						PostFXObject swapper = pfxStack.m_PostFXStack[i];
						pfxStack.m_PostFXStack[i] = pfxStack.m_PostFXStack[i + 1];
						pfxStack.m_PostFXStack[i + 1] = swapper;
						return;
					}
				} else {
					GUI.enabled = false;
					GUILayout.Button("v", GUILayout.Width(m_fArrowWidth));
					GUI.enabled = true;
				}

				if (GUILayout.Button("x", GUILayout.Width(m_fArrowWidth))) {
					pfxStack.RemovePostFX(pfxStack.m_PostFXStack[i]);
					return;
				}

				pfxStack.m_PostFXStack[i].m_bEnabled = GUILayout.Toggle(pfxStack.m_PostFXStack[i].m_bEnabled, strToggle);
				EditorGUILayout.EndHorizontal();
			}*/

			PostFXObject newObj = (PostFXObject)EditorGUILayout.ObjectField("Add PostFXObject:", null, typeof(PostFXObject), true);
			if(newObj != null) {
				pfxStack.AddPostFX(newObj);
			}

			// TODO: Only do this on modify -sam 20/03/2017
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
		}
	}
}