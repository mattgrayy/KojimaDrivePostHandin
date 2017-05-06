using UnityEngine;
using System.Collections;

namespace Bam
{
    public class SurfaceParticleScript : MonoBehaviour
    {
        GameObject m_grassPrefab, m_rockPrefab, m_sandPrefab;
        AudioClip m_grassClip;

        [SerializeField]
        ParticleSystem[] m_grassParticles, m_rockParticles, m_sandParticles;
        RaycastHit[] m_wheelCasts;
        Kojima.CarScript m_myCar;
        Bam.CarTerrainProperties m_terrainProps;
        AudioSource m_particleSoundSource;

        float volume = 0.55f;

        void Start()
        {
            m_grassClip = ParticleBank.singleton.grassSnd;
            m_myCar = GetComponent<Kojima.CarScript>();
            m_terrainProps = GetComponent<Bam.CarTerrainProperties>();
            m_particleSoundSource = gameObject.AddComponent<AudioSource>();
            m_particleSoundSource.volume = 0;
            m_particleSoundSource.clip = m_grassClip;
            m_particleSoundSource.Play();
            m_particleSoundSource.loop = true;   

            m_grassParticles = new ParticleSystem[4];
            m_rockParticles = new ParticleSystem[4];
            m_sandParticles = new ParticleSystem[4];

            m_grassPrefab = ParticleBank.singleton.grass;
            m_rockPrefab = ParticleBank.singleton.rock;
            m_sandPrefab = ParticleBank.singleton.sand;

            for(int i = 0; i < m_grassParticles.Length; i++)
            {
                m_grassParticles[i] = Instantiate<GameObject>(m_grassPrefab).GetComponent<ParticleSystem>();
                m_grassParticles[i].gameObject.hideFlags = HideFlags.HideInHierarchy;
            }

            for (int i = 0; i < m_rockParticles.Length; i++)
            {
                m_rockParticles[i] = Instantiate<GameObject>(m_rockPrefab).GetComponent<ParticleSystem>();
                m_rockParticles[i].gameObject.hideFlags = HideFlags.HideInHierarchy;
            }

            for (int i = 0; i < m_sandParticles.Length; i++)
            {
                m_sandParticles[i] = Instantiate<GameObject>(m_sandPrefab).GetComponent<ParticleSystem>();
                m_sandParticles[i].gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
        }

        void OnDestroy()
        {
            if(m_grassParticles==null)
            {
                return;
            }

            for(int i = 0; i < m_grassParticles.Length; i++)
            {
                if (m_grassParticles[i] != null)
                {
                    Destroy(m_grassParticles[i].gameObject);
                }
            }

            for (int i = 0; i < m_sandParticles.Length; i++)
            {
                if (m_sandParticles[i] != null)
                {
                    Destroy(m_sandParticles[i].gameObject);
                }
            }

            for (int i = 0; i < m_rockParticles.Length; i++)
            {
                if (m_rockParticles[i] != null)
                {
                    Destroy(m_rockParticles[i].gameObject);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            m_wheelCasts = m_myCar.GetWheelRaycasts;

            if(!m_myCar.AllWheelsGrounded || m_myCar.CurrentlyInWater)
            {
                m_particleSoundSource.volume = Mathf.Lerp(m_particleSoundSource.volume, 0, Time.deltaTime * 2);
            }

            if (m_myCar.IsMoving && !m_myCar.InMidAir && !m_myCar.CurrentlyInWater)
            {
                TerrainProperties.Properties_s curProperties = m_terrainProps.GetCurrentTerrainProperties();
                switch (curProperties.m_friendlyName)
                {
                    case "Grass":
                        m_particleSoundSource.clip = m_grassClip;
                        m_particleSoundSource.volume = Mathf.Lerp(m_particleSoundSource.volume, 0.2f * m_myCar.m_normalisedForwardVelocity * volume, Time.deltaTime * 4);

                        for (int i = 0; i < m_grassParticles.Length; i++)
                        {
                                if (m_myCar.IsWheelGrounded(i))
                                {
                                    m_grassParticles[i].transform.position = m_wheelCasts[i].point;
                                    m_grassParticles[i].Play();
                                    m_rockParticles[i].Stop();
                                    m_sandParticles[i].Stop();
                                ScaleParticles(m_grassParticles[i], 125);
                                }
                                else
                                {
                                    m_grassParticles[i].Stop();
                                    m_rockParticles[i].Stop();
                                    m_sandParticles[i].Stop();
                                }
                           }
                            break;
                    case "Rock":
                        m_particleSoundSource.volume = Mathf.Lerp(m_particleSoundSource.volume, 0.25f * m_myCar.m_normalisedForwardVelocity * volume, Time.deltaTime * 2);

                        for (int i = 0; i < m_rockParticles.Length; i++)
                        {
                            if (m_myCar.IsWheelGrounded(i))
                            {
                                m_rockParticles[i].transform.position = m_wheelCasts[i].point;
                                m_grassParticles[i].Stop();
                                m_rockParticles[i].Play();
                                m_sandParticles[i].Stop();
                                ScaleParticles(m_rockParticles[i], 55);
                            }
                            else
                            {
                                m_grassParticles[i].Stop();
                                m_rockParticles[i].Stop();
                                m_sandParticles[i].Stop();
                            }
                        }
                            break;            
                    case "Sand":
                        m_particleSoundSource.volume = Mathf.Lerp(m_particleSoundSource.volume, 0.25f * m_myCar.m_normalisedForwardVelocity * volume, Time.deltaTime * 2);

                        for (int i = 0; i < m_sandParticles.Length; i++)
                        {
                            if (m_myCar.IsWheelGrounded(i))
                            {
                                m_sandParticles[i].transform.position = m_wheelCasts[i].point;
                                m_grassParticles[i].Stop();
                                m_rockParticles[i].Stop();
                                m_sandParticles[i].Play();
                                ScaleParticles(m_sandParticles[i], 355);
                            }
                            else
                            {
                                m_grassParticles[i].Stop();
                                m_rockParticles[i].Stop();
                                m_sandParticles[i].Stop();
                            }
                        }
                            break;
                    case "Road":
                        m_particleSoundSource.volume = Mathf.Lerp(m_particleSoundSource.volume, 0, Time.deltaTime * 2);

                        for (int i = 0; i < m_sandParticles.Length; i++)
                        {
                            m_grassParticles[i].Stop();
                            m_rockParticles[i].Stop();
                            m_sandParticles[i].Stop();
                        }
                        break;
                }
            }
            else
            {
                m_particleSoundSource.volume = Mathf.Lerp(m_particleSoundSource.volume, 0, Time.deltaTime * 2);

                for (int i = 0; i < m_sandParticles.Length; i++)
                {
                    m_grassParticles[i].Stop();
                    m_rockParticles[i].Stop();
                    m_sandParticles[i].Stop();
                }
            }
        }

        void ScaleParticles(ParticleSystem effect, int emissionAmount)
        {
            ParticleSystem.MinMaxCurve emissionRate = effect.emission.rate;
            emissionRate.mode = ParticleSystemCurveMode.TwoConstants;
            emissionRate.constantMax = emissionAmount * m_myCar.m_forwardVelocity * 0.035f;
            emissionRate.constantMin = emissionAmount * m_myCar.m_forwardVelocity * 0.035f;

            ParticleSystem.EmissionModule em = effect.emission;
            em.rate = emissionRate;
        }
    }
}
