using UnityEngine;
using System.Collections;
using System;

namespace Bam
{

	public class RunnerBoundsScript : MonoBehaviour
	{
		public Sabotage				m_sabotage = null;
		public SabotageZoneScript	m_zone = null;
		public Collider				m_collider { get; private set; }

		// Use this for initialization
		void Start()
		{
			YamsDebug.AssertObjectNotNull(this, m_sabotage);
			m_sabotage.RoundStartEvent += RoundStartEventHandler;
			m_collider = GetComponent<Collider>();
		}

		private void OnDestroy()
		{
			m_sabotage.RoundStartEvent -= RoundStartEventHandler;
		}

		void RoundStartEventHandler(object sender, EventArgs args)
		{
			foreach (var player in m_sabotage.players)
			{
				bool ignoreColliders = false;
				if (player.myRole == SabotagePlayer.Role.Chaser)
				{
					ignoreColliders = true;
				}
				IgnoreCollisionWithCar(player, m_collider, ignoreColliders);
			}
			//@@testing a thing
			m_collider.enabled = false;
		}

		void IgnoreCollisionWithCar(SabotagePlayer player, Collider myCollider, bool ignore)
		{
			Collider[] carColliders = player.myObject.GetComponents<Collider>();
			Collider[] carChildColliders = player.myObject.GetComponentsInChildren<Collider>();

			foreach (var c in carColliders)
			{
				Physics.IgnoreCollision(c, myCollider, ignore);
			}

			foreach (var c in carChildColliders)
			{
				Physics.IgnoreCollision(c, myCollider, ignore);
			}
		}
		
		// Update is called once per frame
		void Update()
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, Vector3.down, out hit, m_zone.m_offsetY * 2, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
			{
				transform.position = hit.point;
			}

			Vector3 scale  = transform.localScale;
			float scaleValue = m_zone.m_radius * 2;
			scale.x = scaleValue;
			scale.z = scaleValue;
			transform.localScale = scale;

			///@@@Testing a thing
			if (!m_sabotage.gamemodeRunning) return;
			foreach (var player in m_sabotage.players)
			{
				if (true || player.myRole == SabotagePlayer.Role.Runner)
				{
					Vector2 carPosition;
					carPosition.x = player.myObject.transform.position.x;
					carPosition.y = player.myObject.transform.position.z;
					Vector2 zonePos;
					zonePos.x = transform.position.x;
					zonePos.y = transform.position.z;
				
					if (Vector2.Distance(carPosition,zonePos) >= m_zone.m_radius)
					{
						Vector3 direction = (transform.position - player.myObject.transform.position).normalized;
						const float FORCE = 4000;
						player.myObject.GetComponent<Rigidbody>().AddForce(direction * FORCE, ForceMode.Impulse);
					}
				}
			}
		}
	}
}