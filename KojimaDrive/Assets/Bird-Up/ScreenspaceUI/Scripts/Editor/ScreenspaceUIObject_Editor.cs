//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Editor script for the Screenspace UI objects.
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Bird {
	[CustomEditor(typeof(ScreenspaceUIObject))]
	[CanEditMultipleObjects]
	public class ScreenspaceUIObject_Editor : Editor {

		SerializedProperty m_Master;
		void OnEnable() {
			m_Master = serializedObject.FindProperty("m_Master");
		}
		public override void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUILayout.PropertyField(m_Master);
			EditorGUILayout.Separator();

			EditorGUILayout.LabelField("Adjust Mode:", EditorStyles.boldLabel);
			System.Array arr = System.Enum.GetValues(typeof(ScreenspaceUIObject.adjustMode_e));
			foreach (ScreenspaceUIObject.adjustMode_e curState in arr) {
				if (curState == ScreenspaceUIObject.adjustMode_e.ADJUST_NONE) {
					continue;
				}

				bool[] bStates = new bool[targets.Length];
				int nCur = 0;
				int i = 0;
				foreach (ScreenspaceUIObject obj in targets) {
					bStates[i] = (obj.m_eAdjustMode & curState) == curState;
					nCur += bStates[i] ? 1 : 0;
					++i;
				}

				bool bButtonState = bStates[0];
				bool bNewButtonState = bStates[0];
				if (nCur == 0 || nCur >= targets.Length) {
					// Normal tickbox
					bNewButtonState = EditorGUILayout.Toggle(curState.ToString(), bStates[0]);
				} else {
					// Mixed tickbox
					bNewButtonState = EditorGUILayout.Toggle(curState.ToString(), bStates[0], new GUIStyle("ToggleMixed"));
				}

				if (bButtonState != bNewButtonState) {
					foreach (ScreenspaceUIObject obj in targets) {
						if (bNewButtonState) {
							obj.m_eAdjustMode |= curState;
						} else {
							obj.m_eAdjustMode &= ~curState;
						}
					}
				}
			}

			EditorGUILayout.Separator();

			// TODO: Make this a proper function
			{
				bool[] bAdjustTypo = new bool[targets.Length];
				int nCurTypo = 0;
				int j = 0;
				foreach (ScreenspaceUIObject obj in targets) {
					bAdjustTypo[j] = obj.m_bAdjustTypogenicWordwrap;
					nCurTypo += bAdjustTypo[j] ? 1 : 0;
					++j;
				}
				bool bButtonStateTypo = bAdjustTypo[0];
				bool bNewButtonStateTypo = bAdjustTypo[0];
				if (nCurTypo == 0 || nCurTypo >= targets.Length) {
					// Normal tickbox
					bNewButtonStateTypo = EditorGUILayout.Toggle("Scale Typogenic Word Wrap", bAdjustTypo[0]);
				} else {
					// Mixed tickbox
					bNewButtonStateTypo = EditorGUILayout.Toggle("Scale Typogenic Word Wrap", bAdjustTypo[0], new GUIStyle("ToggleMixed"));
				}

				if (bButtonStateTypo != bNewButtonStateTypo) {
					foreach (ScreenspaceUIObject obj in targets) {
						obj.m_bAdjustTypogenicWordwrap = bNewButtonStateTypo;
					}
				}
			}

			{
				bool[] bAdjustTypo = new bool[targets.Length];
				int nCurTypo = 0;
				int j = 0;
				foreach (ScreenspaceUIObject obj in targets) {
					bAdjustTypo[j] = obj.m_bAdjustTypogenicCharacterSize;
					nCurTypo += bAdjustTypo[j] ? 1 : 0;
					++j;
				}
				bool bButtonStateTypo = bAdjustTypo[0];
				bool bNewButtonStateTypo = bAdjustTypo[0];
				if (nCurTypo == 0 || nCurTypo >= targets.Length) {
					// Normal tickbox
					bNewButtonStateTypo = EditorGUILayout.Toggle("Scale Typogenic Character Size", bAdjustTypo[0]);
				} else {
					// Mixed tickbox
					bNewButtonStateTypo = EditorGUILayout.Toggle("Scale Typogenic Character Size", bAdjustTypo[0], new GUIStyle("ToggleMixed"));
				}

				if (bButtonStateTypo != bNewButtonStateTypo) {
					foreach (ScreenspaceUIObject obj in targets) {
						obj.m_bAdjustTypogenicCharacterSize = bNewButtonStateTypo;
					}
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

	}
}