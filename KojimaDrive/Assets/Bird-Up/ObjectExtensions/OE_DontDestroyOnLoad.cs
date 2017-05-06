using UnityEngine;
using System.Collections.Generic;

public static class ObjectDB {
	private static List<Object> s_DontDestroyOnLoadObjects = new List<Object>();
	public static void DontDestroyOnLoad_Managed(this Object obj) {
		s_DontDestroyOnLoadObjects.Add(obj);
		UnityEngine.Object.DontDestroyOnLoad(obj);
	}
	public static void Destroy(this Object obj) {
		s_DontDestroyOnLoadObjects.Remove(obj);
		UnityEngine.Object.Destroy(obj);
	}
	public static List<Object> GetDontDestroyOnLoadObjects() {
		return s_DontDestroyOnLoadObjects;
	}
}