using UnityEngine;
using System.Collections;
using System;

namespace Bam
{
	public class CarZoneScript : MonoBehaviour
	{
		/// <summary>
		/// Set this manually! (Done in Sabotage.cs when the script is added to the car)
		/// </summary>
		public SabotageZoneScript m_zone = null;
		/// <summary>
		/// Force to apply to the car in the direction of the safezone when it leaves
		/// </summary>
		public float m_force = 4000;
		/// <summary>
		/// Offset from zone.m_groundPos to jump to
		/// </summary>
		public Vector3 m_forceTargetOffset = new Vector3(0, 7.0f, 0);
		/// <summary>
		/// Force mode... Impulse works well!
		/// </summary>
		public ForceMode m_forceMode = ForceMode.Impulse;
		/// <summary>
		/// Should the car be pulled back when outside of the circle?
		/// This is set in Sabotage.cs when the script is added
		/// </summary>
		public bool m_pullbackAffectsThisCar;
		/// <summary>
		/// ZoneStatus struct has some info about whether the car is inside the zone or not (see below)
		/// </summary>
		public ZoneStatus m_zoneStatus { get; private set; }
		public struct ZoneStatus
		{
			/// <summary>
			/// Car currently in the safe zone?
			/// </summary>
			public bool m_inZone;
			/// <summary>
			/// Car in the safe zone in previous frame?
			/// </summary>
			public bool m_inZonePreviousFrame;
			/// <summary>
			/// Did the car exit or enter the safe zone this frame?
			/// </summary>
			public bool m_zoneStatusChangedThisFrame { get { return m_inZone != m_inZonePreviousFrame; } }
			public ZoneStatus(bool inZone, bool inZonePreviousFrame)
			{
				m_inZone = inZone;
				m_inZonePreviousFrame = inZonePreviousFrame;
			}
		}

		private Rigidbody m_rigidBody;
		// Use this for initialization
		void Start()
		{
			m_zoneStatus = new ZoneStatus(true, true);

			m_rigidBody = GetComponent<Rigidbody>();
			YamsDebug.AssertObjectNotNull(this, m_rigidBody);
		}
	
		void Update()
		{
			Vector2 carPosition;
			carPosition.x =	transform.position.x;
			carPosition.y = transform.position.z;
			Vector2 zonePos = new Vector2(m_zone.m_groundPos.x, m_zone.m_groundPos.z);

			bool inZonePreviousFrame = m_zoneStatus.m_inZonePreviousFrame;
			bool inZone = true;
			if (Vector2.Distance(carPosition, zonePos) >= m_zone.m_radius)
			{
				//Debug.Log("not in zone");
				inZone = false;
				if (m_pullbackAffectsThisCar)
				{
					//Debug.Log("applyig force");
					Vector3 direction = ((m_zone.m_groundPos + m_forceTargetOffset) - transform.position).normalized;
					m_rigidBody.AddForce(direction * m_force, m_forceMode);
				}
			}
			m_zoneStatus = new ZoneStatus(inZone, inZonePreviousFrame);
		}
	}
}