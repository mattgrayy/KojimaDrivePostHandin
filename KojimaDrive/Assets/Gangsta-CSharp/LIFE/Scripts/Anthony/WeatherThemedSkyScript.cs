using UnityEngine;
using System.Collections;

namespace GCSharp
{
	public class WeatherThemedSkyScript : MonoBehaviour
	{
		private Material m_skybox;
		private WeatherManager m_weatherManagerScript;

		private int m_matchingColours;

		public float m_fadeTime = 1.5f;
		[SerializeField]
		private float m_fadeTimer;

		private Color m_prevSkyTint;
		[SerializeField]
		private Color m_currentSkyTint;
		private Color m_targetSkyTint;
		
		private Color m_prevGroundColour;
		[SerializeField]
		private Color m_currentGroundColour;
		private Color m_targetGroundColour;
		
		private float m_prevAtmosphereThiccness;
		[SerializeField]
		private float m_currentAtmosphereThiccness;
		private float m_targetAtmosphereThiccness;

		private float m_prevExposure;
		[SerializeField]
		private float m_currentExposure;
		private float m_targetExposure;

		public Color m_defaultSkyboxSkyTint;
		public Color m_defaultSkyboxGroundCol;
		public float m_defaultSkyAtmosphereThiccness;
		public float m_defaultExposure;
		public Color m_rainSkyboxSkyTint;
		public Color m_rainSkyboxGroundCol;
		public float m_rainSkyAtmosphereThiccness;
		public float m_rainExposure;
		public Color m_snowSkyboxSkyTint;
		public Color m_snowSkyboxGroundCol;
		public float m_snowSkyAtmosphereThiccness;
		public float m_snowExposure;
		public Color m_fogSkyboxSkyTint;
		public Color m_fogSkyboxGroundCol;
		public float m_fogSkyAtmosphereThiccness;
		public float m_fogExposure;
		public Color m_lightningSkyboxSkyTint;
		public Color m_lightningSkyboxGroundCol;
		public float m_lightningSkyAtmosphereThiccness;
		public float m_lightningExposure;

		// Use this for initialization
		void Start()
		{
			m_skybox = RenderSettings.skybox;
			m_matchingColours = 0;
			m_fadeTimer = 0.0f;
			m_prevSkyTint = m_skybox.GetColor("_SkyTint");
			m_currentSkyTint = m_skybox.GetColor("_SkyTint");
			m_currentGroundColour = m_skybox.GetColor("_GroundColor");
			m_currentAtmosphereThiccness = m_skybox.GetFloat("_AtmosphereThickness");
			m_currentExposure = m_skybox.GetFloat("_Exposure");
			ResetTargets();
		}

		// Update is called once per frame
		void Update()
		{
			m_skybox = RenderSettings.skybox;
			if (m_matchingColours < 4)
			{
				m_fadeTimer += Time.deltaTime;
				if (m_fadeTimer > m_fadeTime)
				{
					m_fadeTimer = m_fadeTime;
				}
			}
			m_matchingColours = 0;
			if (m_currentSkyTint != m_targetSkyTint)
			{
				m_currentSkyTint = Color.Lerp(m_prevSkyTint, m_targetSkyTint, m_fadeTimer/m_fadeTime);
			}
			else
			{
				m_matchingColours++;
			}

			if (m_currentGroundColour != m_targetGroundColour)
			{
				m_currentGroundColour = Color.Lerp(m_prevGroundColour, m_targetGroundColour, m_fadeTimer / m_fadeTime);
			}
			else
			{
				m_matchingColours++;
			}

			if (m_currentAtmosphereThiccness != m_targetAtmosphereThiccness)
			{
				m_currentAtmosphereThiccness = Mathf.Lerp(m_prevAtmosphereThiccness, m_targetAtmosphereThiccness, m_fadeTimer / m_fadeTime);
			}
			else
			{
				m_matchingColours++;
			}

			if (m_currentExposure != m_targetExposure)
			{
				m_currentExposure = Mathf.Lerp(m_prevExposure, m_targetExposure, m_fadeTimer / m_fadeTime);
			}
			else
			{
				m_matchingColours++;
			}

			if (m_matchingColours == 4 && m_fadeTimer == m_fadeTime)
			{
				m_fadeTimer = 0.0f;
			}

			m_skybox.SetColor("_SkyTint", m_currentSkyTint);
			m_skybox.SetColor("_GroundColor", m_currentGroundColour);
			m_skybox.SetFloat("_AtmosphereThickness", m_currentAtmosphereThiccness);
			m_skybox.SetFloat("_Exposure", m_currentExposure);
		}

		private void SetPrevVariables()
		{
			m_prevExposure = m_targetExposure;
			m_prevAtmosphereThiccness = m_targetAtmosphereThiccness;
			m_prevGroundColour = m_targetGroundColour;
			m_prevSkyTint = m_targetSkyTint;
		}

		public void ResetTargets()
		{
			SetPrevVariables();
			m_targetAtmosphereThiccness = m_defaultSkyAtmosphereThiccness;
			m_targetGroundColour = m_defaultSkyboxGroundCol;
			m_targetSkyTint = m_defaultSkyboxSkyTint;
			m_targetExposure = m_defaultExposure;
		}

		public void SetRainTargets()
		{
			SetPrevVariables();
			m_targetAtmosphereThiccness = m_rainSkyAtmosphereThiccness;
			m_targetGroundColour = m_rainSkyboxGroundCol;
			m_targetSkyTint = m_rainSkyboxSkyTint;
			m_targetExposure = m_rainExposure;
		}

		public void SetSnowTargets()
		{
			SetPrevVariables();
			m_targetAtmosphereThiccness = m_snowSkyAtmosphereThiccness;
			m_targetGroundColour = m_snowSkyboxGroundCol;
			m_targetSkyTint = m_snowSkyboxSkyTint;
			m_targetExposure = m_snowExposure;
		}

		public void SetFogTargets()
		{
			SetPrevVariables();
			m_targetAtmosphereThiccness = m_fogSkyAtmosphereThiccness;
			m_targetGroundColour = m_fogSkyboxGroundCol;
			m_targetSkyTint = m_fogSkyboxSkyTint;
			m_targetExposure = m_fogExposure;
		}

		public void SetThunderTargets()
		{
			SetPrevVariables();
			m_targetAtmosphereThiccness = m_lightningSkyAtmosphereThiccness;
			m_targetGroundColour = m_lightningSkyboxGroundCol;
			m_targetSkyTint = m_lightningSkyboxSkyTint;
			m_targetExposure = m_lightningExposure;
		}
	}
}