//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Editor script for the PostFX Generic stack object
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text.RegularExpressions;

namespace Bird {
	[CustomEditor(typeof(PostFX_Generic))]
	public class PostFX_Generic_Editor : Editor {
		public static string SplitCamelCase( string str ) {
			return Regex.Replace( 
				Regex.Replace( 
				str, 
				@"(\P{Ll})(\P{Ll}\p{Ll})", 
				"$1 $2" 
			), 
			@"(\p{Ll})(\P{Ll})", 
			"$1 $2" );
		}

		PostFX_Generic.shaderProperty_t FindNamedProperty(System.Collections.Generic.List<PostFX_Generic.shaderProperty_t> propList, string strName) {
			for(int i = 0; i < propList.Count; i++) {
				if(propList[i].m_Name == strName) {
					return propList[i];
				}
			}

			return null;
		}
		
		public void ComputeProperties(Shader shader, PostFX_Generic pfxTarget, bool bKeepValues = false) {
			Debug.Log("PostFX_Generic_Editor (" + pfxTarget.gameObject.name + "): Rebuilding property list...");

			pfxTarget.CreateMaterial();
			if (shader == null) {
				return;
			}
			System.Collections.Generic.List<PostFX_Generic.shaderProperty_t> currentList = new System.Collections.Generic.List<PostFX_Generic.shaderProperty_t>();
			//pfxTarget.m_ShaderProps = currentList;
			int nPropCount = ShaderUtil.GetPropertyCount(shader);
			for (int j = 0; j < nPropCount; j++) {
				if (ShaderUtil.GetPropertyName(shader, j) == "_MainTex") {
					continue; // Skip maintex since this is a camera effect
				}
				PostFX_Generic.shaderProperty_t newthing = new PostFX_Generic.shaderProperty_t();
				newthing.m_Name = ShaderUtil.GetPropertyName(shader, j);
				newthing.m_PrettyName = Regex.Replace(newthing.m_Name, @"^([m_])", "");
				newthing.m_PrettyName = Regex.Replace(newthing.m_PrettyName, @"^([_])", "");
				newthing.m_PrettyName = SplitCamelCase(newthing.m_PrettyName); 
				newthing.m_Type = (PostFX_Generic.propertyType_e)ShaderUtil.GetPropertyType(shader, j);

				switch (newthing.m_Type) {
					case PostFX_Generic.propertyType_e.Color:
						if (bKeepValues) {
							newthing.m_colDefaultVal = pfxTarget.m_Material.GetColor(newthing.m_Name);

							newthing.m_colVal = FindNamedProperty(pfxTarget.m_ShaderProps, newthing.m_Name).m_colVal;
							pfxTarget.m_Material.SetColor(newthing.m_Name, newthing.m_colVal);
						} else {
							newthing.m_colDefaultVal = newthing.m_colVal = pfxTarget.m_Material.GetColor(newthing.m_Name);
						}
						break;
					case PostFX_Generic.propertyType_e.Vector:
						if (bKeepValues) {
							newthing.m_vecDefaultVal = pfxTarget.m_Material.GetVector(newthing.m_Name);

							newthing.m_vecVal = FindNamedProperty(pfxTarget.m_ShaderProps, newthing.m_Name).m_vecVal;
							pfxTarget.m_Material.SetVector(newthing.m_Name, newthing.m_vecVal);
						} else {
							newthing.m_vecDefaultVal = newthing.m_vecVal = pfxTarget.m_Material.GetVector(newthing.m_Name);
						}
						break;
					case PostFX_Generic.propertyType_e.Range:
						if (bKeepValues) {
							newthing.m_fDefaultVal = pfxTarget.m_Material.GetFloat(newthing.m_Name);
							newthing.m_fRangeMin = ShaderUtil.GetRangeLimits(shader, j, 1);
							newthing.m_fRangeMax = ShaderUtil.GetRangeLimits(shader, j, 2);

							newthing.m_fVal = FindNamedProperty(pfxTarget.m_ShaderProps, newthing.m_Name).m_fVal;
							pfxTarget.m_Material.SetFloat(newthing.m_Name, newthing.m_fVal);
						} else {
							newthing.m_fRangeMin = ShaderUtil.GetRangeLimits(shader, j, 1);
							newthing.m_fRangeMax = ShaderUtil.GetRangeLimits(shader, j, 2);
							newthing.m_fDefaultVal = newthing.m_fVal = pfxTarget.m_Material.GetFloat(newthing.m_Name);
						}
						break;
					case PostFX_Generic.propertyType_e.Float:
					default:
						if (bKeepValues) {
							newthing.m_fDefaultVal = pfxTarget.m_Material.GetFloat(newthing.m_Name);

							newthing.m_fVal = FindNamedProperty(pfxTarget.m_ShaderProps, newthing.m_Name).m_fVal;
							pfxTarget.m_Material.SetFloat(newthing.m_Name, newthing.m_fVal);
						} else {
							newthing.m_fDefaultVal = newthing.m_fVal = pfxTarget.m_Material.GetFloat(newthing.m_Name);
						}
						break;
					case PostFX_Generic.propertyType_e.TexEnv:
						if (bKeepValues) {
							newthing.m_texDefaultVal = pfxTarget.m_Material.GetTexture(newthing.m_Name);

							newthing.m_texVal = FindNamedProperty(pfxTarget.m_ShaderProps, newthing.m_Name).m_texVal;
							pfxTarget.m_Material.SetTexture(newthing.m_Name, newthing.m_texVal);
						} else {
							newthing.m_texDefaultVal = newthing.m_texVal = pfxTarget.m_Material.GetTexture(newthing.m_Name);
						}
						break;
				}

				currentList.Add(newthing);
			}

			pfxTarget.m_ShaderProps = currentList;
			Debug.Log("PostFX_Generic_Editor (" + pfxTarget.gameObject.name + "): Property list successfully rebuilt!");
		}

