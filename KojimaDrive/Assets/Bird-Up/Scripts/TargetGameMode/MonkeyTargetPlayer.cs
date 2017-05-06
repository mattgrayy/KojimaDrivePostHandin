using UnityEngine;
using System.Collections;
using System;
using Rewired;

namespace Bird
{
	[Serializable]
	public class MonkeyTargetPlayer : MonoBehaviour
	{
		public GameObject myObject;
		private Rewired.Player m_rewiredPlayer;
		public int myID;
		[SerializeField]
		bool m_died = false;
		[SerializeField]
		bool m_landing = false;
		public bool SetLanding
		{
			set
			{
				m_landing = value;
			}
		}
		public bool SetFailed
		{
			set
			{
				m_died = value;
				myObject.GetComponent<Kojima.CarScript> ().PutAwayGlider ();
				myObject.GetComponent<Kojima.CarScript>().SetCanMove (false);
			}
		}
		[SerializeField]
		bool m_landed = false;
		public bool HasLanded
		{
			get
			{
				return m_landed;
			}
		}
		public bool ReadyToEnd
		{
			get
			{
				return (m_died || m_landed);
			}
		}

		public void Initialise(GameObject gameobject, int ID, Rewired.Player _player)
		{
			myObject = gameobject;
			myID = ID;
			m_rewiredPlayer = _player;
		}

		public void Reset()
		{
			m_died = false;
			m_landing = false;
			m_landed = false;
			myObject.GetComponent<Kojima.CarScript>().SetCanMove(true);
			myObject.GetComponent<Kojima.CarScript>().ResetCar();
		}

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if (!ReadyToEnd)
			{
				if (myObject.GetComponent<Kojima.CarScript>().CurrentlyInWater)
				{
					m_died = true;
					m_landed = false;
					m_landing = false;
				}
				if ((m_landing) && (!m_died))
				{
					if (!myObject.GetComponent<Kojima.CarScript>().IsMoving)
					{
						m_landed = true;
					}
					myObject.GetComponent<Kojima.CarScript>().ApplyHandbrake();
				}
				if (m_rewiredPlayer != null)
				{
					if (m_rewiredPlayer.GetButtonDown ("Grapple"))
					{
						if (myObject.GetComponent<Kojima.CarScript> ().CurrentlyGliding)
						{
							myObject.GetComponent<Kojima.CarScript> ().PutAwayGlider ();
							myObject.transform.Find("Glider_Prefab(Clone)").gameObject.SetActive(false);
						} 
						else
						{
							myObject.GetComponent<Kojima.CarScript> ().PullOutGlider ();
							myObject.transform.Find("Glider_Prefab(Clone)").gameObject.SetActive(true);
						}
					}
				}
			}
		}
	}
}