//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: PostFX Stack, allowing ordered rendering of Post-Render Effects
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bird {
	[ExecuteInEditMode]
	public class PostFXStack : MonoBehaviour {
#if UNITY_EDITOR
		private void OnDrawGizmos() {
			Gizmos.DrawIcon(transform.position, "PostFXStack.png", true);
		}
#endif

		public bool m_bDontDestroyOnLoad = false;
		public bool m_bOverwriteClashedInstance = false;
		static public List<PostFXStack> s_PostFXStacks = new List<PostFXStack>();
		static private void AddToGlobalPostFXStackRegister(PostFXStack stack) {
			// Make sure we have a unique name...
			PostFXStack clashed = GetPostFXStack(stack.m_strGlobalName);
			if (clashed != null) {
				if (clashed == stack) {
					// This is fine - we shouldn't throw an error if we match with ourselves.
					return;
				}
				Debug.LogError("PostFXStack: Static name clash in stack '" + stack.m_strGlobalName + "'! Please use a different name if overwriting the previous stack is not intended!");
				if (stack.m_bOverwriteClashedInstance) {
					RemoveFromGlobalPostFXStackRegister(clashed);
					DestroyImmediate(clashed.gameObject);
				} else {
					DestroyImmediate(stack.gameObject);
					return;
				}
			}

			s_PostFXStacks.Add(stack);
		}

		static private void RemoveFromGlobalPostFXStackRegister(PostFXStack stack) {
			s_PostFXStacks.Remove(stack);
		}

		static public PostFXStack GetPostFXStack(string strName) {
#if UNITY_EDITOR
			if (!Application.isPlaying) {
				PostFXStack[] pfxs = GameObject.FindObjectsOfType<PostFXStack>();
				for (int i = 0; i < pfxs.Length; i++) {
					if (pfxs[i] == null) {
						continue;
					}
					if (pfxs[i].m_strGlobalName == strName) {
						return pfxs[i];
					}
				}

				return null;
			}
#endif
			for(int i = 0; i < s_PostFXStacks.Count; i++) {
				if(s_PostFXStacks[i].m_strGlobalName == strName) {
					return s_PostFXStacks[i];
				}
			}

			return null;

		}

		private void OnDestroy() {
			RemoveFromGlobalPostFXStackRegister(this);
		}

		public bool m_bSetupOnAwake = false;
		private void Awake() {
			if (m_bSetupOnAwake) {
				AddToGlobalPostFXStackRegister(this);

				if (m_bDontDestroyOnLoad && Application.isPlaying) {
					ObjectDB.DontDestroyOnLoad_Managed(this);
				}
			}
		}

		private void Start() {
			if (!m_bSetupOnAwake) {
				AddToGlobalPostFXStackRegister(this);

				if (m_bDontDestroyOnLoad && Application.isPlaying) {
					ObjectDB.DontDestroyOnLoad_Managed(this);
				}
			}
		}

		public string m_strGlobalName;
		public string GlobalName {
			get {
				return m_strGlobalName;
			}
			set {
				m_strGlobalName = value;
			}
		}


		public List<PostFXObject> m_PostFXStack;
		public void AddPostFX(PostFXObject stackEntry) {
			if(stackEntry.m_Owner != null) {
				stackEntry.m_Owner.RemovePostFX(stackEntry);
			}

			stackEntry.m_Owner = this;
			m_PostFXStack.Add(stackEntry);
		}

		public PostFXObject GetPostFX(string postFXName) {
			for(int i = 0; i < m_PostFXStack.Count; i++) {
				if(m_PostFXStack[i].m_UniqueName == postFXName) {
					return m_PostFXStack[i];
				}
			}

			return null;
		}

		public void RemovePostFX(PostFXObject stackEntry) {
			m_PostFXStack.Remove(stackEntry);
			stackEntry.m_Owner = null;
		}

		public List<PostFXObject> GetPostFXList() {
			return m_PostFXStack;
		}

		public void OnRenderImage(RenderTexture source, RenderTexture destination) {
			int width = source.width;
			int height = source.height;

			RenderTexture rt = RenderTexture.GetTemporary(width, height);
			RenderTexture rt2 = RenderTexture.GetTemporary(width, height);
			Graphics.Blit(source, rt);

			for (int i = 0; i < m_PostFXStack.Count; i++) {
				if(m_PostFXStack[i] == null || !m_PostFXStack[i].m_bEnabled || m_PostFXStack[i].m_Material == null || m_PostFXStack[i].m_Shader == null) {
					continue;
				}

				m_PostFXStack[i].RenderEffect(rt, rt2);
				RenderTexture swap = rt;
				rt = rt2;
				rt2 = swap;
			}

			Graphics.Blit(rt, destination);
			RenderTexture.ReleaseTemporary(rt);
			RenderTexture.ReleaseTemporary(rt2);
		}
	}
}