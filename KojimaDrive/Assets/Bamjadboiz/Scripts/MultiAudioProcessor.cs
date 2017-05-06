//===================== Kojima Drive - Bamjadboiz 2017 ====================//
//
// Author:      Orlando Cazalet-Hyams (Yams on Slack)
// Purpose:     This script handles emulating multiple audio listeners (MultiAudioListener) listening
//              out for audio sources (MultiAudioSource).
// Namespace:   Bam
//
//=========================================================================//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bam
{
    public class MultiAudioProcessor : MonoBehaviour
    {
		public float						m_speed = 200;
        public Vector3                      m_safeOffset = new Vector3(0,-1000,0);
		public bool							m_debugInfo = false;
        private static MultiAudioProcessor  s_singleton;
		[SerializeField]//for debugging
        private List<MultiListener>         m_listeners;
		[SerializeField]//for debugging
		private List<MultiAudioSource>      m_sources;
		[SerializeField]//for debugging
		private Dictionary<AudioEffectZone, int> m_effectZones;
		AudioListener                       m_listener;
		
        private void Awake()
        {
            Debug.Assert(s_singleton == null, "[Yams] Only one MultiAudioProcessor per scene!");
            s_singleton = this;
            m_listeners = new List<MultiListener>();
            m_sources = new List<MultiAudioSource>();
			m_effectZones = new Dictionary<AudioEffectZone, int>();

			m_listener = GetComponent<AudioListener>();
            if (m_listener == null) m_listener = gameObject.AddComponent<AudioListener>();
        }

        public static void AddListener(MultiListener listener)
        {
			//Debug.Log("[Yams] Adding listener");
			s_singleton.m_listeners.Add(listener);
        }
        public static void AddSource(MultiAudioSource source)
        {
			//Debug.Log("[Yams] Adding source");
			s_singleton.m_sources.Add(source);
        }
        public static void RemoveListener(MultiListener listener)
        {
			//Debug.Log("[Yams] Removing listener");
            if (s_singleton) {
                s_singleton.m_listeners.Remove(listener);
            } else if (s_singleton.m_debugInfo) {
                Debug.Log("[Yams] Couldn't remove the multi audio listener because a multi audio processor doesn't exist. This is ok if the scene is closing.");
            }
        }
        public static void RemoveSource(MultiAudioSource source)
        {
			//Debug.Log("[Yams] Removing source");
			if (s_singleton) { 
                s_singleton.m_sources.Remove(source);
            } else if (s_singleton.m_debugInfo) {
                Debug.Log("[Yams] Couldn't remove the multi audio source because a multi audio processor doesn't exist. This is ok if the scene is closing.");
            }
        }


		// Update is called once per frame
		void Update()
        {
            transform.position = m_safeOffset;

            foreach (var source in m_sources)
            {
                MultiListener closestListener = null;
                float closest = float.MaxValue;
                Vector3 closestDisplacement = Vector3.zero;
                Quaternion rotDiff = Quaternion.identity;
                //Find closest listener
                foreach (var listener in m_listeners)
                {
                    Vector3 displacement = source.transform.position - listener.transform.position;
					if (displacement.magnitude < closest)
                    {
                        closestListener = listener;
                        closest = displacement.magnitude;
                        closestDisplacement = displacement;
					}
                }

				if (closestListener != null)
				{
					source.output = closestListener.MixerGroup;
					//Get relative position
					Vector3 target = m_safeOffset + (Quaternion.FromToRotation(closestListener.transform.forward, closestDisplacement) * m_listener.transform.forward);
					//Fast lerp to stop janky sounds
					source.m_sourceObject.transform.position = Vector3.Lerp(source.m_sourceObject.transform.position, target, Time.deltaTime * m_speed);
				}
				else
				{
					source.output = null;
				}
			}
        }
    }
}