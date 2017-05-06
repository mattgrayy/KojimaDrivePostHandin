//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Transition listener that loads a given scene.
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Bird {
	public class UI_TL_LoadScenes : UI_Transition_Listener {
		public sceneLoader_t[] m_ScenesToLoad;

		public enum loadSceneMode_e {
			SINGLE = 0,
			ADDITIVE = 1, 
			UNLOAD = 2
		}

		[System.Serializable]
		public class sceneLoader_t {
			public string name;
			public loadSceneMode_e m_loadSceneMode;
		}


		public override void TransitionCompleted() {
			ProcessSceneList(m_ScenesToLoad);
		}

		public static void ProcessSceneList(sceneLoader_t[] sceneList) {
			for (int i = 0; i < sceneList.Length; i++) {
				if (sceneList[i] != null) {
					LoadScene(sceneList[i]);
				}
			}
		}

		public static void LoadScene(sceneLoader_t scene) {
			if(scene.m_loadSceneMode == loadSceneMode_e.UNLOAD) {
				SceneManager.UnloadScene(scene.name);
			} else {
				SceneManager.LoadScene(scene.name, (LoadSceneMode)scene.m_loadSceneMode);
			}
		}

		public override void TransitionInterrupted() { }
		public override void TransitionStarted() { }
	}
}