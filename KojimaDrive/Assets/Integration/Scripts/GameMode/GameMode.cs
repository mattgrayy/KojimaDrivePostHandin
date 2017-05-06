using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Kojima
{
    public class GameMode : MonoBehaviour
    {
        [Serializable]
        public struct SpawnPoint
        {
            public string m_name;
            public Transform m_transform;
            public bool m_inUse;

            public string Name() { return m_name; }
            public void Name(string _name) { m_name = _name; }

            public Transform Transform() { return m_transform; }
            public void Transform(Transform _transform) { m_transform = _transform; }

            public bool InUse() { return m_inUse; }
            public void InUse(bool _inUse) { m_inUse = _inUse; }
        }

        [Serializable]
        public struct Phase
        {
            public string m_phaseName;
            public string m_nextPhase;
            [HideInInspector]
            public string m_previousPhase;

            public float m_length;
            public float m_delay;
            public bool m_minutes;
            public string m_message;

            [HideInInspector]
            public GameObject m_timerObject;
            public Timer m_timer;

            public void SetupPhase(GameObject _timerObject, string _previousPhase = "NULL")
            {
                m_previousPhase = _previousPhase;

                m_timerObject = Instantiate(_timerObject);
                m_timer = m_timerObject.GetComponent<Timer>();
                m_timer.SetTimer(m_phaseName, m_minutes, m_length, m_delay);
            }

            public void ResetPhase()
            {
                m_timer.GetComponent<Timer>().ResetTimer();
            }

            public void BeginPhase()
            {
                m_timer.StartTimer();
            }

            public void PausePhase()
            {
                m_timer.PauseTimer();
            }

            public void UnpausePhase()
            {
                m_timer.UnpauseTimer();
            }
        }

        [Serializable]
        public struct Team
        {
            public string m_teamName;
            public List<int> m_teamMembers;
        }

        protected bool m_active = false;

        public float m_modeWidth;
        public float m_modeHeight;
        private Vector3 m_modeLocation;
        private Rect m_modeRect;

        public GameModeManager.GameModeState m_mode;

        [HideInInspector]
        public int m_numberOfPlayers;

        [HideInInspector]
        public List<int> m_playerScores;
        public int m_winningScore = 3;
        protected int m_gameWinner = -1;

        public bool m_resetModelsOnEnd = true;
        [HideInInspector]
        public List<string> m_startingModels;
        public bool m_lockCarChanges = false;

        public List<Team> m_teams;

        public List<SpawnPoint> m_spawns;

        public List<Phase> m_phases;
        [HideInInspector]
        public Phase m_currentPhase;
        
        public GameObject m_timerObject;
        protected Transform m_timerHolder;

		public Bird.BaseTransition m_CustomLoadTransition;
		public bool m_bDisplayTextIntro = true;
		public string m_strEventName;
		public string m_strEventByline;

        protected void Start()
        {
			if(m_CustomLoadTransition != null) {
				m_CustomLoadTransition.StartTransition();
			} else {
				// Do a default transition
				//Bird.DefaultTransitions.Event_DefaultTransitionIn();
			    Kojima.EventManager.m_instance.AddEvent(Events.Event.UI_TRANS_DEFAULT_IN);
			}

			if(m_bDisplayTextIntro) {
				Bird.HUD_EventStart.hudEventStartData_t data = new Bird.HUD_EventStart.hudEventStartData_t();
				data.m_strEventName = m_strEventName;
				data.m_strEventDesc = m_strEventByline;
				Kojima.EventManager.m_instance.AddEvent(Events.Event.UI_HUD_SHOW_EVENTSTART, data);
			}

            InitialiseEvent();
            GetNumberOfPlayers();

            m_timerHolder = new GameObject("TimerHolder").transform;
            m_timerHolder.SetParent(this.transform);

            if (m_phases.Count != 0)
            {
                m_currentPhase = GetPhase("Inactive");

                Phase dynamicPhase = new Phase();
                dynamicPhase.m_phaseName = "Dynamic";
                m_phases.Add(dynamicPhase);
            }

            for (int iter = 0; iter <= m_phases.Count - 1; iter++)
            {
                Phase tempPhase = m_phases[iter];
                tempPhase.m_timerObject = Instantiate(m_timerObject);
                tempPhase.m_timerObject.transform.SetParent(m_timerHolder);
                tempPhase.m_timer = tempPhase.m_timerObject.GetComponent<Timer>();
                tempPhase.m_timer.SetTimer(tempPhase.m_phaseName, tempPhase.m_minutes, tempPhase.m_length, tempPhase.m_delay);
                m_phases[iter] = tempPhase;
            }
        }

        protected void Update()
        {

        }

        /// <summary>
        /// Returns whether the game mode is active
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return m_active;
        }

        /// <summary>
        /// Sets game mode active state
        /// </summary>
        public void SetActive(bool _active)
        {
            m_active = _active;
        }

        /// <summary>
        /// Returns the world position of the game mode
        /// </summary>
        public Vector3 GetEventPosition()
        {
            return m_modeLocation;
        }

        /// <summary>
        /// Returns the Rect the game mode encompasses
        /// </summary>
        /// <returns></returns>
        public Rect GetEventRect()
        {
            return m_modeRect;
        }

        /// <summary>
        /// Sets game modes positon and game mode Rect
        /// </summary>
        public void InitialiseEvent()
        {
            m_modeLocation = transform.position;
            Vector2 modeMinimum = new Vector2(m_modeLocation.x - m_modeWidth / 2, m_modeLocation.z - m_modeHeight / 2);
            m_modeRect = new Rect(modeMinimum, new Vector2(m_modeWidth, m_modeHeight));

            for (int iter = 1; iter <= 4; iter++)
            {
                m_playerScores.Add(0);
            }
        }

        public void ResetEvent()
        {
            m_playerScores.Clear();
            m_startingModels.Clear();
        }

        public Phase GetPhase(string _name)
        {
            Phase tempPhase = new Phase();
            tempPhase.m_phaseName = "NULL";

            foreach(Phase phase in m_phases)
            {
                if(phase.m_phaseName == _name)
                {
                    tempPhase = phase;
                }
            }

            if (tempPhase.m_phaseName == "NULL")
            {
                Debug.Log("Phase does not exist!");
            }

            return tempPhase;
        }

        /// <summary>
        /// Resets all pre-made timers
        /// </summary>
        protected void ResetAllPhases()
        {
            foreach (Phase phase in m_phases)
            {
                phase.ResetPhase();
            }
        }

        protected void TransistionToNextPhase()
        {
            string prevPhase = "NULL";

            if (m_currentPhase.m_previousPhase != "NULL")
            {
                prevPhase = m_currentPhase.m_phaseName;
            }

            if (m_currentPhase.m_nextPhase != "NULL")
            {
                m_currentPhase = GetPhase(m_currentPhase.m_nextPhase);
                m_currentPhase.m_previousPhase = prevPhase;
            }
            else
            {
                Debug.Log("No next Phase!");
            }
        }

        protected void TransistionToPreviousPhase()
        {
            if (m_currentPhase.m_previousPhase != "NULL")
            {
                m_currentPhase = GetPhase(m_currentPhase.m_previousPhase);
            }
            else
            {
                Debug.Log("No previous Phase!");
            }
        }

        protected void AddMemberToTeam(int _playerIndex, string _teamName)
        {
            foreach(Team team in m_teams)
            {
                if(team.m_teamName == _teamName)
                {
                    if (!MemberOnTeam(_playerIndex, team))
                    {
                        team.m_teamMembers.Add(_playerIndex);
                    }
                }
            }
        }

        protected void RemoveMemberFromTeam(int _playerIndex, string _teamName)
        {
            foreach (Team team in m_teams)
            {
                if (team.m_teamName == _teamName)
                {
                    if (MemberOnTeam(_playerIndex, team))
                    {
                        team.m_teamMembers.Remove(_playerIndex);
                    }
                }
            }
        }

        protected bool MemberOnTeam(int _playerIndex, Team _team)
        {
            bool result = false;

            foreach(int member in _team.m_teamMembers)
            {
                if(member == _playerIndex)
                {
                    result = true;
                }
            }

            return result;
        }

        protected SpawnPoint GetSpawn(int _index)
        {
            return m_spawns[_index];
        }

        protected SpawnPoint GetSpawn(string _name)
        {
            for (int iter = 0; iter <= (m_spawns.Count - 1); iter++)
            {
                if (m_spawns[iter].Name() == _name)
                {
                    return m_spawns[iter];
                }
            }
            return m_spawns[0];
        }

        protected SpawnPoint GetNextSpawn(bool _setUsed)
        {
            for (int iter = 0; iter <= m_spawns.Count - 1; iter++)
            {
                if (!m_spawns[iter].m_inUse)
                {
                    SpawnPoint tempSpawn = m_spawns[iter];
                    tempSpawn.m_inUse = true;
                    m_spawns[iter] = tempSpawn;
                    return GetSpawn(iter);
                }
            }
            return GetSpawn(0);
        }

        protected void ResetSpawns()
        {
            for (int iter = 0; iter <= m_spawns.Count - 1; iter++)
            {
                SpawnPoint tempSpawn = m_spawns[iter];
                tempSpawn.m_inUse = false;
                m_spawns[iter] = tempSpawn;
            }
        }

        protected void SpawnNextSpawn(GameObject _car)
        {
            SpawnPoint spawn = GetNextSpawn(true);
            _car.transform.position = spawn.m_transform.position;
            _car.transform.rotation = spawn.m_transform.rotation;
        }

        protected void SpawnByName(GameObject _car, string _name)
        {
            SpawnPoint spawn = GetSpawn(_name);
            _car.transform.position = spawn.m_transform.position;
            _car.transform.rotation = spawn.m_transform.rotation;
        }

        protected void AddScore(int _playerIndex, int _score)
        {
            m_playerScores[_playerIndex] += _score;
        }

        protected void SetScore(int _playerIndex, int _score)
        {
            m_playerScores[_playerIndex] = _score;
        }

        protected bool CheckWinner(int _playerIndex)
        {
            if (m_playerScores[_playerIndex] == m_winningScore)
            {
                m_gameWinner = _playerIndex;
                return true;
            }

            return false;
        }

        protected bool CheckWinnerAll()
        {
            for (int iter = 0; iter <= m_playerScores.Count - 1; iter++)
            {
                if (m_playerScores[iter] == m_winningScore)
                {
                    m_gameWinner = iter;
                    return true;
                }
            }
            return false;
        }

        protected void ResetScores()
        {
            for (int iter = 0; iter <= m_playerScores.Count - 1; iter++)
            {
                m_playerScores[iter] = 0;
            }
        }

        protected void ChangePlayerMovement(int _index, bool _state)
        {
            Kojima.GameController.s_singleton.m_players[_index].SetCanMove(_state);
        }

        protected void ChangeAllPlayerMovement(bool _state)
        {
            for (int iter = 0; iter <= m_numberOfPlayers - 1; iter++)
            {
                Kojima.GameController.s_singleton.m_players[iter].SetCanMove(_state);
            }
        }

        protected int GetNumberOfPlayers()
        {
            for (int iter = 0; iter <= Kojima.GameController.s_singleton.m_players.Length - 1; iter++)
            {
                if (Kojima.GameController.s_singleton.m_players[iter])
                {
                    m_numberOfPlayers++;
                }
            }

            return m_numberOfPlayers;
        }

        public void BeginGame()
        {
            //Sets game to active
            m_active = true;

            for (int iter = 0; iter <= Kojima.GameController.s_singleton.m_players.Length - 1; iter++)
            {
                if (Kojima.GameController.s_singleton.m_players[iter])
                {
                    m_startingModels.Add(Kojima.GameController.s_singleton.m_players[iter].gameObject.GetComponent<CarData>().m_name);
                }
            }
        }

        /// <summary>
        /// Handles game logic when the game ends
        /// </summary>
        public void EndGame()
        {
            //Sets game to inactive
            m_active = false;

            if (m_resetModelsOnEnd)
            {
                for (int iter = 0; iter <= m_numberOfPlayers - 1; iter++)
                {
                    if (m_startingModels.Count != 0)
                    {
                        Kojima.CarSwapManager.m_sInstance.ChangeCar(iter, Kojima.GameController.s_singleton.m_players[iter].m_nControllerID, m_startingModels[iter]);
                    }
                    else
                    {
                        Kojima.CarSwapManager.m_sInstance.ChangeCar(iter, Kojima.GameController.s_singleton.m_players[iter].m_nControllerID, "Manta");
                    }
                }
            }

            ResetEvent();

            //Sets the game manager back to freeroam to initalise the game mode's deactivation globaly
            Kojima.GameModeManager.m_instance.m_currentMode = Kojima.GameModeManager.GameModeState.FREEROAM;

			// Resets the gamemode specific score
			HF.PlayerExp.ResetCurrentEXP();

            Bam.LobbyManagerScript.singleton.ReturnToLobby();
        }

        protected bool CheckAllCarLocked()
        {
            for (int iter = 0; iter <= m_numberOfPlayers - 1; iter++)
            {
                if (!Kojima.GameController.s_singleton.m_players[iter].gameObject.GetComponent<SwapCarOnPress>().CarLocked())
                {
                    return false;
                }
            }
            return true;
        }

        protected void SetAllCarsLocks(bool _state)
        {
            for (int iter = 0; iter <= m_numberOfPlayers - 1; iter++)
            {
                Kojima.GameController.s_singleton.m_players[iter].gameObject.GetComponent<SwapCarOnPress>().SetCarLock(_state);
            }
        }

        public string GetTime()
        {
            return m_currentPhase.m_timer.m_sText;
        }

		// Added these for timer colour lerping
		public float GetPhaseLength() {
			return m_currentPhase.m_timer.m_fTimerLength;
		}

		public float GetTimeFloat() {
			return m_currentPhase.m_timer.m_fRemainingTimeSeconds;
		}

		public float GetStartTime() {
			return m_currentPhase.m_timer.m_fStartTime;
		}

        public void AddCurrentTimer(string _name, float _timerLength, float _delay = 0.0f, bool _minutes = false)
        {
            m_currentPhase.m_timer = new Timer();
            m_currentPhase.m_timer.m_sName = _name;
            m_currentPhase.m_timer.m_fTimerLength = _timerLength;
            m_currentPhase.m_timer.m_fDelayLength = _delay;
            m_currentPhase.m_timer.m_bMinutes = _minutes;
        }

        public void RemoveCurrentTimer()
        {
            Destroy(m_currentPhase.m_timer);
        }
    }
}