//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Generic PostFX stack object for those PostFX shaders that don't need
//			special treatment to render out.
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;


namespace Bird {
	[ExecuteInEditMode]
	public class PostFX_Generic : PostFXObject {
		// Copy of the one from UnityEditor.ShaderUtil.ShaderPropertyType
		public enum propertyType_e {
			Color = 0,
			Vector = 1,
			Float = 2,
			Range = 3,
			TexEnv = 4
		}

		// Naming convention states that this class is named wrong, but C# structs are passed as value-based objects,
		//		  which causes massive problems for this system. It feels wrong to use _s, so I'll use the old C style _t here. -sam 20/03/2017
		[System.Serializable]
		public class shaderProperty_t {
			public shaderProperty_t() {
				// do nothing
			}
			public shaderProperty_t(shaderProperty_t old) {
				m_Name = old.m_Name;
				m_PrettyName = old.m_PrettyName;
				m_Type = old.m_Type;

				switch (m_Type) {
					case propertyType_e.Color:
						m_colVal = old.m_colVal;
						m_colDefaultVal = old.m_colDefaultVal;
						break;
					case propertyType_e.Vector:
						m_vecVal = old.m_vecVal;
						m_vecDefaultVal = old.m_vecDefaultVal;
						break;
					case propertyType_e.Float:
						m_fVal = old.m_fVal;
						m_fDefaultVal = old.m_fDefaultVal;
						break;
					case propertyType_e.Range:
						m_fVal = old.m_fVal;
						m_fDefaultVal = old.m_fDefaultVal;
						m_fRangeMax = old.m_fRangeMax;
						m_fRangeMin = old.m_fRangeMin;
						break;
					case propertyType_e.TexEnv:
						m_texVal = old.m_texVal;
						m_texDefaultVal = old.m_texDefaultVal;
						break;
					default:
						break;

				}

			}

			public void Set(shaderProperty_t old, bool bSetType = false) {
				m_Name = old.m_Name;
				m_PrettyName = old.m_PrettyName;
				if (bSetType) {
					m_Type = old.m_Type;
				}

				switch (m_Type) {
					case propertyType_e.Color:
						m_colVal = old.m_colVal;
						m_colDefaultVal = old.m_colDefaultVal;
						break;
					case propertyType_e.Vector:
						m_vecVal = old.m_vecVal;
						m_vecDefaultVal = old.m_vecDefaultVal;
						break;
					case propertyType_e.Float:
						m_fVal = old.m_fVal;
						m_fDefaultVal = old.m_fDefaultVal;
						break;
					case propertyType_e.Range:
						m_fVal = old.m_fVal;
						m_fDefaultVal = old.m_fDefaultVal;
						m_fRangeMax = old.m_fRangeMax;
						m_fRangeMin = old.m_fRangeMin;
						break;
					case propertyType_e.TexEnv:
						m_texVal = old.m_texVal;
						m_texDefaultVal = old.m_texDefaultVal;
						break;
					default:
						break;

				}

				for (int i = 0; i < m_Curves.Length; i++) {
					m_Curves[i] = old.m_Curves[i];
				}
			}

			public string m_Name;
			public string m_PrettyName;
			public propertyType_e m_Type;
			public float m_fVal;
			public float m_fRangeMin;
			public float m_fRangeMax;
			public Color m_colVal;
			public Vector4 m_vecVal;
			public Texture m_texVal;

			public float m_fDefaultVal;
			public Color m_colDefaultVal;
			public Vector4 m_vecDefaultVal;
			public Texture m_texDefaultVal;

			public AnimationCurve[] m_Curves = new AnimationCurve[4] { null, null, null, null };
			public bool m_bUseAnimationCurves = false;
			public bool m_bSetFirstFrameToInitialState = false;
			public bool m_bAddToInitialState = false;

			public bool m_bChangeInTransition = false;
			public bool m_bLerpInTransition = false;

