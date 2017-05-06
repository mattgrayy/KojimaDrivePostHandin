using UnityEngine;
using System.Collections;

namespace Bam
{
	public class ZoneParticlesScript : MonoBehaviour
	{
		private ParticleSystem m_particles;
		private SabotageZoneScript m_zone;
		// Use this for initialization
		void Awake()
		{
			m_zone = GetComponentInParent<SabotageZoneScript>();
			Debug.Assert(m_zone != null, "[Yams] ZoneParticleScript requires a SabotageZoneScript on parent object");

			m_particles = GetComponent<ParticleSystem>();
			Debug.Assert(m_particles != null, "[Yams] SabotageZoneScript requires a particle system.");
		}

		// Update is called once per frame
		void Update()
		{
			UpdateRadius();
			UpdateTransform();
		}

		void UpdateRadius()
		{
			ParticleSystem.ShapeModule shape = m_particles.shape;
			shape.radius = 10;

            transform.localScale = m_zone.m_radius * 0.1f * Vector3.one;
		}

		void UpdateTransform()
		{
			RaycastHit hit;
			Debug.DrawLine(transform.position, transform.position + Vector3.down * m_zone.m_offsetY * 2);
			if (Physics.Raycast(transform.parent.position + Vector3.up, Vector3.down, out hit, m_zone.m_offsetY * 200, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
			{
				m_particles.transform.position = hit.point;
			}

		}
	}
}