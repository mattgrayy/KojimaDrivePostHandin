using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace HF
{
    //===================== Kojima Drive - Half-Full 2017 ====================//
    //
    // Author: ADAM MOOREY
    // Purpose: Manager which handles the adding, and choosing of a finish point
    // Namespace: HALF-FULL
    //
    //===============================================================================//

    public class FinishManager : MonoBehaviour
    {
        public static FinishManager s_pFinishManager;
            
        List<GameObject> m_glisFinishPoints = new List<GameObject>();

        public bool m_bPick = false;
        public bool m_bSpawned = false;
        private int m_runnerNum;
        private int m_chosenFinish;

        GameObject eventObject;

        void Start()
        {
            if (s_pFinishManager == null)
            {
                s_pFinishManager = this;
            }
            else if (s_pFinishManager != this)
            {
                Destroy(gameObject);
            }

            foreach (Transform finishPoint in transform)
            {
                m_glisFinishPoints.Add(finishPoint.gameObject);
            }
        }

        public void PickFinish()
        {
            //if current time is equal to half of chasing & spawned is false
            if (m_bPick && !m_bSpawned)
            {
                eventObject = gameObject.transform.parent.gameObject;
                eventObject.GetComponent<DriveAndSeekMode>().m_infoText.GetComponent<Text>().text = "Get To The Finish!";

                m_chosenFinish = Random.Range(0, m_glisFinishPoints.Count);
                m_glisFinishPoints[m_chosenFinish].SetActive(true);
                m_glisFinishPoints[m_chosenFinish].AddComponent<FinishPoint>();

                for (int iter = 0; iter <= eventObject.GetComponent<DriveAndSeekMode>().m_numberOfPlayers - 1; iter++)
                {
                    if (Kojima.GameController.s_singleton.m_players[iter].gameObject.GetComponent<DriveAndSeek>().m_bRunner)
                    {
                        Bird.HUDController.hudElementToggleData_t arrowObject = new Bird.HUDController.hudElementToggleData_t();
                        arrowObject.m_nPlayerID = Kojima.GameController.s_singleton.m_players[iter].gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex;
                        arrowObject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.ENABLE;
                        arrowObject.m_Type = typeof(Bird.HUD_NavArrow);

                        Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, arrowObject);

                        m_runnerNum = Kojima.GameController.s_singleton.m_players[iter].gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex;
                        Bird.CheckpointManager.CM.AddCheckpoint(m_glisFinishPoints[m_chosenFinish], m_runnerNum);
                    }
                }
            
                m_bSpawned = true;
            }
        }

		public void ResetFinish()
		{
			foreach (Transform child in gameObject.transform) 
			{
				if (child.gameObject.activeSelf) 
				{
					if (eventObject)
                    {
                        Bird.CheckpointManager.CM.RemoveCheckpoint(m_glisFinishPoints[m_chosenFinish], m_runnerNum);
                        m_bPick = false;
						m_bSpawned = false;
						Destroy(child.gameObject.GetComponent<Bird.Checkpoint>());
						Destroy(child.gameObject.GetComponent<FinishPoint>());
						child.gameObject.SetActive(false);
                    }
				}
			}
		}
    }
}
