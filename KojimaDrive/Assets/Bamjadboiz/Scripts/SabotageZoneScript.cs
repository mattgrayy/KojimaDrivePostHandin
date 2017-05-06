//===================== Kojima Drive - Bamjadboiz 2017 ====================//
//
// Author:		Orlando 
// Purpose:		Componenet that manages the sabotage zone which the chaser must stay within.
// Namespace:	Bam
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bam
{
	public class SabotageZoneScript : MonoBehaviour
	{
		public Color	m_colourSafe;
		public Color	m_colourUnsafe;
		public float	m_speedMove;
		public float	m_speedGrow;
		public float	m_radiusBonusMin;
		public float	m_radiusBonusMax;
		public float	m_radiusBonusDistMultiplier;
		public float	m_offsetY;
		public float	m_radiusMax;
		public bool		m_chaserInZone { private set; get; }
		public Vector3	m_groundPos { get; private set; }

		private Sabotage		m_sabotage;
		private Projector		m_projector;
		public float			m_radius { get; private set; }

		void Awake()
		{
			m_projector = GetComponent<Projector>();

			Vector3 pos = transform.position;
			pos.y = m_offsetY;
			transform.position = pos;

			m_chaserInZone = false;
		}

		void Start()
		{
			m_sabotage = GameObject.Find("Sabotage").GetComponent<Sabotage>();
		}

		void Update()
		{
			/* Early out if gamemode is not running */
			if (!m_sabotage.gamemodeRunning) return;

			UpdateRadius();
			UpdatePosition();
			UpdateStatus();
			UpdateColour();
		}

		void UpdateRadius()
		{
			float maxDist = 0;
			float dist = 0;
			Vector3 pos0;
			Vector3 pos1;
			//get furthest distance between cars
			foreach (var p0 in m_sabotage.players)
			{
				if (p0.myRole == SabotagePlayer.Role.Chaser || p0.myObject==null) continue;
				pos0 = p0.myObject.transform.position;
				pos0.y = 0;
				foreach (var p1 in m_sabotage.players)
				{
					if (p1.myObject != p0.myObject)
					{
						if (p1.myRole == SabotagePlayer.Role.Chaser) continue;
						pos1 = p1.myObject.transform.position;
						pos1.y = 0;
						dist = (pos1 - pos0).sqrMagnitude;
						if (dist > maxDist) maxDist = dist;
					}
				}
			}
			//Was using sqr magnitude for multiple distnace checks, so sqrt max val now
			maxDist = Mathf.Sqrt(maxDist);

			/* Add bonus based on the car dists */
			float bonus = Mathf.Clamp(maxDist * m_radiusBonusDistMultiplier, m_radiusBonusMin, m_radiusBonusMax);
			maxDist += bonus;

			m_radius = Mathf.Lerp(m_radius, maxDist, m_speedGrow * Time.deltaTime);

			if (m_radius > m_radiusMax) m_radius = m_radiusMax;

			m_projector.orthographicSize = m_radius;
		}

		void UpdateStatus()
		{
            
			SabotagePlayer chaser = m_sabotage.players[m_sabotage.m_chaserID];

            if (chaser.myObject)
            {
                Vector3 chaserPos = chaser.myObject.transform.position;
                Vector3 pos = transform.position;
                chaserPos.y = 0;
                pos.y = 0;

                m_chaserInZone = Vector3.Distance(chaserPos, pos) < m_radius;
            }
		}

		void UpdateColour()
		{	
			if (m_chaserInZone)
			{
				m_projector.material.color = m_colourSafe;
			}
			else
			{
				m_projector.material.color = m_colourUnsafe;
			}
		}

		void UpdatePosition()
		{
			/* Work out where the zone should be */
			Vector3 target = Vector3.zero;
			foreach (var player in m_sabotage.players)
			{
				if (player.myRole == SabotagePlayer.Role.Runner && player.myObject)
				{
					target += player.myObject.transform.position;
				}
			}
			//get average (one less than count because of chaser)
			target /= m_sabotage.players.Count - 1;
			target = Vector3.Lerp(transform.position, target, m_speedMove * Time.deltaTime);

			RaycastHit hit;
			if (Physics.Raycast(transform.position, Vector3.down, out hit, m_offsetY * 2, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
			{
				m_groundPos = hit.point;
			}


			target.y = m_groundPos.y + m_offsetY;
			transform.position = target;
		}
	}
}