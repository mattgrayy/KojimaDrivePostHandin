using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Kojima
{
    public class RaceGameMode : GameMode
    {
        private RaceScript rs;
        private bool loadScene = false;
        new
            void Start()
        {
            base.Start();
            EventManager.m_instance.SubscribeToEvent(Events.Event.GM_RACE, RaceSetup);

        }

        void OnDestroy()
        {
            EventManager.m_instance.UnsubscribeToEvent(Events.Event.GM_RACE, RaceSetup);
        }

        void RaceSetup()
        {
            SceneManager.LoadScene("Race1Additive", LoadSceneMode.Additive);
        }

        new
        void Update()
        {
            base.Update();
            if (loadScene)
            {
                //SceneManager.LoadScene("Race1Additive", LoadSceneMode.Additive);
                loadScene = false;
            }


            //Game Mode Loop
            if (m_active)
            {
                Debug.Log("Race Loaded");




                //Game Modes are required to have an exit point

            }
        }

        /// <summary>
        /// Handles game logic when the game ends
        /// </summary>
        new
        public void EndGame()
        {
            SceneManager.UnloadScene("Race1Additive");

            base.EndGame();
        }
    }
}

