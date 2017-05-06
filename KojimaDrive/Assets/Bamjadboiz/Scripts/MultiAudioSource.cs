//===================== Kojima Drive - Bamjadboiz 2017 ====================//
//
// Author:      Orlando Cazalet-Hyams (Yams on Slack)
// Purpose:     This script acts as a wrapper for Unity's AudioSource. Use this with
//              MultiAudioSource scripts in a scene with a MultiAudioProcessor.
// Namespace:   Bam
//
//=========================================================================//

using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
/*NOTICE: Didn't follow coding style to maintain continuity between unity stuff*/
namespace Bam
{
    public class MultiAudioSource : MonoBehaviour
    {
		public AudioClip    clip            { get { return m_clip; } set { m_clip = value; m_audioSource.clip = value; } }
        public AudioMixerGroup output       { get { return m_output; } set { m_output = value; m_audioSource.outputAudioMixerGroup = value; } }
        public bool         mute            { get { return m_mute; } set { m_mute = value; m_audioSource.mute = value; } }
        public bool         bypassEffects   { get { return m_bypassEffects; } set { m_bypassEffects = value; m_audioSource.bypassEffects = value; } }
        public bool         bypassListenerEffects { get { return m_bypassListenerEffects; } set { m_bypassListenerEffects = value; m_audioSource.bypassListenerEffects = value; } }
        public bool         bypassReverZones{ get { return m_bypassReverbZones; } set { m_bypassReverbZones = value; m_audioSource.bypassReverbZones = value; } }
        public bool         playOnAwake     { get { return m_playOnAwake; } set { m_playOnAwake = value; m_audioSource.playOnAwake = value; } }
        public bool         loop            { get { return m_loop; } set { m_loop = value; m_audioSource.loop = value; } }
        public int          priority        { get { return m_priority; } set { m_priority = value; m_audioSource.priority = value; } }
        public float        volume          { get { return m_volume; } set { m_volume = value; m_audioSource.volume = value; } }
        public float        pitch           { get { return m_pitch; } set { m_pitch = value; m_audioSource.pitch = value; } }
        public float        stereoPan       { get { return m_stereoPan; } set { m_stereoPan = value; m_audioSource.panStereo = value; } }
        public float        spatialBlend    { get { return m_spatialBlend; } set { m_spatialBlend = value; m_audioSource.spatialBlend = value; } }
        public float        reverbZoneMix   { get { return m_reverbZoneMix; } set { m_reverbZoneMix = value; m_audioSource.reverbZoneMix = value; } }
        public float        dopplerLevel    { get { return m_dopplerLevel; } set { m_dopplerLevel = value; m_audioSource.dopplerLevel = value; } }
        public float        spread          { get { return m_spread; } set { m_spread = value; m_audioSource.spread = value; } }
        public AudioRolloffMode volumeRolloff { get { return m_volumeRolloff; } set { m_volumeRolloff = value; m_audioSource.rolloffMode = value; } }
        public float        minDistance     { get { return m_minDistance; } set { m_minDistance = value; m_audioSource.minDistance = value; } }
        public float        maxDistance     { get { return m_maxDistance; } set { m_maxDistance = value; m_audioSource.maxDistance = value; } }
        
        public AudioVelocityUpdateMode VelocityUpdateMode { get { return m_velocityUpdateMode; } set { m_velocityUpdateMode = value; m_audioSource.velocityUpdateMode = value; } }
        public int          timeSamples     { get { return m_timeSamples; } set { m_timeSamples = value;  m_audioSource.timeSamples = value; } }
        public float        time            { get { return m_time; } set { m_time = value;  m_audioSource.time = value; } }
        public bool         spatialize      { get { return m_spatialize; } set { m_spatialize = value;  m_audioSource.spatialize = value; } }
        public bool         ignoreListenerVolume { get { return m_ignoreListenerVolume; } set { m_ignoreListenerVolume = value; m_audioSource.ignoreListenerVolume = value; } }
        public bool         ignoreListenerPause { get { return m_ignoreListenerPause; } set { m_ignoreListenerPause = value; m_audioSource.ignoreListenerPause = value; } }
		public string		friendlyName	{ get { return m_friendlyName;  } set { if (m_sourceObject) m_sourceObject.name = value; m_friendlyName = value;} }
		public bool			dontDestroyOnLoad { get { return m_dontDestroyOnLoad; } set { if (value) { DontDestroyOnLoad(); } m_dontDestroyOnLoad = value; } }

		[SerializeField]
		private bool m_dontDestroyOnLoad = false;
		[SerializeField] private string m_friendlyName = "Local Multiplayer Audio Source";
		[SerializeField] private AudioClip m_clip;
        [SerializeField] private AudioMixerGroup m_output;
        [SerializeField] private bool m_mute;
        [SerializeField] private bool m_bypassEffects;
        [SerializeField] private bool m_bypassListenerEffects;
        [SerializeField] private bool m_bypassReverbZones;
        [SerializeField] private bool m_playOnAwake = true;
        [SerializeField] private bool m_loop;
        [SerializeField][Range(0, 256)] private int m_priority = 128;
        [SerializeField][Range(0,1)]    private float m_volume =1;
        [SerializeField][Range(-3,3)]   private float m_pitch = 1;
        [SerializeField][Range(-1, 1)]  private float m_stereoPan = 0;
        [SerializeField][Range(0,1)]    private float m_spatialBlend = 1;
        [SerializeField][Range(0, 1.1f)]private float m_reverbZoneMix = 1;
        [SerializeField][Range(0,5)]    private float m_dopplerLevel = 1;
        [SerializeField][Range(0, 360)] private float m_spread;
        [SerializeField] AudioRolloffMode m_volumeRolloff = AudioRolloffMode.Logarithmic;
        [SerializeField] private float m_minDistance=1;
        [SerializeField] private float m_maxDistance = 500;

