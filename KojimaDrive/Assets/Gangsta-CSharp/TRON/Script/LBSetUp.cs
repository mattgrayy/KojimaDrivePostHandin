using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GCSharp
{
    public class LBSetUp : MonoBehaviour
    {
        public GameObject m_lbSpawnPointPref;
        public GameObject m_lbCollBoxPref;
        public float m_spawnTime;
        public float m_cooldownTime;
        public float m_offset;
        public int m_spawnLimit;
        private GameObject[] m_players;
        public Material[] m_lbMats;
        public GameObject m_arrowPref;

        private Bird.HUDController.hudElementToggleMultiData_t dataobject;

        [SerializeField]
        private List<GameObject> m_playerArrows;

        public int m_colourCount = 0;

        // Use this for initialization
        private void Start()
        {
            //Init();
        }

        public void Init()
        {
			
            m_players = GameObject.FindGameObjectsWithTag("Player");
            m_playerArrows = new List<GameObject>();
            PlayerSetUp();

			Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_HIDE_ALL_ELEMENTS);

			// Unhide the elements we want (this event accepts both hudElementToggleData_t and hudElementToggleMultiData_t - multi accepts an array of types)
			Bird.HUDController.hudElementToggleMultiData_t dataobject = new Bird.HUDController.hudElementToggleMultiData_t();
			dataobject.m_nPlayerID = 0; // Target player ID 0 = all players (otherwise, it's 1 - 4)
			dataobject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.ENABLE;
			dataobject.m_ArrayTypes = new System.Type[] { typeof(Bird.HUD_Timer), typeof(Bird.HUD_EXP), typeof(Bird.HUD_ScorePopupMgr), typeof(Bird.HUD_NavArrow) };

			// This will enable the exp display, timer, score popup and race position
			Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, dataobject);
			Debug.Assert(Kojima.GameModeManager.m_instance.m_currentGameMode, "Current Game Mode = null???");
			Debug.Log("Current Mode: " + Kojima.GameModeManager.m_instance.m_currentMode);
		}

        private void PlayerSetUp()
        {
            for (int i = 0; i < m_players.Length; i++)
            {
                // Set up LightBlade script
                m_players[i].AddComponent<LightBlade>();
                //m_lbCollBoxPref.GetComponent<Renderer>().material = m_lbMats[m_colourCount];
                //m_colourCount++;
                m_players[i].GetComponent<LightBlade>().SetUpScript(m_lbSpawnPointPref, m_lbCollBoxPref, m_spawnTime, m_cooldownTime, m_offset, m_spawnLimit);
                m_players[i].GetComponent<LightBlade>().SetRendMats(m_lbMats);

                //GameObject t_newArrow = (GameObject)Instantiate(m_arrowPref, m_players[i].transform.position, Quaternion.identity);
                //Vector3 t_arrowPos = new Vector3(m_players[i].transform.position.x, m_players[i].transform.position.y + 4f, m_players[i].transform.position.z);
                //t_newArrow.transform.position = t_arrowPos;
                //t_newArrow.transform.parent = m_players[i].transform;
                //m_playerArrows.Add(t_newArrow);
            }
        }

        public void GameOver()
        {
            //Cleaner();
            gameObject.GetComponent<LBMode>().EndGame();
        }

        public void Cleaner()
        {
            GameObject[] t_player = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < t_player.Length; i++)
            {
                Destroy(t_player[i].GetComponent<LightBlade>());
                //Destroy(m_playerArrows[i]);
            }
        }
    }
}