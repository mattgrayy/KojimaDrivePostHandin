using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace Bam
{

	public class AudioEffectZone : MonoBehaviour
	{
		public AudioMixerGroup m_audioMixerGroup;
		public int m_priority;
		//public float m_radius;
		public Collider m_collider;
		

		// Use this for initialization
		void Start()
		{
			Debug.Assert(m_collider, "Audio effects must have a collider componenet");
		}

	
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