        private AudioVelocityUpdateMode m_velocityUpdateMode;
        private int m_timeSamples = 0;
        private float m_time = 0;
        private bool m_spatialize = false;
        private bool m_ignoreListenerVolume = false;
        private bool m_ignoreListenerPause = false;

	
        [HideInInspector] public GameObject m_sourceObject;
        [HideInInspector] public AudioSource m_audioSource;
		// Use this for initialization

		//Should be called for audiosources on objects that are not destroyed on loading
		public void DontDestroyOnLoad()
		{
			ObjectDB.DontDestroyOnLoad_Managed(this);
			ObjectDB.DontDestroyOnLoad_Managed(m_sourceObject);
		}

		private void OnLevelWasLoaded(int level)
		{
			if (!m_dontDestroyOnLoad)
			{
				Destroy(m_sourceObject);
				Destroy(this);
			}
		}

		public void PlayOneShot(AudioClip clip)
        {
            m_audioSource.PlayOneShot(clip);
        }

        public void PlayOneShot(AudioClip clip, float volumeScale)
        {
            m_audioSource.PlayOneShot(clip, volumeScale);
        }

        public void PlayDelayed(float time)
        {
            m_audioSource.PlayDelayed(time);
        }

        public void Play()
        {
            m_audioSource.Play();
        }

        public void Play(ulong delay)
        {
            m_audioSource.Play(delay);
        }

        public void Pause()
        {
            m_audioSource.Pause();
        }

        public void Unpause()
        {
            m_audioSource.UnPause();
        }

        public void Stop()
        {
            m_audioSource.Stop();
        }

        public void SetSpatializerFloat(int index, float value)
        {
            m_audioSource.SetSpatializerFloat(index, value);
        }

        public void SetScheduledEndtime(double time)
        {
            m_audioSource.SetScheduledEndTime(time);
        }

        public void SetScheduledStartTime(double time)
        {
            m_audioSource.SetScheduledStartTime(time);
        }

        public AnimationCurve GetCustomCurve(AudioSourceCurveType type)
        {
            return m_audioSource.GetCustomCurve(type);
        }

        //public static void PlayClipAtPoint(AudioClip clip, Vector3 position)
        //{
        //    AudioSource.PlayClipAtPoint(clip, position);
        //}

        //public static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume)
        //{
        //    AudioSource.PlayClipAtPoint(clip, position, volume);
        //}

        void OnValidate()
        {
            if (m_audioSource != null)
            {
                UpdateSource();
            }
        }

        private void UpdateSource()
        {
            m_audioSource.clip = m_clip;
            m_audioSource.outputAudioMixerGroup = m_output;
            m_audioSource.mute = m_mute;
            m_audioSource.bypassEffects = m_bypassEffects;
            m_audioSource.bypassListenerEffects = m_bypassListenerEffects;
            m_audioSource.bypassReverbZones = m_bypassReverbZones;
            m_audioSource.playOnAwake = m_playOnAwake;
            m_audioSource.loop = m_loop;
            m_audioSource.priority = m_priority;
            m_audioSource.volume = m_volume;
            m_audioSource.pitch = m_pitch;
            m_audioSource.panStereo = m_stereoPan;
            m_audioSource.spatialBlend = m_spatialBlend;
            m_audioSource.reverbZoneMix = m_reverbZoneMix;
            m_audioSource.dopplerLevel = m_dopplerLevel;
            m_audioSource.spread = m_spread;
            m_audioSource.rolloffMode = m_volumeRolloff;
            m_audioSource.minDistance = m_minDistance;
            m_audioSource.maxDistance = m_maxDistance;
            m_audioSource.velocityUpdateMode = m_velocityUpdateMode;
            m_audioSource.timeSamples = m_timeSamples;
            m_audioSource.time = m_time;
            m_audioSource.ignoreListenerVolume = m_ignoreListenerVolume;
            m_audioSource.ignoreListenerPause = m_ignoreListenerPause;
			if (m_dontDestroyOnLoad) DontDestroyOnLoad();

		}

        void Awake()
        {
            m_sourceObject = new GameObject(m_friendlyName);
            m_audioSource = m_sourceObject.AddComponent<AudioSource>();
            UpdateSource();
            if (m_playOnAwake) m_audioSource.Play();
        }

        void Start()
        {
            MultiAudioProcessor.AddSource(this);
        }

        private void OnDestroy()
        {
            MultiAudioProcessor.RemoveSource(this);

            if (m_sourceObject)
            {
                Destroy(m_sourceObject);
            }
        }
    }
}