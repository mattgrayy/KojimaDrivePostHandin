using UnityEngine;
using Rewired;
using System.Collections;


namespace Kojima
{
    public class GameModeTrigger : MonoBehaviour
    {
        //Rewired.Player m_rewiredPlayer;

        //private void OnTriggerStay(Collider other)
        //{
        //    if (!GetComponent<GameMode>().IsActive())
        //    {
        //        if (other.gameObject.GetComponent<CarScript>())
        //        {
        //            switch (other.gameObject.GetComponent<CarScript>().m_nplayerIndex)
        //            {
        //                case 1:
        //                    m_rewiredPlayer = ReInput.players.GetPlayer(0);
        //                    if (m_rewiredPlayer.GetButton("Action"))
        //                    {
        //                        GameModeManager.m_instance.SetGameMode(GetComponent<GameMode>());
        //                        //SetEvent("Player" + 1);
        //                    }
        //                    break;
        //                case 2:
        //                    m_rewiredPlayer = ReInput.players.GetPlayer(1);
        //                    if (m_rewiredPlayer.GetButton("Action"))
        //                    {
        //                        GameModeManager.m_instance.SetGameMode(GetComponent<GameMode>());
        //                        //SetEvent("Player" + 2);
        //                    }
        //                    break;
        //                case 3:
        //                    m_rewiredPlayer = ReInput.players.GetPlayer(2);
        //                    if (m_rewiredPlayer.GetButton("Action"))
        //                    {
        //                        GameModeManager.m_instance.SetGameMode(GetComponent<GameMode>());
        //                        //SetEvent("Player" + 3);
        //                    }
        //                    break;
        //                case 4:
        //                    m_rewiredPlayer = ReInput.players.GetPlayer(3);
        //                    if (m_rewiredPlayer.GetButton("Action"))
        //                    {
        //                        GameModeManager.m_instance.SetGameMode(GetComponent<GameMode>());
        //                        //SetEvent("Player" + 4);
        //                    }
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //    }
        //}

        private void Start()
        {
            SetEvent();
        }

        /// <summary>
        /// Activates the game mode attached to the same object
        /// </summary>
        public void SetEvent()
        { 
            GameModeManager.m_instance.SetGameMode(GetComponent<GameMode>());
        }
    }
}

