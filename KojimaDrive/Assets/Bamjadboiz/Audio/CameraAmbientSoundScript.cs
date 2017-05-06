using UnityEngine;
using System.Collections;

namespace Bam
{
    public class CameraAmbientSoundScript : MonoBehaviour
    {
        [SerializeField]
        AudioClip oceanSound, windSound;

        MultiAudioSource oceanSource, windSource;

        public float oceanHeight = 25;
        public float windHeight = 54;

        // Use this for initialization
        void Start()
        {
            oceanSource = SetupSource(oceanSound);
            windSource = SetupSource(windSound);
        }

        MultiAudioSource SetupSource(AudioClip clip)
        {
            MultiAudioSource newSource = gameObject.AddComponent<MultiAudioSource>();
            newSource.loop = true;
            newSource.volume = 0;
            newSource.clip = clip;
			newSource.dontDestroyOnLoad = true;

			newSource.Play();

            newSource.m_sourceObject.hideFlags = HideFlags.HideInHierarchy;
            return newSource;
        }

        // Update is called once per frame
        void Update()
        {
            float windVolume = 0;

            if (transform.position.y != 0)
            {
                windVolume = (transform.position.y / windHeight) * 0.025f / Kojima.GameController.s_ncurrentPlayers;
            }

            windSource.volume = Mathf.Lerp(0, 1, windVolume);
        }
    }
}