			public void ResetToDefaults() {
				switch (m_Type) {
					case propertyType_e.Color:
						m_colVal = m_colDefaultVal;
						break;
					case propertyType_e.Vector:
						m_vecVal = m_vecDefaultVal;
						break;
					case propertyType_e.Float:
						m_fVal = m_fDefaultVal;
						break;
					case propertyType_e.Range:
						m_fVal = m_fDefaultVal;
						break;
					case propertyType_e.TexEnv:
						m_texVal = m_texDefaultVal;
						break;
					default:
						break;

				}

				for (int i = 0; i < m_Curves.Length; i++) {
					m_Curves[i] = null;
				}
			}

#if UNITY_EDITOR
			// Preview vars used for curve editing
			[System.NonSerialized]
			public float m_fPreviewPosition = 0.0f;
#endif
		}

		public System.Collections.Generic.List<shaderProperty_t> m_ShaderProps;
		public int m_nPasses = 1;

		public shaderProperty_t GetProperty(string strName) {
			for (int i = 0; i < m_ShaderProps.Count; i++) {
				if (m_ShaderProps[i].m_Name == strName) {
					return m_ShaderProps[i];
				}
			}

			return null;
		}

		public override void SetupMaterial() {
			for(int i = 0; i < m_ShaderProps.Count; i++) {
				switch(m_ShaderProps[i].m_Type) {
					case propertyType_e.Color:
						m_Material.SetColor(m_ShaderProps[i].m_Name, m_ShaderProps[i].m_colVal);
						break;
					case propertyType_e.Vector:
						m_Material.SetVector(m_ShaderProps[i].m_Name, m_ShaderProps[i].m_vecVal);
						break;
					case propertyType_e.Float:
						m_Material.SetFloat(m_ShaderProps[i].m_Name, m_ShaderProps[i].m_fVal);
						break;
					case propertyType_e.Range:
						m_Material.SetFloat(m_ShaderProps[i].m_Name, m_ShaderProps[i].m_fVal);
						break;
					case propertyType_e.TexEnv:
					default:
						m_Material.SetTexture(m_ShaderProps[i].m_Name, m_ShaderProps[i].m_texVal);
						break;
				}
			}
		}

		public override void RenderEffect(RenderTexture source, RenderTexture destination) {
			SetupMaterial();
			if (m_nPasses == 0) {
				Graphics.Blit(source, destination);
				return;
			} else if (m_nPasses == 1) {
				Graphics.Blit(source, destination, m_Material);
				return;
			} else {
				int width = source.width;
				int height = source.height;

				RenderTexture rt = RenderTexture.GetTemporary(width, height);
				RenderTexture rt2 = RenderTexture.GetTemporary(width, height);
				Graphics.Blit(source, rt);

				for (int i = 0; i < m_nPasses; i++) {
					Graphics.Blit(rt, rt2, m_Material);
					RenderTexture swap = rt;
					rt = rt2;
					rt2 = swap;
				}

				Graphics.Blit(rt, destination);
				RenderTexture.ReleaseTemporary(rt);
				RenderTexture.ReleaseTemporary(rt2);
			}
		}

		public void ResetMaterial() {
			for (int i = 0; i < m_ShaderProps.Count; i++) {
				switch (m_ShaderProps[i].m_Type) {
					case propertyType_e.Color:
						m_ShaderProps[i].m_colVal = m_ShaderProps[i].m_colDefaultVal;
						m_Material.SetColor(m_ShaderProps[i].m_Name, m_ShaderProps[i].m_colDefaultVal);
						break;
					case propertyType_e.Vector:
						m_ShaderProps[i].m_vecVal = m_ShaderProps[i].m_vecDefaultVal;
						m_Material.SetVector(m_ShaderProps[i].m_Name, m_ShaderProps[i].m_vecDefaultVal);
						break;
					case propertyType_e.Float:
						m_ShaderProps[i].m_fVal = m_ShaderProps[i].m_fDefaultVal;
						m_Material.SetFloat(m_ShaderProps[i].m_Name, m_ShaderProps[i].m_fDefaultVal);
						break;
					case propertyType_e.Range:
						m_ShaderProps[i].m_fVal = m_ShaderProps[i].m_fDefaultVal;
						m_Material.SetFloat(m_ShaderProps[i].m_Name, m_ShaderProps[i].m_fDefaultVal);
						break;
					case propertyType_e.TexEnv:
					default:
						m_ShaderProps[i].m_texVal = m_ShaderProps[i].m_texDefaultVal;
						m_Material.SetTexture(m_ShaderProps[i].m_Name, m_ShaderProps[i].m_texDefaultVal);
						break;
				}
			}
		}
	}
}