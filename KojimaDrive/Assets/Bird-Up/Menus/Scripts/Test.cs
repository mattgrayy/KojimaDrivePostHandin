using UnityEngine;
using System.Collections;

namespace Bird {
	public class Test : MonoBehaviour {

		// Use this for initialization
		void Start() {
			ObjectDB.DontDestroyOnLoad_Managed(this);
		}

		private void Update() {
			Debug.LogError("BUTT");
		}
	}
}