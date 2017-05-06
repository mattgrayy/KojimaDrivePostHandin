using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kojima
{
    public class GameModeManager : MonoBehaviour
    {
        public static GameModeManager m_instance = null;

        public enum GameModeState
        {
            FREEROAM,
            EXAMPLE,
            DRIVEANDSEEK,
            VOLCANOMADNESS,
            VOLCANOMADNESS2,
            SABOTAGE,
            CARAVAN,
            CARAVANSCORE,
            RACE,
            PASSTHEBOMB,
            LIGHTBLADE,
            PARKING,
            MONKEYTARGET,
            Count
        };

        public GameModeState m_currentMode = GameModeState.FREEROAM;
        public GameModeState m_prevMode = GameModeState.FREEROAM;
        public GameModeState m_floatingMode = GameModeState.FREEROAM;

        public GameMode m_currentGameMode;
        private GameObject m_modeHolder;

        public List<string> m_startingModels;

        // Use this for initialization
        private void Start()
        {
            if (m_instance)
            {
                Destroy(this.gameObject);
            }
            else
            {
                m_instance = this;
            }

            m_modeHolder = new GameObject("Mode Holder");
            m_modeHolder.transform.SetParent(transform);
        }

        private void LateUpdate()
        {
            //Checks whether game mode has changed
            if (m_currentMode != m_floatingMode)
            {
                m_prevMode = m_floatingMode;
                m_floatingMode = m_currentMode;
                UpdateEvent();
            }
        }

        /// <summary>
        /// Handles the initialising of the new Game Mode globally
        /// </summary>
        private void UpdateEvent()
        {
            switch (m_currentMode)
            {
                case GameModeState.FREEROAM:
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.GM_FREEROAM);
						m_currentGameMode = null;
                        break;
                    }
                case GameModeState.DRIVEANDSEEK:
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.GM_DRIVEANDSEEK);
                        break;
                    }
                case GameModeState.EXAMPLE:
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.GM_EXAMPLE);
                        break;
                    }
                case GameModeState.VOLCANOMADNESS:
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.GM_VOLCANOMADNESS);
                        break;
                    }
                case GameModeState.VOLCANOMADNESS2:
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.GM_VOLCANOMADNESS2);
                        break;
                    }
                case GameModeState.SABOTAGE:
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.GM_SABOTAGE);
                        break;
                    }
                case GameModeState.CARAVAN:
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.GM_CARAVAN);
                        break;
                    }
                case GameModeState.CARAVANSCORE:
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.GM_CARAVANSCORE);
                        break;
                    }
                case GameModeState.RACE:
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.GM_RACE);
                        break;
                    }
                case GameModeState.PASSTHEBOMB:
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.GM_PASSTHEBOMB);
                        break;
                    }
                case GameModeState.PARKING:
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.GM_PARKING);
                        break;
                    }
                case GameModeState.MONKEYTARGET:
                    {
                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.GM_MONKEYTARGET);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        public void SetGameMode(GameMode _mode)
        {
            _mode.BeginGame();
            GameModeManager.m_instance.m_currentGameMode = _mode;
            GameModeManager.m_instance.m_currentMode = _mode.m_mode;
        }

        public void UnloadGameMode()
        {
            GameModeManager.m_instance.m_currentGameMode.EndGame();
        }
    }
}