//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: PostFX Stack entry (abstract class)
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public abstract class PostFXObject : MonoBehaviour {
		[Tooltip("Unique name to allow for global access of this PostFX object")]
		public string m_UniqueName;
		public bool m_bEnabled = true;
		public PostFXStack m_Owner = null;
		void Awake() {
			CreateMaterial();
			SetupMaterial();
		}

		public void CreateMaterial() { 
			m_Material = new Material(m_Shader);
		}

		public abstract void SetupMaterial();
		public abstract void RenderEffect(RenderTexture src, RenderTexture dst);

		[NotNull]
		public Shader m_Shader;
		//[HideInInspector]
		public Material m_Material;

		private void OnDestroy() {
			if (m_Owner) {
				m_Owner.RemovePostFX(this);
			}
		}

		public void AddToStack(PostFXStack stack) {
			stack.AddPostFX(this);
		}

		public void RemoveFromParentStack() {
			if (m_Owner) {
				m_Owner.RemovePostFX(this);
			}
		}
	}
}