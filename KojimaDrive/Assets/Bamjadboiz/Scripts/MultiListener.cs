//===================== Kojima Drive - Bamjadboiz 2017 ====================//
//
// Author:      Orlando Cazalet-Hyams (Yams on Slack)
// Purpose:     This script acts as a listener for split screen local multiplayer.
//				Listens for MultiAudioSource components.
// Namespace:   Bam
//
//==========================================================================//

using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace Bam
{
    public class MultiListener : MonoBehaviour
    {
 

		public static bool pause { get { return AudioListener.pause; } set { AudioListener.pause = value; } }
		public static float volume { get { return AudioListener.volume; } set { AudioListener.volume = value; } }
		public static string TagString { get { return "MultiSourceListener"; } }

		public AudioMixerGroup MixerGroup { get; private set; }

		private List<AudioEffectZone> m_effectZones;

		public void AddEffectZone(AudioEffectZone effectZone)
		{
			m_effectZones.Add(effectZone);
		}

		public void RemoveEffectZone(AudioEffectZone effectZone)
		{
			m_effectZones.Remove(effectZone);
		}
		void Awake()
		{
			this.tag = TagString;
			m_effectZones = new List<AudioEffectZone>();
		}
		// Use this for initialization
		void Start()
		{
			MultiAudioProcessor.AddListener(this);
		}

		private void OnDestroy()
		{
			MultiAudioProcessor.RemoveListener(this);
		}

		private void Update()
		{
			//In effect radius?
			AudioEffectZone priorityEffect = null;
			foreach (var effect in m_effectZones)
			{
				//choose higher priority effect
				if (!priorityEffect || effect.m_priority > priorityEffect.m_priority)
				{
					priorityEffect = effect;
				}
				//Or if equal priority, use closest
				else if (!priorityEffect || effect.m_priority == priorityEffect.m_priority)
				{
					float distToCur = (transform.position - priorityEffect.transform.position).sqrMagnitude;
					float distToNew = (transform.position - effect.transform.position).sqrMagnitude;
					if (distToNew < distToCur)
					{
						priorityEffect = effect;
					}
				}
			}
			if (priorityEffect) MixerGroup = priorityEffect.m_audioMixerGroup;
			else MixerGroup = null;
		}

	}
}