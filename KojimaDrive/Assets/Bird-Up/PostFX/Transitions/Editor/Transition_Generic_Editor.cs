//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Editor script for the Generic Transition Controller
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace Bird {
	[CustomEditor(typeof(Transition_Generic))]
	public class Transition_Generic_Editor : Editor {
		public string m_strString;
		public void OnEnable() {
			m_strString = "";
		}

		GUIContent tooltip_affected = new GUIContent("M", "Modified By Transition");
		GUIContent tooltip_immediate = new GUIContent("L", "Use Lerp");
		GUIContent tooltip_curve = new GUIContent("C", "Use Animation Curve");
		string[] curve_names_colour = new string[4] { "R", "G", "B", "A" };
		string[] curve_names_vector = new string[4] { "X", "Y", "Z", "W" };
		Color[] curve_colors = new Color[4] { Color.red, Color.green, Color.blue, Color.white };

		bool m_bFoldoutUIListeners = false;

		public override void OnInspectorGUI() {
			Transition_Generic transTarget = (Transition_Generic)target;
			if (transTarget.m_TargetPostFXObjects == null) {
				transTarget.m_TargetPostFXObjects = new List<Transition_Generic.targetPostFX_s>();
			}

			transTarget.m_strTargetPostFXStack = EditorGUILayout.TextField(new GUIContent("Target PostFX Stack", "Which PostFX Stack to modify (by global name)"), transTarget.m_strTargetPostFXStack);
			PostFXStack pfxs = PostFXStack.GetPostFXStack(transTarget.m_strTargetPostFXStack);
			if (pfxs == null) {
				EditorGUILayout.HelpBox("ERROR: Cannot find PFXStack called '" + transTarget.m_strTargetPostFXStack + "'!", MessageType.Error);
			} else {
				GUI.enabled = false;
				EditorGUILayout.ObjectField("PFXStack:", pfxs, typeof(PostFXStack), true);
				GUI.enabled = true;
			}

			{
				if (!Application.isPlaying) {
					GUI.enabled = false;
				}

				if (transTarget.TransitionActive) {
					GUI.enabled = false;
					GUILayout.Button(transTarget.m_bPlayInReverse ? "Perform Transition" : "Transition Active");
					GUILayout.Button(transTarget.m_bPlayInReverse ? "Transition Active" : "Perform Transition (Reverse)");
				} else {
					if (GUILayout.Button("Perform Transition")) {
						transTarget.StartTransition();
						return;
					}

					if (GUILayout.Button("Perform Transition (Reverse)")) {
						transTarget.StartTransitionReverse();
						return;
					}
				}

				GUI.enabled = true;
			}

			EditorGUILayout.LabelField("Transition Details", EditorStyles.boldLabel);
			transTarget.TransitionChannel = EditorGUILayout.TextField(new GUIContent("Transition Channel", "Will override any other transitions on this channel when started"), transTarget.TransitionChannel);
			transTarget.m_fTransitionTime = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Transition Time", "Time to transition (seconds)"), transTarget.m_fTransitionTime), 0.0001f, 10000000);
			transTarget.m_bTransitionOnStart = EditorGUILayout.Toggle("Auto Transition On Start", transTarget.m_bTransitionOnStart);
			transTarget.m_bLockInputWhileRunning = EditorGUILayout.Toggle("Lock Menu Input While Running", transTarget.m_bLockInputWhileRunning);
			transTarget.m_bLockPlayerInputWhileRunning = EditorGUILayout.Toggle("Lock Player Input While Running", transTarget.m_bLockPlayerInputWhileRunning);
			transTarget.m_bUseUnscaledTime = EditorGUILayout.Toggle("Use Unscaled Time", transTarget.m_bUseUnscaledTime);
			transTarget.m_bRevertToInitialValuesOnCompletion = EditorGUILayout.Toggle("Revert To Initial Values On Completion", transTarget.m_bRevertToInitialValuesOnCompletion);
			transTarget.m_bDestroySelfOnCompletion = EditorGUILayout.Toggle("Destroy Self On Completion", transTarget.m_bDestroySelfOnCompletion);
			GUI.enabled = transTarget.m_bDestroySelfOnCompletion;
			transTarget.m_bDestroyGOOnCompletion = EditorGUILayout.Toggle("Destroy GameObject", transTarget.m_bDestroyGOOnCompletion);
			GUI.enabled = false;
			EditorGUILayout.Slider("Lerp Value", transTarget.m_fLerpState, 0, 1.0f);
			EditorGUILayout.LabelField("Transition Running For " + transTarget.m_fTransitionProgress.ToString() + " secs.");
			GUI.enabled = true;

			// Event stuff
			EditorGUILayout.LabelField("");
			EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
			transTarget.m_bFireKJDEvents = EditorGUILayout.Toggle("Fire KDJ Events", transTarget.m_bFireKJDEvents);
			transTarget.m_bCustomKJDCompletionEvent = EditorGUILayout.Toggle("Fire Custom KJD Completion Event", transTarget.m_bCustomKJDCompletionEvent);
			if (transTarget.m_bCustomKJDCompletionEvent) {
				transTarget.m_eCustomCompletionEvent = (Kojima.Events.Event)EditorGUILayout.EnumPopup(transTarget.m_eCustomCompletionEvent);
			}

			if(transTarget.m_UIEventListeners == null) {
				transTarget.m_UIEventListeners = new List<UI_Transition_Listener>();
			}

			EditorGUILayout.BeginHorizontal();
			m_bFoldoutUIListeners = EditorGUILayout.Toggle(m_bFoldoutUIListeners, EditorStyles.foldout, GUILayout.Width(10.0f));
			if(GUILayout.Button("UI Listeners", EditorStyles.label)) {
				m_bFoldoutUIListeners = !m_bFoldoutUIListeners;
			}
			EditorGUILayout.EndHorizontal();

			if (m_bFoldoutUIListeners) {
				for (int i = 0; i < transTarget.m_UIEventListeners.Count; i++) {
					EditorGUILayout.BeginHorizontal();
					transTarget.m_UIEventListeners[i] = (UI_Transition_Listener)EditorGUILayout.ObjectField("    Listener " + i.ToString(), transTarget.m_UIEventListeners[i], typeof(UI_Transition_Listener), true);
					if(GUILayout.Button("x", GUILayout.Width(25.0f))) {
						transTarget.m_UIEventListeners.Remove(transTarget.m_UIEventListeners[i]);
						EditorGUILayout.EndHorizontal();
						break;
					}
					EditorGUILayout.EndHorizontal();
				}

				UI_Transition_Listener newListener = (UI_Transition_Listener)EditorGUILayout.ObjectField("Add Transition Listener", null, typeof(UI_Transition_Listener), true);
				if (newListener) {
					transTarget.m_UIEventListeners.Add(newListener);
				}
			}
			

			EditorGUILayout.LabelField("");

			EditorGUILayout.LabelField("Transition Targets", EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Add")) {
				Transition_Generic.targetPostFX_s newTarget = new Transition_Generic.targetPostFX_s();
				transTarget.m_TargetPostFXObjects.Add(newTarget);
			}
			if (GUILayout.Button("Clear List")) {
				transTarget.m_TargetPostFXObjects.Clear();
				return;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Separator();

			for (int i = 0; i < transTarget.m_TargetPostFXObjects.Count; i++) {
				if (i != 0) {
					GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
				}
				EditorGUILayout.BeginHorizontal();
				transTarget.m_TargetPostFXObjects[i].m_bFoldoutOpen = EditorGUILayout.Toggle(transTarget.m_TargetPostFXObjects[i].m_bFoldoutOpen, EditorStyles.foldout, GUILayout.Width(10.0f));
				transTarget.m_TargetPostFXObjects[i].m_strPostFXStackMember = EditorGUILayout.TextField("", transTarget.m_TargetPostFXObjects[i].m_strPostFXStackMember);
				transTarget.m_TargetPostFXObjects[i].m_PostFXStackMember = pfxs.GetPostFX(transTarget.m_TargetPostFXObjects[i].m_strPostFXStackMember);
				bool bPostFXObjValid = transTarget.m_TargetPostFXObjects[i].m_PostFXStackMember != null && (transTarget.m_TargetPostFXObjects[i].m_PostFXStackMember.GetType() == typeof(PostFX_Generic));

				if (!bPostFXObjValid) {
					GUI.enabled = false;
				}
				if (GUILayout.Button("Populate")) {
					transTarget.m_TargetPostFXObjects[i].m_EndState = new List<PostFX_Generic.shaderProperty_t>();
					for (int j = 0; j < ((PostFX_Generic)transTarget.m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps.Count; j++) {
						transTarget.m_TargetPostFXObjects[i].m_EndState.Add(new PostFX_Generic.shaderProperty_t(((PostFX_Generic)transTarget.m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps[j]));
						transTarget.m_TargetPostFXObjects[i].m_EndState[j].ResetToDefaults();
					}

					transTarget.m_TargetPostFXObjects[i].m_strLastValidName = transTarget.m_TargetPostFXObjects[i].m_strPostFXStackMember;
				}
				if (GUILayout.Button("Reset")) {
					for (int j = 0; j < transTarget.m_TargetPostFXObjects[i].m_EndState.Count; j++) {
						transTarget.m_TargetPostFXObjects[i].m_EndState[j].ResetToDefaults();
					}
				}
				if (!bPostFXObjValid) {
					GUI.enabled = true;
				}

				if (GUILayout.Button("X")) {
					transTarget.m_TargetPostFXObjects.RemoveAt(i);
					return;
				}
				EditorGUILayout.EndHorizontal();

				if (transTarget.m_TargetPostFXObjects[i].m_bFoldoutOpen) {
					if (transTarget.m_TargetPostFXObjects[i].m_PostFXStackMember == null) {
						EditorGUILayout.HelpBox("ERROR: PostFXObject called '" + transTarget.m_TargetPostFXObjects[i].m_strPostFXStackMember + "' not found!", MessageType.Error);
						continue;
					} else if (transTarget.m_TargetPostFXObjects[i].m_PostFXStackMember.GetType() == typeof(PostFX_Generic)) {
						GUI.enabled = false;
						EditorGUILayout.ObjectField("PFXStack:", pfxs, typeof(PostFXStack), true);
						GUI.enabled = true;
					} else {
						EditorGUILayout.HelpBox("ERROR: PostFXObject called '" + transTarget.m_TargetPostFXObjects[i].m_strPostFXStackMember + "' is not a PostFX_Generic!", MessageType.Error);
						continue;
					}


					EditorGUILayout.LabelField("Misc Properties", EditorStyles.boldLabel);
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(tooltip_affected, EditorStyles.miniLabel, GUILayout.Width(12f));
					EditorGUILayout.LabelField(tooltip_immediate, EditorStyles.miniLabel, GUILayout.Width(10f));
					EditorGUILayout.LabelField(tooltip_curve, EditorStyles.miniLabel, GUILayout.Width(11f));
					EditorGUILayout.LabelField("Property", EditorStyles.miniLabel);
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					transTarget.m_TargetPostFXObjects[i].m_bAlterPFXO = EditorGUILayout.Toggle(transTarget.m_TargetPostFXObjects[i].m_bAlterPFXO, GUILayout.Width(10.0f));
					GUI.enabled = false;
					EditorGUILayout.Toggle(false, GUILayout.Width(10.0f));
					EditorGUILayout.Toggle(false);
					GUI.enabled = transTarget.m_TargetPostFXObjects[i].m_bAlterPFXO;
					Rect rct = GUILayoutUtility.GetLastRect();
					rct.x += 15;
					rct.width = (Screen.width / 2) - rct.x;
					EditorGUI.LabelField(rct, "Enable PFX");
					rct.width = rct.width / 2;
					rct.x += rct.width * 1.825f;
					transTarget.m_TargetPostFXObjects[i].m_bEnablePFXO = EditorGUI.Toggle(rct, transTarget.m_TargetPostFXObjects[i].m_bEnablePFXO);
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
					GUI.enabled = true;
					transTarget.m_TargetPostFXObjects[i].m_bAlterPasses = EditorGUILayout.Toggle(transTarget.m_TargetPostFXObjects[i].m_bAlterPasses, GUILayout.Width(10.0f));
					GUI.enabled = transTarget.m_TargetPostFXObjects[i].m_bAlterPasses;
					if (transTarget.m_TargetPostFXObjects[i].m_bCurvePasses) {
						GUI.enabled = false;
					}
					transTarget.m_TargetPostFXObjects[i].m_bLerpPasses = EditorGUILayout.Toggle(transTarget.m_TargetPostFXObjects[i].m_bLerpPasses, GUILayout.Width(10.0f));
					GUI.enabled = transTarget.m_TargetPostFXObjects[i].m_bAlterPasses;
					if (transTarget.m_TargetPostFXObjects[i].m_bLerpPasses) {
						GUI.enabled = false;
					}
					transTarget.m_TargetPostFXObjects[i].m_bCurvePasses = EditorGUILayout.Toggle(transTarget.m_TargetPostFXObjects[i].m_bCurvePasses, GUILayout.Width(10.0f));
					GUI.enabled = transTarget.m_TargetPostFXObjects[i].m_bAlterPasses;
					if (transTarget.m_TargetPostFXObjects[i].m_bCurvePasses) {
						if (transTarget.m_TargetPostFXObjects[i].m_PassesCurve == null) {
							transTarget.m_TargetPostFXObjects[i].m_PassesCurve = new AnimationCurve();
						}

						transTarget.m_TargetPostFXObjects[i].m_PassesCurve = EditorGUILayout.CurveField("Iterations", transTarget.m_TargetPostFXObjects[i].m_PassesCurve,
							Color.white, new Rect(0, 0, 1, 100));

						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						Rect sliderrect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.colorField);
						EditorGUI.LabelField(sliderrect, "Preview");
						sliderrect.x += EditorGUIUtility.labelWidth + 40;
						sliderrect.width -= EditorGUIUtility.labelWidth + 40;

						transTarget.m_TargetPostFXObjects[i].m_fPassesCurvePreview = GUI.HorizontalSlider(sliderrect, transTarget.m_TargetPostFXObjects[i].m_fPassesCurvePreview, 0.0f, 1.0f);

						EditorGUILayout.EndHorizontal();
						sliderrect.x -= 75.0f;
						sliderrect.width = 70.0f;
						EditorGUI.LabelField(sliderrect, Mathf.RoundToInt(transTarget.m_TargetPostFXObjects[i].m_PassesCurve.Evaluate(transTarget.m_TargetPostFXObjects[i].m_fPassesCurvePreview)).ToString());
						EditorGUILayout.BeginHorizontal();
					} else {
						transTarget.m_TargetPostFXObjects[i].m_nEndPasses = Mathf.Clamp(EditorGUILayout.IntField("Iterations", transTarget.m_TargetPostFXObjects[i].m_nEndPasses), 0, 100);
					}
					GUI.enabled = true;
					EditorGUILayout.EndHorizontal();
					if (transTarget.m_TargetPostFXObjects[i].m_EndState == null) {
						// Not populated
						EditorGUILayout.HelpBox("ERROR: Shader property list is not populated!", MessageType.Info);
						continue;
					}

					if (transTarget.m_TargetPostFXObjects[i].m_strPostFXStackMember != transTarget.m_TargetPostFXObjects[i].m_strLastValidName) {
						// Stale
						EditorGUILayout.HelpBox("ERROR: Shader property list is stale; Please repopulate!", MessageType.Info);
						continue;
					}

					EditorGUILayout.LabelField("Shader Properties", EditorStyles.boldLabel);
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(tooltip_affected, EditorStyles.miniLabel, GUILayout.Width(12f));
					EditorGUILayout.LabelField(tooltip_immediate, EditorStyles.miniLabel, GUILayout.Width(10f));
					EditorGUILayout.LabelField(tooltip_curve, EditorStyles.miniLabel, GUILayout.Width(11f));
					EditorGUILayout.LabelField("Shader Property", EditorStyles.miniLabel);
					EditorGUILayout.EndHorizontal();

					for (int j = 0; j < transTarget.m_TargetPostFXObjects[i].m_EndState.Count; j++) {
						PostFX_Generic.shaderProperty_t property = transTarget.m_TargetPostFXObjects[i].m_EndState[j];
						EditorGUILayout.BeginHorizontal();
						property.m_bChangeInTransition = EditorGUILayout.Toggle(property.m_bChangeInTransition, GUILayout.Width(10.0f));
						GUI.enabled = property.m_bChangeInTransition;
						if (property.m_Type == PostFX_Generic.propertyType_e.TexEnv) {
							// Textures cannot be lerped or animated
							bool bState = GUI.enabled;
							GUI.enabled = false;
							property.m_bLerpInTransition = EditorGUILayout.Toggle(false, GUILayout.Width(10.0f));
							property.m_bUseAnimationCurves = EditorGUILayout.Toggle(false, GUILayout.Width(10.0f));
							GUI.enabled = bState;
						} else {
							bool bState = GUI.enabled;
							GUI.enabled = !property.m_bUseAnimationCurves && bState;
							property.m_bLerpInTransition = EditorGUILayout.Toggle(property.m_bLerpInTransition, GUILayout.Width(10.0f));

							GUI.enabled = !property.m_bLerpInTransition && bState;
							property.m_bUseAnimationCurves = EditorGUILayout.Toggle(property.m_bUseAnimationCurves, GUILayout.Width(10.0f));
							GUI.enabled = bState;
						}

						if (property.m_bUseAnimationCurves) {
							for (int c = 0; c < property.m_Curves.Length; c++) {
								if (property.m_Curves[c] == null) {
									property.m_Curves[c] = new AnimationCurve();
								}
							}

							switch (property.m_Type) {
								case PostFX_Generic.propertyType_e.Color: {
										EditorGUILayout.LabelField(property.m_PrettyName);
										EditorGUILayout.EndHorizontal();
										for (int c = 0; c < property.m_Curves.Length; c++) {
											property.m_Curves[c] = EditorGUILayout.CurveField(curve_names_colour[c], property.m_Curves[c], curve_colors[c], new Rect(0, 0, 1.0f, 1.0f));
										}

										EditorGUILayout.BeginHorizontal();
										Rect sliderrect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.colorField);
										EditorGUI.LabelField(sliderrect, "Preview");
										sliderrect.x += EditorGUIUtility.labelWidth;
										sliderrect.width -= EditorGUIUtility.labelWidth;

										property.m_fPreviewPosition = GUI.HorizontalSlider(sliderrect, property.m_fPreviewPosition, 0.0f, 1.0f);

										EditorGUILayout.EndHorizontal();
										Color prevCol = new Color();
										for (int c = 0; c < property.m_Curves.Length; c++) {
											prevCol[c] = property.m_Curves[c].Evaluate(property.m_fPreviewPosition);
										}
										bool bState = GUI.enabled;
										GUI.enabled = false;
										sliderrect.x -= 50.0f;
										sliderrect.width = 45.0f;
										EditorGUI.ColorField(sliderrect, GUIContent.none, prevCol, false, true, false, null);
										GUI.enabled = bState;

										EditorGUILayout.BeginHorizontal();
										break;
									}
								case PostFX_Generic.propertyType_e.Vector: {
										EditorGUILayout.LabelField(property.m_PrettyName);
										EditorGUILayout.EndHorizontal();

										for (int c = 0; c < property.m_Curves.Length; c++) {
											property.m_Curves[c] = EditorGUILayout.CurveField(curve_names_vector[c], property.m_Curves[c]/*, curve_colors[c], new Rect(0, -1000.0f, 1.0f, 999999999.0f * 2)*/);
										}

										EditorGUILayout.BeginHorizontal();
										Rect sliderrect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.colorField);
										EditorGUI.LabelField(sliderrect, "Preview");
										sliderrect.x += EditorGUIUtility.labelWidth;
										sliderrect.width -= EditorGUIUtility.labelWidth;

										property.m_fPreviewPosition = GUI.HorizontalSlider(sliderrect, property.m_fPreviewPosition, 0.0f, 1.0f);

										EditorGUILayout.EndHorizontal();
										Vector4 prevvec = new Vector4();
										for (int c = 0; c < property.m_Curves.Length; c++) {
											prevvec[c] = property.m_Curves[c].Evaluate(property.m_fPreviewPosition);
										}
										EditorGUILayout.LabelField(prevvec.ToString());
										EditorGUILayout.BeginHorizontal();
										break;
									}
								case PostFX_Generic.propertyType_e.Float: {
									EditorGUILayout.CurveField(property.m_PrettyName, property.m_Curves[0]/*, Color.white, new Rect(0, -999999999.0f, 1.0f, 999999999.0f * 2)*/);

										EditorGUILayout.EndHorizontal();
										EditorGUILayout.BeginHorizontal();
										Rect sliderrect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.colorField);
										EditorGUI.LabelField(sliderrect, "Preview");
										sliderrect.x += EditorGUIUtility.labelWidth + 40;
										sliderrect.width -= EditorGUIUtility.labelWidth + 40;

										property.m_fPreviewPosition = GUI.HorizontalSlider(sliderrect, property.m_fPreviewPosition, 0.0f, 1.0f);

										EditorGUILayout.EndHorizontal();
										sliderrect.x -= 75.0f;
										sliderrect.width = 70.0f;
										EditorGUI.LabelField(sliderrect, property.m_Curves[0].Evaluate(property.m_fPreviewPosition).ToString());
										EditorGUILayout.BeginHorizontal();
										break;
								}
								case PostFX_Generic.propertyType_e.Range: {
									EditorGUILayout.CurveField(property.m_PrettyName, property.m_Curves[0], Color.white, new Rect(0, property.m_fRangeMin, 1.0f, Mathf.Abs(0 - property.m_fRangeMin) + property.m_fRangeMax));
									 
									EditorGUILayout.EndHorizontal();
									EditorGUILayout.BeginHorizontal();
									Rect sliderrect = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.colorField);
									EditorGUI.LabelField(sliderrect, "Preview");
									sliderrect.x += EditorGUIUtility.labelWidth + 40;
									sliderrect.width -= EditorGUIUtility.labelWidth + 40;

									property.m_fPreviewPosition = GUI.HorizontalSlider(sliderrect, property.m_fPreviewPosition, 0.0f, 1.0f);

									EditorGUILayout.EndHorizontal();
									sliderrect.x -= 75.0f;
									sliderrect.width = 70.0f;
									EditorGUI.LabelField(sliderrect, property.m_Curves[0].Evaluate(property.m_fPreviewPosition).ToString());
									EditorGUILayout.BeginHorizontal();
									break;
								}
								case PostFX_Generic.propertyType_e.TexEnv:
								default:
									EditorGUILayout.HelpBox("ERROR: Cannot animate texture! How did you set this?", MessageType.Error);
									break;
							}

							EditorGUILayout.EndHorizontal();
							property.m_bSetFirstFrameToInitialState = EditorGUILayout.Toggle(new GUIContent("First Frame To Initial State",
								"First frame of your curve will be set to the state of the PostFX values on transition start"),
								property.m_bSetFirstFrameToInitialState);

							property.m_bAddToInitialState = EditorGUILayout.Toggle(new GUIContent("Additive",
								"Add to the values of the PostFX on transition start"), property.m_bAddToInitialState);
							EditorGUILayout.BeginHorizontal();
						} else {
							switch (property.m_Type) {
								case PostFX_Generic.propertyType_e.Color:
									property.m_colVal = EditorGUILayout.ColorField(property.m_PrettyName, property.m_colVal);
									break;
								case PostFX_Generic.propertyType_e.Vector:
									property.m_vecVal = EditorGUILayout.Vector4Field(property.m_PrettyName, property.m_vecVal);
									break;
								case PostFX_Generic.propertyType_e.Float:
									property.m_fVal = EditorGUILayout.FloatField(property.m_PrettyName, property.m_fVal);
									break;
								case PostFX_Generic.propertyType_e.Range:
									property.m_fVal = EditorGUILayout.Slider(property.m_PrettyName, property.m_fVal, property.m_fRangeMin, property.m_fRangeMax);
									break;
								case PostFX_Generic.propertyType_e.TexEnv:
								default:
									property.m_texVal = (Texture)EditorGUILayout.ObjectField(property.m_PrettyName, property.m_texVal, typeof(Texture), false);
									break;
							}
						}
						GUI.enabled = true;
						EditorGUILayout.EndHorizontal();
					}
				}
			}
		}
	}
}