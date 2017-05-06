using UnityEngine;
using System.Collections;

namespace HF
{
    //===================== Kojima Drive - Half-Full 2017 ====================//
    //
    // Author: ADAM MOOREY
    // Purpose: Acts as main controller script, adding and removing variables/scripts dependent on player
    // Namespace: HALF-FULL
    //
    //===============================================================================//

    public class DriveAndSeek : MonoBehaviour
    {
        //key player identifier
        public int m_iPlayerNumber;
        private int m_runnerNum;

        //detection variables
        public bool m_bRunner = false;
        public bool m_bChaser = false;
        public bool m_bDead = false;

        void Start()
        {
			m_iPlayerNumber = gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex;

            Bird.HUDController.hudElementToggleData_t arrowObject = new Bird.HUDController.hudElementToggleData_t();
            arrowObject.m_nPlayerID = gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex;
            arrowObject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.ENABLE;
            arrowObject.m_Type = typeof(Bird.HUD_Timer);

            Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, arrowObject);

            Bird.HUDController.hudElementToggleData_t xpObject = new Bird.HUDController.hudElementToggleData_t();
            xpObject.m_nPlayerID = gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex;
            xpObject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.ENABLE;
            xpObject.m_Type = typeof(Bird.HUD_EXP);

            Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, xpObject);
        }

        void Update()
        {
            if (m_bDead)
            {
                Kojima.GameController.s_singleton.m_players[m_iPlayerNumber - 1].SetCanMove(!m_bDead);
            }
        }

        private void OnDestroy()
        {
            Bird.HUDController.hudElementToggleData_t arrowObject = new Bird.HUDController.hudElementToggleData_t();
            arrowObject.m_nPlayerID = gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex;
            arrowObject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.DISABLE;
            arrowObject.m_Type = typeof(Bird.HUD_Timer);

            Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, arrowObject);

            Bird.HUDController.hudElementToggleData_t xpObject = new Bird.HUDController.hudElementToggleData_t();
            xpObject.m_nPlayerID = gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex;
            xpObject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.DISABLE;
            xpObject.m_Type = typeof(Bird.HUD_EXP);

            Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, xpObject);
        }

        //add the scripts related to the runner
        public void SetRunner()
        {
            m_bRunner = true;
            gameObject.AddComponent<Health>();
            gameObject.AddComponent<WaypointSpawner>();

			Bird.HUDController.hudElementToggleData_t arrowObject = new Bird.HUDController.hudElementToggleData_t();
			arrowObject.m_nPlayerID = gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex;
			arrowObject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.DISABLE;
			arrowObject.m_Type = typeof(Bird.HUD_NavArrow);

            Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, arrowObject);
        }

        //add the scripts related to the chaser
        public void SetChaser(string _hiderTag, int _runnerNum)
        {
            m_bChaser = true;
            gameObject.AddComponent<Health>();
            gameObject.AddComponent<Chaser>();
            gameObject.GetComponent<Chaser>().m_sHiderTag = _hiderTag;

            Bird.HUDController.hudElementToggleData_t arrowObject = new Bird.HUDController.hudElementToggleData_t();
            arrowObject.m_nPlayerID = gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex;
            arrowObject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.ENABLE;
            arrowObject.m_Type = typeof(Bird.HUD_NavArrow);

            Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, arrowObject);

            m_runnerNum = _runnerNum;
            Bird.CheckpointManager.CM.AddCheckpoint(Kojima.GameController.s_singleton.m_players[_runnerNum].gameObject, gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex);
        }

        //remove scripts related to the runner
        public void ResetRunner()
        {
            m_bRunner = false;
            Destroy(GetComponent<Health>());
            GetComponent<WaypointSpawner>().ResetWaypoints();
            Destroy(GetComponent<WaypointSpawner>());
			FinishManager.s_pFinishManager.ResetFinish();
        }

        //remove scripts related to the chaser
        public void ResetChaser()
        {
            m_bChaser = false;
            Destroy(GetComponent<Health>());
            Destroy(GetComponent<Chaser>());
            Bird.CheckpointManager.CM.RemoveCheckpoint(Kojima.GameController.s_singleton.m_players[m_runnerNum].gameObject, gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex);
        }
    }
}
