using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Bird
{
	public class Target : MonoBehaviour {

		[SerializeField]
		private List<Color> m_tierColours;
		//private List<Kojima.CarScript> m_cars;
		[SerializeField]
		//private List<int> m_tiers;
		MonkeyTargetGameMode m_gameMode;

		public void SetMode(MonkeyTargetGameMode _mode)
		{
			m_gameMode = _mode;
		}

		// Use this for initialization
		void Start() {
			//m_cars = new List<Kojima.CarScript>();
			foreach (Transform child in transform.parent)
			{
				if (child.gameObject.GetComponent<MeshRenderer>())
				{
					m_tierColours.Add(child.gameObject.GetComponent<MeshRenderer>().material.color);
				}
			}
		}

		public int FindTier(Kojima.CarScript Car)
		{
			Vector3 TargetPos = transform.position;
			TargetPos.y = 0.0f;
			double TotalDistance = 0.0f;
			int wheelsIn = 0;
			foreach (Transform wheel in Car.GetAllWheels)
			{
				Vector3 WheelPos = wheel.position;
				WheelPos.y = 0.0f;
				TotalDistance += Vector3.Distance(TargetPos, WheelPos);
				wheelsIn++;
			}
			//Find average wheel distance
			TotalDistance = TotalDistance / wheelsIn;
			return (int)(TotalDistance / 12.7);
		}

		// Update is called once per frame
		void Update() {
			//for (int i = 0; i < m_cars.Count; i++)
			//{
			//	m_cars[i].ApplyHandbrake();
			//	m_tiers[i] = FindTier(m_cars[i]);
			//}
		}

		void OnTriggerEnter(Collider Col)
		{
			if (Col.GetComponent<Kojima.CarScript>())// && (!m_cars.Contains(Col.GetComponentInParent<Kojima.CarScript>())))
			{
				Kojima.CarScript NewCar = Col.GetComponent<Kojima.CarScript>();
				//m_cars.Add(NewCar);
				NewCar.SetCanMove(false);
				//m_tiers.Add(FindTier(NewCar));
				m_gameMode.LandCar(NewCar);
			}
		}
	}
}
