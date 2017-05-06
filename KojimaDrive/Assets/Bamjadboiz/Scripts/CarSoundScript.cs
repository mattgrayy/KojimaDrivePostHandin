using UnityEngine;
using System.Collections;

namespace Bam
{
    public class CarSoundScript : MonoBehaviour
    {
        Kojima.CarScript m_myCar;

        //Audio
        [SerializeField]
        Bam.MultiAudioSource m_engine, m_acceleration, m_otherSounds, m_skidSounds;

        float engineVolume = 0.25f, acceleratorVolume = 0.2f;

        float cooldown = 0;

        int curGear = 0;
        float curGearPitchTarget = 1.65f;
        float gearSwitchTimeCounter = 0;

        void Awake()
        {
	
		}

        public void SetGear(int gear)
        {
            curGear = gear;
            curGearPitchTarget = 1.65f - gear * 0.35f;

            m_acceleration.pitch = curGearPitchTarget * 0.4f;

            m_otherSounds.pitch = 1;
            m_otherSounds.PlayOneShot(m_myCar.GetSoundPack.gearChangeSound, 0.375f);

            if(curGear==3)
            {
                curGearPitchTarget = 1.75f;
            }
        }

        // Use this for initialization
        void Start()
        {
            m_myCar = GetComponent<Kojima.CarScript>();

            Initialise();
        }

        public void HonkHorn()
        {
            m_otherSounds.pitch = 1.0f;
            m_otherSounds.PlayOneShot(m_myCar.GetSoundPack.horn, 0.135f);
        }

        void Initialise()
        {
            //Create audio sources
            m_engine = gameObject.AddComponent<Bam.MultiAudioSource>();
            m_engine.spatialBlend = 1.0f;
            m_engine.loop = true;
            m_engine.volume = 0.0f;
            m_engine.clip = m_myCar.GetSoundPack.engine;
            m_engine.Play();
            m_engine.timeSamples = Random.Range(1, m_engine.clip.samples-5);
            m_engine.transform.SetParent(transform);
            m_engine.m_sourceObject.hideFlags = HideFlags.HideInHierarchy;
			m_engine.friendlyName = "[Yams] MAS Engine";

			m_acceleration = gameObject.AddComponent<Bam.MultiAudioSource>();
            m_acceleration.spatialBlend = 1.0f;
            m_acceleration.loop = true;
            m_acceleration.volume = 0;
            m_acceleration.clip = m_myCar.GetSoundPack.acceleration;
            m_acceleration.Play();
            m_acceleration.timeSamples = Random.Range(1, m_acceleration.clip.samples - 5);
            m_acceleration.transform.SetParent(transform);
            m_acceleration.m_sourceObject.hideFlags = HideFlags.HideInHierarchy;
			m_acceleration.friendlyName = "[Yams] MAS Acceleration";

			m_otherSounds = gameObject.AddComponent<Bam.MultiAudioSource>();
            m_otherSounds.spatialBlend = 1.0f;
            m_otherSounds.transform.SetParent(transform);
			m_otherSounds.m_sourceObject.hideFlags = HideFlags.HideInHierarchy;
			m_otherSounds.friendlyName = "[Yams] MAS Other";

			m_skidSounds = gameObject.AddComponent<Bam.MultiAudioSource>();
            m_skidSounds.loop = true;
            m_skidSounds.clip = m_myCar.GetSoundPack.skidding;
            m_skidSounds.volume = 0.0f;
            m_skidSounds.Play();
            m_skidSounds.transform.SetParent(transform);
			m_skidSounds.m_sourceObject.hideFlags = HideFlags.HideInHierarchy;
			m_skidSounds.friendlyName = "[Yams] MAS Skid";

			SetGear(0);
        }

        // Update is called once per frame
        void Update()
        {
            if(cooldown>0)
            {
                cooldown -= Time.deltaTime;
            }

            HandleSkidding();
            HandleEngine();
            HandleAcceleration();
        }

        void HandleAcceleration()
        {
            float targetAccelerationVolume = 0.0f;
            float targetAccelerationPitch = 1;

            targetAccelerationVolume = Mathf.Abs(m_myCar.GetAcceleratorInput);
            targetAccelerationPitch = Mathf.Clamp(0.8f + (m_myCar.m_normalisedForwardVelocity * 0.9f), 0, curGearPitchTarget + 0.1f);

            if (m_myCar.InMidAir)
            {
                targetAccelerationPitch *= 1.5f;
            }

            if (m_myCar.CurrentlyInWater)
            {
                targetAccelerationPitch = 0.7f;
            }

            if (m_acceleration.volume < 0.05f && targetAccelerationVolume > 0.1f)
            {
                m_acceleration.pitch = 0.8f;
            }

            if(m_myCar.CurrentlyGliding)
            {
                targetAccelerationVolume = 0.0f;
                targetAccelerationPitch = 1;
            }

            m_acceleration.volume = Mathf.Lerp(m_acceleration.volume, targetAccelerationVolume * acceleratorVolume, 2 * Time.deltaTime);
            m_acceleration.pitch = Mathf.Lerp(m_acceleration.pitch, targetAccelerationPitch, 4 * Time.deltaTime);

            if (curGear < 3)
            {
                if (m_acceleration.pitch >= curGearPitchTarget)
                {
                    gearSwitchTimeCounter += Time.deltaTime;

                    if (gearSwitchTimeCounter >= 2 + curGear * 0.35f)
                    {
                        SetGear(curGear + 1);
                    }
                }
                else
                {
                    gearSwitchTimeCounter = 0;
                }
            }

            if (m_acceleration.volume < 0.1f && curGear > 0)
            {
                SetGear(0);
            }
        }

