using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class FogSystem : MonoBehaviour
    {
        private ParticleSystem m_particleSystem;

        private float m_fEndParticleTimerStart;
        private float m_fEndParticleTimerEnd;

        void Awake()
        {
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

        public void Init()
        {
            //We turn on emission on the particle system as we always turn it off in the end
            if (m_particleSystem)
            {
                var emissionModule = m_particleSystem.emission;
                emissionModule.enabled = true;
            }

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
        }
    }
}
