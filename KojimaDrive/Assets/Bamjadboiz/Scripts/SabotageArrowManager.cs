using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Bam
{
	public class SabotageArrowManager : MonoBehaviour
	{
		public GameObject m_prefab;
		public SabotageZoneScript m_zone;
		private Sabotage m_sabotage;
		public List<FloorArrow> m_arrows;
		private bool m_firstReset=true;
		private void Awake()
		{
			m_arrows = new List<FloorArrow>();
			m_sabotage = GameObject.Find("Sabotage").GetComponent<Sabotage>();
		}
	
		private void OnDestroy()
		{
			
		}

		void InstantiateArrows()
		{
			foreach (var player in m_sabotage.players)
			{
				FloorArrow arrow = Instantiate(m_prefab).GetComponent<FloorArrow>();
				m_arrows.Add(arrow);
			}
			ResetArrows();
		}
		void NewRoundEventHandler(object sender, EventArgs args)
		{
			if (m_firstReset)
			{
				InstantiateArrows();
				m_firstReset = false;
			}
			ResetArrows();
		}

		void ResetArrows()
		{
			int idx = 0;
			foreach (var player in m_sabotage.players)
			{
				if (idx >= m_arrows.Count) break;

				FloorArrow arrow = m_arrows[idx];
				arrow.m_car = player.myObject.GetComponent<Kojima.CarScript>();
				switch (player.myRole)
				{
					case SabotagePlayer.Role.Chaser:
						{
							arrow.m_target = m_zone.gameObject;
							break;
						}
					case SabotagePlayer.Role.Runner:
						{
							arrow.m_target = m_sabotage.players[m_sabotage.m_chaserID].myObject;
							break;
						}
				}
			}
		}

		// Update is called once per frame
		void Update()
		{
			//Should only get called once when gamemode resets but here for testing
			//ResetArrows();
		}
	}
}