        public void PlayOneShot(AudioClip clip, float volume = 1)
        {
            m_otherSounds.pitch = Random.Range(0.9f, 1.1f);
            m_otherSounds.PlayOneShot(clip, volume);
        }

        void HandleEngine()
        {
            float targetEngineVolume = 0.25f;
            float targetEnginePitch = 1;

            targetEnginePitch += m_myCar.m_forwardVelocity * 0.005f;

            if (m_myCar.InMidAir)
            {
                targetEnginePitch *= 1.25f;
            }

            if (m_myCar.CurrentlyInWater)
            {
                targetEnginePitch = 0.7f;
            }

            if(m_myCar.CurrentlyGliding)
            {
                targetEngineVolume = 0.0f;
                targetEnginePitch = 1;
            }

            m_engine.volume = Mathf.Lerp(m_engine.volume, targetEngineVolume * engineVolume, 2 * Time.deltaTime);
            m_engine.pitch = Mathf.Lerp(m_engine.pitch, targetEnginePitch, 4 * Time.deltaTime);
        }

        void HandleSkidding()
        {
            float skidIntensity = m_myCar.GetSkidIntensity();
            skidIntensity = Mathf.Clamp(skidIntensity, 0, 1.1f);

            m_skidSounds.volume = Mathf.Lerp(m_skidSounds.volume, skidIntensity * 0.25f, 5 * Time.deltaTime);
            //skidSounds.pitch = Mathf.Lerp(skidSounds.pitch, 0.8f + (skidIntensity * 0.3f), 0.1f * Time.deltaTime);

            if (m_myCar.InMidAir)
            {
                //skidSounds.volume = 0;
            }
        }

        void FixedUpdate()
        {

        }

        public void WheelHasLanded(float intensity = 1)
        {
            //m_myCar.m_carBody.transform.localPosition -= m_myCar.transform.up * 0.2f;
            //otherSounds.PlayOneShot(smallImpact, 0.25f);
        }

        public void SetSounds(AudioClip _engine, AudioClip _acceleration)
        {
            m_engine.clip = _engine;
            m_engine.Play();

            m_acceleration.clip = _acceleration;
            m_acceleration.Stop();
        }

        void OnDestroy()
        {
            if (m_skidSounds)
            {
                Destroy(m_skidSounds.gameObject);
                Destroy(m_acceleration.gameObject);
                Destroy(m_engine.gameObject);
                Destroy(m_otherSounds.gameObject);
            }
        }

        void OnCollisionEnter(Collision col)
        {
            if (cooldown > 0 || !m_myCar)
                return;

            if (m_myCar.CurrentlyInWater)
                return;

            for (int i = 0; i < col.contacts.Length; i++)
            {

                float dot = Vector3.Dot(col.contacts[i].normal.normalized, col.relativeVelocity.normalized);
                //Debug.Log(col.relativeVelocity.normalized + " / Magnitude of " + col.relativeVelocity.magnitude + " /  Dot: " + dot + " / Intensity: " + dot);

                float intensity = Mathf.Abs(dot) * col.relativeVelocity.magnitude;

                if (intensity > 18.5f)
                {
                    intensity = 18.5f;
                }

                if (m_myCar.GetCam)
                {
                    m_myCar.GetCam.Land(intensity);
                }

                if(col.contacts[i].normal.y < 0.4f)
                {
                    intensity += 1 - col.contacts[i].normal.y;
                }

                m_otherSounds.pitch = Random.Range(0.6f, 1.31f);

                if (Vector3.Dot(transform.up, col.contacts[i].normal) < 0.8f)
                {
                    if (intensity > 6.5f)
                        m_otherSounds.PlayOneShot(m_myCar.m_baseCarInfo.mySoundPack.largeImpact, intensity * 0.025f);
                    else
                        m_otherSounds.PlayOneShot(m_myCar.m_baseCarInfo.mySoundPack.smallImpact, intensity * 0.04f);
                }
                else
                {
                    //Debug.Log("it's only the floor");
                    m_otherSounds.PlayOneShot(m_myCar.m_baseCarInfo.mySoundPack.landing, intensity * 0.04f);
                }
            }

            cooldown = 0.5f;

            //for (int i = 0; i < col.contacts.Length; i++)
            //{
            //    float magnitude = col.relativeVelocity.magnitude;
            //    float dot = Vector3.Dot(col.contacts[i].normal.normalized, m_myCar.GetVelocity().normalized);

            //    float intensity = Mathf.Clamp(Mathf.Abs(dot * 10, 0, 1);

            //    Debug.Log(col.relativeVelocity.normalized + " / Magnitude of " + magnitude + " /  Dot: " + dot + " / Intensity: " + intensity);

            //    if (magnitude > 15 && intensity > 0.1f)
            //    {
            //        if (col.gameObject.layer == 0)
            //        {
            //            if (magnitude > 20)
            //            {
            //                m_otherSounds.PlayOneShot(m_myCar.m_baseCarInfo.mySoundPack.largeImpact, magnitude * 0.01f * intensity);
            //            }
            //            else
            //            {
            //                m_otherSounds.PlayOneShot(m_myCar.m_baseCarInfo.mySoundPack.smallImpact, magnitude * 0.01f * intensity);
            //            }

            //            cooldown = 0.9f;
            //            //Debug.Break();
            //        }
            //    }
            //}
        }
    }
    
}