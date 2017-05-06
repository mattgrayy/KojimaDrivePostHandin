//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Generic transition object that modifies PostFX_Generic objects
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bird {
	public class Transition_Generic : BaseTransition {
#if UNITY_EDITOR
		private void OnDrawGizmos() {
			if (TransitionActive) {
				Gizmos.DrawIcon(transform.position, "Transition_On.png", true);
			} else {
				Gizmos.DrawIcon(transform.position, "Transition_Off.png", true);
			}
		}
#endif

		[Name("Target PostFX Stack")]
		public string m_strTargetPostFXStack;
		public float m_fTransitionTime = 1.0f;
		public float m_fTransitionProgress = 0.0f;

		[System.Serializable]
		public class targetPostFX_s {
			public string m_strPostFXStackMember = "";
			public PostFXObject m_PostFXStackMember = null;
			public List<PostFX_Generic.shaderProperty_t> m_StartState;
			public int m_nStartPasses;
			public List<PostFX_Generic.shaderProperty_t> m_EndState;
			public int m_nEndPasses = 0;
			public bool m_bAlterPasses = false;
			public bool m_bLerpPasses = false;
			public bool m_bCurvePasses = false;
			public AnimationCurve m_PassesCurve = null;

			public bool m_bEnablePFXO = true;
			public bool m_bAlterPFXO = true;
#if UNITY_EDITOR
			[System.NonSerialized]
			public bool m_bFoldoutOpen = true;
			[System.NonSerialized]
			public float m_fPassesCurvePreview = 0.0f;
			public string m_strLastValidName = "";
#endif
		}

		public List<targetPostFX_s> m_TargetPostFXObjects;
		public float m_fLerpState = 0.0f;
		public bool m_bTransitionOnStart = false;
		public bool m_bDestroySelfOnCompletion = false;
		public bool m_bDestroyGOOnCompletion = false;
		public bool m_bRevertToInitialValuesOnCompletion = false;
		public void Start() {
			if (m_bTransitionOnStart) {
				StartTransition();
			}
		}

		public override void InterruptTransition() {
			base.InterruptTransition();
		}

		public override void StopTransition(bool bInterrupt = false) {
			base.StopTransition(bInterrupt);

			if (m_bRevertToInitialValuesOnCompletion) {
				for (int i = 0; i < m_TargetPostFXObjects.Count; i++) {
					for (int j = 0; j < m_TargetPostFXObjects[i].m_StartState.Count; j++) {
						((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps[j].Set(m_TargetPostFXObjects[i].m_StartState[j]);
					}

					((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_nPasses = m_TargetPostFXObjects[i].m_nStartPasses;
					((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_bEnabled = m_TargetPostFXObjects[i].m_bEnablePFXO;
				}
			}

			if (m_bDestroySelfOnCompletion) {
				if (m_bDestroyGOOnCompletion) {
					Destroy(gameObject);
				} else {
					Destroy(this);
				}
			}
		}

		public override void StartTransitionReverse() {
			m_bPlayInReverse = true;
			StartTransition();
		}

		public override void StartTransition() {
			m_fLerpState = 0.0f;
			m_fTransitionProgress = 0.0f;

			PostFXStack pfxStack = PostFXStack.GetPostFXStack(m_strTargetPostFXStack);
			if (!pfxStack) {
				Debug.LogError("Transition_Generic::StartTransition - Failed to find PostFX Stack with name '" + m_strTargetPostFXStack + "'");
				return;
			}

			// Save off our current state so we have a thing to lerp from
			for (int i = 0; i < m_TargetPostFXObjects.Count; i++) {
				PostFX_Generic pfxobj = (PostFX_Generic)pfxStack.GetPostFX(m_TargetPostFXObjects[i].m_strPostFXStackMember);
				if (!pfxobj) {
					Debug.LogError("Transition_Generic::StartTransition - PostFX Stack object '" + m_TargetPostFXObjects[i].m_strPostFXStackMember + "' is not a Generic controller!");
					continue;
				}
				m_TargetPostFXObjects[i].m_PostFXStackMember = pfxobj;
				m_TargetPostFXObjects[i].m_StartState = new List<PostFX_Generic.shaderProperty_t>();
				m_TargetPostFXObjects[i].m_nStartPasses = pfxobj.m_nPasses;
				for (int j = 0; j < ((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps.Count; j++) {
					m_TargetPostFXObjects[i].m_StartState.Add(new PostFX_Generic.shaderProperty_t(((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps[j]));
					if (m_TargetPostFXObjects[i].m_EndState[j].m_bChangeInTransition && (!m_TargetPostFXObjects[i].m_EndState[j].m_bLerpInTransition && !m_TargetPostFXObjects[i].m_EndState[j].m_bUseAnimationCurves)) {
						((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps[j].Set(m_TargetPostFXObjects[i].m_EndState[j]);
					}


					if (m_TargetPostFXObjects[i].m_EndState[j].m_bChangeInTransition && m_TargetPostFXObjects[i].m_EndState[j].m_bSetFirstFrameToInitialState) {
						switch (m_TargetPostFXObjects[i].m_EndState[j].m_Type) {
							case PostFX_Generic.propertyType_e.Color:
								for (int c = 0; c < m_TargetPostFXObjects[i].m_EndState[j].m_Curves.Length; c++) {
									if (m_TargetPostFXObjects[i].m_EndState[j].m_Curves[c] != null) {
										m_TargetPostFXObjects[i].m_EndState[j].m_Curves[c].AddKey(0.0f, m_TargetPostFXObjects[i].m_StartState[j].m_colVal[c]);
									}
								}
								break;
							case PostFX_Generic.propertyType_e.Vector:
								for (int c = 0; c < m_TargetPostFXObjects[i].m_EndState[j].m_Curves.Length; c++) {
									if (m_TargetPostFXObjects[i].m_EndState[j].m_Curves[c] != null) {
										m_TargetPostFXObjects[i].m_EndState[j].m_Curves[c].AddKey(0.0f, m_TargetPostFXObjects[i].m_StartState[j].m_vecVal[c]);
									}
								}
								break;
							case PostFX_Generic.propertyType_e.Float:
							case PostFX_Generic.propertyType_e.Range:
								m_TargetPostFXObjects[i].m_EndState[j].m_Curves[0].AddKey(0.0f, m_TargetPostFXObjects[i].m_StartState[j].m_fVal);
								break;
							case PostFX_Generic.propertyType_e.TexEnv:
							default:
								// Curves not supported
								break;

						}			
					}
				}

				if (m_TargetPostFXObjects[i].m_bAlterPasses && !m_TargetPostFXObjects[i].m_bLerpPasses) {
					((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_nPasses = m_TargetPostFXObjects[i].m_nEndPasses;
				}

				if (m_TargetPostFXObjects[i].m_bAlterPFXO) {
					((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_bEnabled = m_TargetPostFXObjects[i].m_bEnablePFXO;
				}
			}

			base.StartTransition();
		}

		public bool m_bPlayInReverse = false;
		public bool m_bUseUnscaledTime = false;
		public override void UpdateTransition() {
			if(m_bUseUnscaledTime) {
				m_fTransitionProgress += Time.unscaledDeltaTime;
			} else {
				m_fTransitionProgress += Time.deltaTime; // I normally wouldn't do timer by counter like this, but this gives some flexibility here.
			}
			
			if (m_bPlayInReverse) {
				m_fLerpState = 1.0f-(m_fTransitionProgress / m_fTransitionTime);
			} else {
				m_fLerpState = m_fTransitionProgress / m_fTransitionTime;
			}

			for (int i = 0; i < m_TargetPostFXObjects.Count; i++) {
				if (((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember) == null) {
					Debug.LogError("Transition_Generic::UpdateTransition - PostFX Stack object '" + m_TargetPostFXObjects[i].m_strPostFXStackMember + "' is not a Generic controller, cannot lerp!");
					continue;
				}

				// Lerp our passes value
				if (m_TargetPostFXObjects[i].m_bAlterPasses) {
					if (m_TargetPostFXObjects[i].m_bLerpPasses) {
						((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_nPasses = Mathf.RoundToInt(Mathf.Lerp(m_TargetPostFXObjects[i].m_nStartPasses,
							m_TargetPostFXObjects[i].m_nEndPasses, m_fLerpState));
					}

					if (m_TargetPostFXObjects[i].m_bCurvePasses) {
						((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_nPasses = Mathf.RoundToInt(m_TargetPostFXObjects[i].m_PassesCurve.Evaluate(m_fLerpState));
					}
				}

				for (int j = 0; j < m_TargetPostFXObjects[i].m_StartState.Count; j++) {
					if (!m_TargetPostFXObjects[i].m_EndState[j].m_bChangeInTransition) {
						continue;
					}

					if (m_TargetPostFXObjects[i].m_EndState[j].m_bUseAnimationCurves) {
						switch (m_TargetPostFXObjects[i].m_StartState[j].m_Type) {
							case PostFX_Generic.propertyType_e.Color:
								if (m_TargetPostFXObjects[i].m_EndState[j].m_bAddToInitialState) {
									for (int c = 0; c < 4; c++) {
										((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps[j].m_colVal[c] =
											m_TargetPostFXObjects[i].m_EndState[j].m_Curves[c].Evaluate(m_fLerpState) + m_TargetPostFXObjects[i].m_StartState[j].m_colVal[c];
									}
								} else {
									for (int c = 0; c < 4; c++) {
										((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps[j].m_colVal[c] =
											m_TargetPostFXObjects[i].m_EndState[j].m_Curves[c].Evaluate(m_fLerpState);
									}
								}
								break;
							case PostFX_Generic.propertyType_e.Vector:
								if (m_TargetPostFXObjects[i].m_EndState[j].m_bAddToInitialState) {
									for (int c = 0; c < 4; c++) {
										((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps[j].m_vecVal[c] =
											m_TargetPostFXObjects[i].m_EndState[j].m_Curves[c].Evaluate(m_fLerpState) + m_TargetPostFXObjects[i].m_StartState[j].m_vecVal[c];
									}
								} else {
									for (int c = 0; c < 4; c++) {
										((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps[j].m_vecVal[c] =
											m_TargetPostFXObjects[i].m_EndState[j].m_Curves[c].Evaluate(m_fLerpState);
									}
								}
								break;
							case PostFX_Generic.propertyType_e.Float:
							case PostFX_Generic.propertyType_e.Range:
								if (m_TargetPostFXObjects[i].m_EndState[j].m_bAddToInitialState) {
									((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps[j].m_fVal =
										m_TargetPostFXObjects[i].m_EndState[j].m_Curves[0].Evaluate(m_fLerpState) + m_TargetPostFXObjects[i].m_StartState[j].m_fVal;
								} else {
									((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps[j].m_fVal =
										m_TargetPostFXObjects[i].m_EndState[j].m_Curves[0].Evaluate(m_fLerpState);
								}
								break;
							case PostFX_Generic.propertyType_e.TexEnv:
								// Just set textures outright
								Debug.LogError("Attempted to animate TexEnv!");
								//((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps[j].m_texVal = m_TargetPostFXObjects[j].m_EndState[j].m_texVal;
								break;
							default:
								break;
						}
						continue;
					}

					if (!m_TargetPostFXObjects[i].m_EndState[j].m_bLerpInTransition) {
						continue;
					}

					switch (m_TargetPostFXObjects[i].m_StartState[j].m_Type) {
						case PostFX_Generic.propertyType_e.Color:
							((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps[j].m_colVal = Color.Lerp(
								m_TargetPostFXObjects[i].m_StartState[j].m_colVal,
								m_TargetPostFXObjects[i].m_EndState[j].m_colVal,
								m_fLerpState);
							break;
						case PostFX_Generic.propertyType_e.Vector:
							((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps[j].m_vecVal = Vector4.Lerp(
								m_TargetPostFXObjects[i].m_StartState[j].m_vecVal,
								m_TargetPostFXObjects[i].m_EndState[j].m_vecVal,
								m_fLerpState);
							break;
						case PostFX_Generic.propertyType_e.Float:
							((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps[j].m_fVal = Mathf.Lerp(
								m_TargetPostFXObjects[i].m_StartState[j].m_fVal,
								m_TargetPostFXObjects[i].m_EndState[j].m_fVal,
								m_fLerpState);
							break;
						case PostFX_Generic.propertyType_e.Range:
							((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps[j].m_fVal = Mathf.Lerp(
								m_TargetPostFXObjects[i].m_StartState[j].m_fVal,
								m_TargetPostFXObjects[i].m_EndState[j].m_fVal,
								m_fLerpState);
							break;
						case PostFX_Generic.propertyType_e.TexEnv:
							// Just set textures outright
							((PostFX_Generic)m_TargetPostFXObjects[i].m_PostFXStackMember).m_ShaderProps[j].m_texVal = m_TargetPostFXObjects[j].m_EndState[j].m_texVal;
							break;
						default:
							break;
					}
				}
			}

			if (m_bPlayInReverse) {
				if (m_fLerpState <= 0.0f) {
					StopTransition();
					m_fLerpState = 0.0f;
					m_fTransitionProgress = m_fTransitionTime;
					m_bPlayInReverse = false;
				}
			} else {
				if (m_fLerpState >= 1.0f) {
					StopTransition();
					m_fLerpState = 1.0f;
					m_fTransitionProgress = m_fTransitionTime;
					m_bPlayInReverse = false;
				}
			}
		}
	}
}