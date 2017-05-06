//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Attribute to set custom names in the inspector
// Namespace: None (decided not to namespace an attribute for usability) 
//
//===============================================================================//

using System;
using UnityEngine;

/// <summary>
/// Set custom names for the inspector
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Field)]
public class NameAttribute : PropertyAttribute {
	public NameAttribute(string strName) {
		m_strName = strName;
	}

	string m_strName;

	/// <summary>
	/// Sets the custom name
	/// </summary>
	public string Name {
		get {
			return m_strName;
		}
		set {
			m_strName = value;
		}
	}
}