		public override void OnInspectorGUI() {
			PostFX_Generic pfxTarget = (PostFX_Generic)target;
			Shader shader = pfxTarget.m_Shader;

			GUI.enabled = false;
			EditorGUILayout.ObjectField("Property of:", pfxTarget.m_Owner, typeof(PostFXStack), true);
			GUI.enabled = true;

			pfxTarget.m_UniqueName = EditorGUILayout.TextField("Unique Name", pfxTarget.m_UniqueName);
			pfxTarget.m_bEnabled = EditorGUILayout.Toggle("Enabled", pfxTarget.m_bEnabled);

			System.Collections.Generic.List<PostFX_Generic.shaderProperty_t> currentList = pfxTarget.m_ShaderProps;
			Shader newShader = (Shader)EditorGUILayout.ObjectField("Shader", pfxTarget.m_Shader, typeof(Shader), false);
			if (newShader != shader) {
				// We have a new shader. Build our property list.
				shader = newShader;
				pfxTarget.m_Shader = shader;
				ComputeProperties(shader, pfxTarget);
			}

			if (shader == null || currentList == null) {
				EditorGUILayout.HelpBox("Error! No shader detected!", MessageType.Error);
				return;
			}

			pfxTarget.m_nPasses = Mathf.Clamp(EditorGUILayout.IntField("Iterations", pfxTarget.m_nPasses), 0, 100);
			EditorGUILayout.LabelField("Shader Properties", EditorStyles.boldLabel);

			for (int j = 0; j < currentList.Count; j++) {
				PostFX_Generic.shaderProperty_t property = currentList[j];
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

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Rebuild Properties")) {
				ComputeProperties(pfxTarget.m_Shader, pfxTarget, true);
			}

			if (GUILayout.Button("Reset Properties")) {
				pfxTarget.ResetMaterial();
			}
			EditorGUILayout.EndHorizontal();

			/*if (GUILayout.Button("Force Update Material")) {
				pfxTarget.SetupMaterial();
			}*/

			pfxTarget.SetupMaterial();
		}
	}

}
