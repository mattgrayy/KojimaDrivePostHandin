using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace Bam
{

	public class AudioEffectZone : MonoBehaviour
	{
		public AudioMixerGroup m_audioMixerGroup;
		public int m_priority;
		public Collider m_collider;
		
	
		private void OnTriggerEnter(Collider other)
		{
			if (other.tag == MultiListener.TagString)
			{
				MultiListener listener = other.GetComponent<MultiListener>();
				if (listener) listener.AddEffectZone(this);
			}
		}
		private void OnTriggerExit(Collider other)
		{
			if (other.tag == MultiListener.TagString)
			{
				MultiListener listener = other.GetComponent<MultiListener>();
				if (listener) listener.RemoveEffectZone(this);
			}
		}
	}
}