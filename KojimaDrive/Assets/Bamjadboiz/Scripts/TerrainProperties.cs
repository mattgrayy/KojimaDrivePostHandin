using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bam
{
	public class TerrainProperties : MonoBehaviour
	{
		public List<TerrainMaterial_s> m_terrainMaterialList;
		public static TerrainProperties s_singleton;

		// Use this for initialization
		void Awake()
		{
			Debug.Assert(s_singleton == null, "Only one terrain properties allowed per scene");
			s_singleton = this;
		}

		public static bool GetName(Material m, ref string name)
		{
			foreach (TerrainMaterial_s terrainMat in s_singleton.m_terrainMaterialList)
			{
				foreach (Material mat in terrainMat.m_materials)
				{
					if (m == mat)
					{
						name = terrainMat.m_friendlyName;
						return true;
					}
				}
			}
			return false;
		}


		[Serializable]
		public struct TerrainMaterial_s
		{
			public string m_friendlyName;
			public Material[] m_materials;
			//TO-DO: Implement default values!
			//public Modifiers_s m_defaultValues;
		}
		[Serializable]
		public struct Modifiers_s
		{
			public float m_acceleration, m_turnMaxSpeed, m_extraGrip, m_maxSpeed;
			public void ApplyPropertiesToCarInfo(ref Kojima.CarScript.CarInfo_s carInfo)
			{
				carInfo.m_acceleration = m_acceleration;
				carInfo.m_turnMaxSpeed = m_turnMaxSpeed;
				carInfo.m_extraGrip = m_extraGrip;
				carInfo.m_maxSpeed = m_maxSpeed;
			}
		}

		[Serializable]
		public struct Properties_s 
		{
			public string m_friendlyName;
			public Modifiers_s m_modifiers;

			public static bool operator ==(Properties_s a, Properties_s b)
			{
				return a.m_friendlyName == b.m_friendlyName;
			}
			public static bool operator !=(Properties_s a, Properties_s b)
			{
				return !(a == b);
			}
		}
	}
}