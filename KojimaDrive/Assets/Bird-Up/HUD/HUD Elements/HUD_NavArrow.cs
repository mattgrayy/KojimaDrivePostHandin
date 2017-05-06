//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Jordan Hunt (integrated into HUDElement baseclass by Sam Morris)
// Purpose: Holds some details for our arrow
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird
{

	// BUGBUG: Currently the arrow rotates local to the car and does no camera-based fixing up - this needs sorting! -sam 23/04/2017
	public class HUD_NavArrow : HUDElement
	{

		public GameObject m_ModelPrefab;
		public GameObject m_SpawnedModel;
		public GameObject m_LocalArrowRotationPrefab;
		Transform m_ArrowModelTransform;
		public Material m_ArrowOpaqueMaterial;
		public Material m_ArrowFadeMaterial;
		protected Renderer m_Renderer; 

		public Transform carArrow;
		public Kojima.CarScript m_TargetPlayer;
		private ArrowCheckpoints cArrowCheck;
		private Material mat; // current mat

		private float currentAlpha;

		void Start()
		{
			m_SpawnedModel = Instantiate(m_ModelPrefab);
			m_SpawnedModel.transform.position = transform.position;
			WorldArrowInfo info = m_SpawnedModel.GetComponent<WorldArrowInfo>();
			m_ArrowModelTransform = info.m_RotationPivot;

			// make material instances
			m_ArrowOpaqueMaterial = Instantiate(m_ArrowOpaqueMaterial);
			m_ArrowFadeMaterial = Instantiate(m_ArrowFadeMaterial);

			info.m_MeshRenderer.material = m_ArrowOpaqueMaterial;
			m_Renderer = info.m_MeshRenderer;
			mat = m_ArrowOpaqueMaterial;
			InitScale();

			SetupPlayerPivot();

			Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.CAR_SWAPPED, OnCarSwap);
		}

		void SetupPlayerPivot() {
			m_TargetPlayer = Kojima.GameController.s_singleton.m_players[m_ParentController.m_nPlayer - 1];
			GameObject spawnedRotation = Instantiate(m_LocalArrowRotationPrefab);
			spawnedRotation.transform.SetParent(m_TargetPlayer.transform);
			spawnedRotation.transform.localPosition = Vector3.zero;
			carArrow = spawnedRotation.transform;
			cArrowCheck = carArrow.GetComponent<ArrowCheckpoints>();
			cArrowCheck.SetUIArrow(this);
		}

		protected override void OnDestroy() {
			Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.CAR_SWAPPED, OnCarSwap);
			base.OnDestroy();
		}

		protected override void OnDisplay() {
			m_SpawnedModel.SetActive(true);
			base.OnDisplay();
		}
		protected override void OnHide() {
			m_SpawnedModel.SetActive(false);
			base.OnHide();
		}

		void OnCarSwap(object data) {
			// Malformed event call?
			Kojima.CarSwapManager.carSwapEventData_t dataobj = (Kojima.CarSwapManager.carSwapEventData_t)data;
			if(dataobj == null) {
				return;
			}

			// It's us who's changed?
			if(dataobj.m_nPlayerID != m_ParentController.m_nPlayer) {
				return;
			}

			// Okay, so a car swap just happened. Set up our arrow pivot again.
			// This is a hack, but since this version of unity doesn't support Matrix4x4.LookAt, it's the best we can do.
			SetupPlayerPivot();
		}

		public override void UpdateHUDElement() {
			if (carArrow != null && m_ArrowModelTransform != null) {
				if (cArrowCheck == null) {
					cArrowCheck = carArrow.GetComponent<ArrowCheckpoints>();
					cArrowCheck.SetUIArrow(this);
				}

				if(Kojima.CameraManagerScript.singleton == null) {
					return;
				}

				Vector3 camRotation = Kojima.CameraManagerScript.singleton.playerCameras[m_ParentController.m_nPlayer - 1].transform.localEulerAngles;
				Vector3 carRotation = Kojima.GameController.s_singleton.m_players[m_ParentController.m_nPlayer - 1].transform.localEulerAngles;
				Vector3 rotationDiff = carRotation - camRotation;
				float fCameraOffset = rotationDiff.y;

				m_ArrowModelTransform.localEulerAngles = new Vector3(0, carArrow.localEulerAngles.y + fCameraOffset, 0);
			}

			if (cArrowCheck) {
				// Disable our arrow if we've got no targets
				m_SpawnedModel.SetActive(cArrowCheck.currentCheckpoints.Count != 0);
			}
		}

		public void LerpMaterialColour(float lValue)
		{
			mat.color = Color.Lerp(Color.red, Color.green, lValue);
		}

		public void LerpMaterial(float lValueCol, float fThrobLerp, float lValueFade) {
			LerpMaterialColour(lValueCol);
			LerpModelScale(fThrobLerp);
			LerpMeshAlpha(lValueFade);
			SetAlpha();
		}

		Vector3 m_InitialScale;
		Vector3 m_LargestScale;
		float m_fScaleThrob = 0.0f;
		float m_fScaleThrobSpeed = 1.0f;
		void InitScale() {
			m_InitialScale = m_Renderer.transform.localScale;
			m_LargestScale = m_InitialScale * 1.5f;
		}

		public void LerpModelScale(float fLerp) {
			m_fScaleThrob = Mathf.Clamp(m_fScaleThrob + (m_fScaleThrobSpeed * Time.deltaTime), 0.0f, 1.0f);
			if(m_fScaleThrob == 1.0f || m_fScaleThrob == 0.0f) {
				m_fScaleThrobSpeed = -m_fScaleThrobSpeed;
			}

			float lerpVal = m_fScaleThrob * m_fScaleThrob * (3f - 2f * m_fScaleThrob); // 'smoothstep' formula from https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
			m_Renderer.transform.localScale = Vector3.Lerp(m_InitialScale, m_LargestScale, fLerp * lerpVal);
		}

		public void LerpMeshAlpha(float lValue)
		{
			float newAlpha = Mathf.Clamp(Mathf.Lerp(0, 1, lValue), 0, 1);

			if (newAlpha != currentAlpha) {
				if(newAlpha >= 1.0f) {
					mat = m_ArrowOpaqueMaterial;
				} else {
					mat = m_ArrowFadeMaterial;
				}

				m_Renderer.material = mat;
			}

			currentAlpha = newAlpha;
		}

		void SetAlpha()
		{
			mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, currentAlpha);
		}
	}
}