using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;

namespace Bird
{
	public class MonkeyTargetGameMode:Kojima.GameMode
	{

		public enum GameState
		{
			SETUP = 0,
			PLAYING,
			ENDROUND,
			ENDGAME
		}
		public GameState m_currentState;

		public BaseTransition m_transition;

		public int m_numberOfRounds;
		[SerializeField]
		private int m_currentRound = 0;

		public GameObject m_targetPrefab;
		[SerializeField]
		private Target m_target;
		public Transform[] m_targetSpawnPoints;

		public GameObject m_spawnPlanePrefab;
		[SerializeField]
		private SpawnPlane m_spawnPlane;
		public Transform[] m_planeSpawnPoints;

		public GameObject m_playerPrefab;
		[SerializeField]
		public List<MonkeyTargetPlayer> m_players;
		public List<GameObject> m_playerObjects;
		private int[] m_scores;

		public float m_roundEndTime;
		private float m_roundEndTimer;

		protected void NewRound()
		{
			m_spawnPlane.gameObject.transform.position = m_planeSpawnPoints[m_currentRound].position;
			m_spawnPlane.gameObject.transform.rotation = m_planeSpawnPoints[m_currentRound].rotation;
			m_target.transform.root.gameObject.transform.position = m_targetSpawnPoints[m_currentRound].position;
			m_target.transform.root.gameObject.transform.rotation = m_targetSpawnPoints[m_currentRound].rotation;
			int i = 0;
			foreach (MonkeyTargetPlayer player in m_players)
			{
				player.myObject.transform.position = m_spawnPlane.GetCarSpawn(i).position;
				player.myObject.transform.rotation = m_spawnPlane.GetCarSpawn(i).rotation;
				player.Reset();
				i++;
			}
			m_currentState = GameState.PLAYING;
		}

		public void LandCar(Kojima.CarScript _car)
		{
			m_players[Array.IndexOf(Kojima.GameController.s_singleton.m_players, _car)].SetLanding = true;
		}

		new
		void Start()
		{
			base.Start();
			m_active = true;
			m_currentState = GameState.SETUP;
			GameObject Target = Instantiate(m_targetPrefab);
			m_target = Target.GetComponentInChildren<Target>();
			m_target.SetMode(this);
			GameObject Plane = Instantiate(m_spawnPlanePrefab);
			m_spawnPlane = Plane.GetComponent<SpawnPlane>();

			m_players = new List<MonkeyTargetPlayer>();
			m_playerObjects = new List<GameObject>();
			m_scores = new int[4];
			int i = 0;
			foreach (Kojima.CarScript player in Kojima.GameController.s_singleton.m_players)
			{
				if (player != null)
				{
					m_playerObjects.Add(player.gameObject);
					GameObject NewPlayer = Instantiate(m_playerPrefab);
					NewPlayer.GetComponent<MonkeyTargetPlayer>().Initialise(player.gameObject, i, player.GetRewiredPlayer());
					m_players.Add(NewPlayer.GetComponent<MonkeyTargetPlayer>());
					i++;
				}
			}
			m_transition.StartTransition ();
			//NewRound();
		}

		new
		void Update()
		{
			base.Update();

			if (m_active)
			{
				//if (FirstSetup)
				//{
				//	FirstSetup = false;
				//	NewRound();
				//}
				switch (m_currentState)
				{
					case GameState.SETUP:
						NewRound();
						break;
					case GameState.PLAYING:
						GamePlay();
						break;
					case GameState.ENDROUND:
						MidRound();
						break;
					case GameState.ENDGAME:
						EndGame();
						break;

				}
			}
		}

		void GamePlay()
		{
			bool RemainActive = false;
			foreach (MonkeyTargetPlayer player in m_players)
			{
				if (player.myObject.transform.position.y < m_target.transform.position.y - 3.0f)
				{
					player.SetFailed = true;
				}
				if (!player.ReadyToEnd)
				{
					RemainActive = true;
					break;
				}
			}
			if (!RemainActive)
			{
				EndRound();
				//end round
			}
		}

		void MidRound()
		{
			m_roundEndTimer += Time.deltaTime;
			if (m_roundEndTimer > m_roundEndTime)
			{
				m_currentRound++;
				if ((m_currentRound >= m_numberOfRounds) || (m_currentRound > m_targetSpawnPoints.Length))
				{
					m_currentState = GameState.ENDGAME;
				}
				else
				{
					NewRound();
				}
				m_roundEndTimer = 0.0f;
			}
		}

		void EndRound()
		{
			foreach (MonkeyTargetPlayer player in m_players)
			{
				if (player.HasLanded)
				{
					//m_scores[player.myID] += m_target.FindTier(player.myObject.GetComponent<Kojima.CarScript>());
					int Tier = m_target.FindTier(player.myObject.GetComponent<Kojima.CarScript>());
					HF.PlayerExp.AddEXP(player.myID, 50 * (7 - Tier), true, true, "Landed in tier " + Tier.ToString(), true);
				}
			}
			m_currentState = GameState.ENDROUND;
			m_transition.StartTransition();
			/*TO DO: ADD POINTS TO XP*/
		}

		new
		void EndGame()
		{
			Destroy(m_target.transform.parent.gameObject);
			Destroy(m_spawnPlane.gameObject);
			foreach (MonkeyTargetPlayer player in m_players)
			{
				//player.myObject.GetComponent<Kojima.CarScript>().EnterWater();
				player.Reset();
				Destroy(player.gameObject);
			}
			m_active = false;
			//Kojima.GameModeManager.m_instance.m_currentMode = Kojima.GameModeManager.GameModeState.FREEROAM;
			base.EndGame();
			//Application.UnloadLevel();
		}
	}
}
