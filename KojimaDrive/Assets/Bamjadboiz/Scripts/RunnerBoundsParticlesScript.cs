using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bam
{
	public class RunnerBoundsParticlesScript : MonoBehaviour
	{
		public Sabotage m_sabotage;
		public GameObject m_effectPrefab;
		public int m_sizeOfPool;
		private List<ParticleSystem> m_pool;
		
		// Use this for initialization
		void Start()
		{
			m_pool = new List<ParticleSystem>(m_sizeOfPool);
			for (int i = 0; i < m_sizeOfPool; ++i)
			{
				m_pool.Add(Instantiate(m_effectPrefab).GetComponent<ParticleSystem>());
			}

			
		}

		private void OnCollisionEnter(Collision collision)
		{
			Kojima.CarScript car = collision.gameObject.GetComponent<Kojima.CarScript>();
			if (car != null)
			{
				foreach (var effect in m_pool)
				{
					if (!effect.isPlaying)
					{
						effect.transform.position = collision.contacts[0].point;
						effect.transform.rotation = Quaternion.LookRotation(effect.transform.position - car.transform.position);
						effect.Play();
					}

				}
			}
		}
	}
}