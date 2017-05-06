//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Special camera that deals with aspect ratio for 'screenspace' UI.
//			Intended as something of an alternative to the Unity Canvas UI.
// Namespace: Bird 
//
//===============================================================================//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bird {
	public enum aspectRatio_e {
		ASPECT_4x3,
		ASPECT_5x4,
		ASPECT_16x9,
		ASPECT_16x10,
		ASPECT_3x2,
		ASPECT_CUSTOM
	}

	[System.Serializable]
	public class aspectRatio_s {
		public aspectRatio_e m_AspectRatio;
		public Vector2 m_CustomAspectRatio;

		public Vector2 GetAspect() {
			switch (m_AspectRatio) {
				case aspectRatio_e.ASPECT_4x3:
					return new Vector2(4, 3);
				case aspectRatio_e.ASPECT_5x4:
					return new Vector2(5, 4);
				case aspectRatio_e.ASPECT_16x9:
					return new Vector2(16, 9);
				case aspectRatio_e.ASPECT_16x10:
					return new Vector2(16, 10);
				case aspectRatio_e.ASPECT_3x2:
					return new Vector2(3, 2);
				case aspectRatio_e.ASPECT_CUSTOM:
				default:
					return m_CustomAspectRatio;
			}
		}

		public void MatchAspect(float fAspect) {
			if(fAspect == 4f/3f) {
				m_AspectRatio = aspectRatio_e.ASPECT_4x3;
			} else if (fAspect == 5f/4f) {
				m_AspectRatio = aspectRatio_e.ASPECT_5x4;
			} else if (fAspect == 16f/9f) {
				m_AspectRatio = aspectRatio_e.ASPECT_16x9;
			} else if (fAspect == 16f/10f) {
				m_AspectRatio = aspectRatio_e.ASPECT_16x10;
			} else if (fAspect == 3f/2f) {
				m_AspectRatio = aspectRatio_e.ASPECT_3x2;
			} else {
				m_AspectRatio = aspectRatio_e.ASPECT_CUSTOM; // TODO: Add some proper detection here!
				Vector2 vecAspect;
				vecAspect.x = Screen.width;
				vecAspect.y = Screen.height;
				float fGcd = GCD(vecAspect.x, vecAspect.y);
				m_CustomAspectRatio = vecAspect / fGcd;
			}
		}

		float GCD(float a, float b) {
			while (b > 0) {
				float rem = a % b;
				a = b;
				b = rem;
			}
			return a;
		}

		public void MatchAspect(Vector2 vecAspect, bool bSimplify) {
			if(bSimplify) {
				float fGcd = GCD(vecAspect.x, vecAspect.y);
				vecAspect = vecAspect / fGcd;
			}

			if (vecAspect == new Vector2(4, 3)) {
				m_AspectRatio = aspectRatio_e.ASPECT_4x3;
			} else if (vecAspect == new Vector2(5, 4)) {
				m_AspectRatio = aspectRatio_e.ASPECT_5x4;
			} else if (vecAspect == new Vector2(16, 9)) {
				m_AspectRatio = aspectRatio_e.ASPECT_16x9;
			} else if (vecAspect == new Vector2(16, 10)) {
				m_AspectRatio = aspectRatio_e.ASPECT_16x10;
			} else if (vecAspect == new Vector2(3, 2)) {
				m_AspectRatio = aspectRatio_e.ASPECT_3x2;
			} else {
				m_AspectRatio = aspectRatio_e.ASPECT_CUSTOM;
				m_CustomAspectRatio = vecAspect;
			}
		}
	}

	[ExecuteInEditMode]
	public class ScreenspaceUICamera : MonoBehaviour {
		// Aspect ratio this camera was created at
		[Header("Aspect Ratio")][Tooltip("The aspect ratio our UI is created at (typically 16:9)")]
		public aspectRatio_s m_InternalAspectRatio;
		[Tooltip("The aspect ratio our UI is scaled to simulate. If changed in Edit mode, this will not function correctly.")]
		public aspectRatio_s m_CurrentAspectRatio;

		// Target resolution on most systems is 1080p, so our internal res is 1080p
		[Header("Resolution")][Tooltip("Resolution that our UI is built at (typically 1920x1080)")]
		public Vector2 m_InternalResolution = new Vector2(1920, 1080);
		[Tooltip("Resolution that our UI is scaled to simulate. If changed in Edit mode, this will not function correctly.")]
		public Vector2 m_TargetResolution;
		[HideInInspector]
		public Vector3 m_InternalScale;
		[HideInInspector]
		public Vector3 m_PrevInternalScale;

		[Name("Object Container")][NotNull][Header("Misc")]
		public GameObject m_ObjectContainer;

		[HideInInspector]
		public Camera m_Cam;
		[HideInInspector]
		public Vector2 m_ScaleChange;
		[HideInInspector]
		public float m_fPositionChange;
		[HideInInspector]
		public List<ScreenspaceUIObject> m_OwnedObjects = new List<ScreenspaceUIObject>();

		void OnValidate() {
			m_Cam = GetComponent<Camera>();

			ChangeAspect();
		}

		void ChangeAspect() {
			PreAdjustObjects();
			CalculateInternalResolution();
			CalculateTargetResolution();
			CalculateInternalScale();
			AdjustObjects();
			transform.localScale = m_InternalScale;
		}

		// Unity ORTHOGRAPHICSIZE val is the Y value of the aspect ratio!

		void Awake() {
			m_Cam = GetComponent<Camera>();

			if (Application.isPlaying) {
				m_PrevInternalRes = m_InternalResolution;
				m_PrevInternalScale = m_InternalScale;

				float fCamAspect = m_Cam.aspect;
				m_CurrentAspectRatio.MatchAspect(fCamAspect);
				ChangeAspect();
			}
		}

		Vector2 m_PrevInternalRes;
		void CalculateInternalResolution() {
			Vector2 aspect = m_InternalAspectRatio.GetAspect();

			if (m_PrevInternalRes.y != m_InternalResolution.y) {
				m_InternalResolution.x = (m_InternalResolution.y / aspect.y) * aspect.x;
			}

			if (m_PrevInternalRes.x != m_InternalResolution.x) {
				m_InternalResolution.y = (m_InternalResolution.x / aspect.x) * aspect.y;
			}

			m_PrevInternalRes = m_InternalResolution;
		}

		void CalculateTargetResolution() {
			Vector2 targetaspect = m_CurrentAspectRatio.GetAspect();
			Vector2 prevRes = m_TargetResolution;
			m_TargetResolution.x = (m_InternalResolution.y / targetaspect.y) * targetaspect.x;
			m_TargetResolution.y = m_InternalResolution.y;

			m_ScaleChange = m_TargetResolution;
			m_ScaleChange.x /= prevRes.x;
			m_ScaleChange.y /= prevRes.y;

			m_fPositionChange = (prevRes.x - m_TargetResolution.x) / 2.0f;
		}

		void CalculateInternalScale() {
			m_PrevInternalScale = m_InternalScale;

			Vector2 aspect = m_InternalAspectRatio.GetAspect();
			m_InternalScale.y = (m_Cam.orthographicSize / m_InternalResolution.y) * 2.0f;
			m_InternalScale.x = (m_InternalScale.y / m_InternalResolution.x) * m_TargetResolution.x;
			m_InternalScale.z = 1.0f;
		}

		void PreAdjustObjects() {
			for (int i = 0; i < m_OwnedObjects.Count; i++) {
				m_OwnedObjects[i].SaveCurrentState(m_ObjectContainer.transform);
			}
		}

		void AdjustObjects() {
			for (int i = 0; i < m_OwnedObjects.Count; i++) {
				m_OwnedObjects[i].AdjustObject(this);
			}
		}

		public void RegisterObject(ScreenspaceUIObject obj) {
			m_OwnedObjects.Add(obj);
			if (obj.transform.parent == null) {
				obj.transform.SetParent(m_ObjectContainer.transform);
			}
		}

		public void UnregisterObject(ScreenspaceUIObject obj) {
			m_OwnedObjects.Remove(obj);
		}

		public void OnApplicationQuit() {
			m_CurrentAspectRatio.m_AspectRatio = m_InternalAspectRatio.m_AspectRatio;
			m_CurrentAspectRatio.m_CustomAspectRatio = m_InternalAspectRatio.m_CustomAspectRatio;
			ChangeAspect();
		}
	}
}