using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class ThunderSystem : MonoBehaviour
    {
		private bool m_active;
		private static LightningFlashScript s_lightning;
		private bool m_flashManager;
		public GameObject m_lightning;
		public float m_minFlashInterval = 5.0f;
		public float m_maxFlashInterval = 6.0f;
		private float m_flashTimer;
		private float m_flashTimerTarget;

		private ParticleSystem m_particleSystem;

		private float m_fEndParticleTimerStart;
		private float m_fEndParticleTimerEnd;

		void Awake()
		{
			m_active = false;
			m_flashManager = false;
			if(!s_lightning)
			{
				GameObject lightning = Instantiate(m_lightning);
				s_lightning = lightning.GetComponent<LightningFlashScript>();
				m_flashManager = true;
				m_flashTimer = 0.0f;
			}
			//This timer makes sure that the rain stops falling and don't suddenly just disappear
			m_fEndParticleTimerStart = 0.0f;
			m_fEndParticleTimerEnd = 5.0f;
			m_particleSystem = GetComponent<ParticleSystem>();
			if (m_particleSystem)
			{
				var emissionModule = m_particleSystem.emission;
				emissionModule.enabled = false;
			}
		}
		
		void Update()
		{
			if(m_flashManager && m_active)
			{
				m_flashTimer += Time.deltaTime;
				if(m_flashTimer > m_flashTimerTarget)
				{
					m_flashTimer = 0.0f;
					m_flashTimerTarget = Random.Range(m_minFlashInterval, m_maxFlashInterval);
					s_lightning.Flash();
				}
			}
		}

		public void Init()
		{
			//We turn on emission on the particle system as we always turn it off in the end
			if (m_particleSystem)
			{
				var emissionModule = m_particleSystem.emission;
				emissionModule.enabled = true;
			}
			if(m_flashManager)
			{
				m_flashTimer = 0.0f;
				m_flashTimerTarget = Random.Range(m_minFlashInterval, m_maxFlashInterval);
			}
			m_active = true;
		}

		public void ExitWeatherEffect()
		{
			if (m_particleSystem)
			{
				m_fEndParticleTimerStart += Time.deltaTime;
				var emissionModule = m_particleSystem.emission;
				emissionModule.enabled = false;

				if (m_fEndParticleTimerStart > m_fEndParticleTimerEnd)
				{
					m_fEndParticleTimerStart = 0.0f;
				}
			}
			if(m_flashManager)
			{
				s_lightning.Cease();
			}
			m_active = false;
		}
	}
}
