using UnityEngine;
using System.Collections;

namespace HF
{
    //===================== Kojima Drive - Half-Full 2017 ====================//
    //
    // Author: ADAM MOOREY
    // Purpose: Manages logic for the finish of the game, once finish point is reached
    // Namespace: HALF-FULL
    //
    //===============================================================================//

    public class FinishPoint : MonoBehaviour
    {
		GameObject m_goParent;
		GameObject m_goEventObject;

		void Awake()
		{
			m_goParent = gameObject.transform.parent.gameObject;
			m_goEventObject = m_goParent.transform.parent.gameObject;
		}

        void OnTriggerStay(Collider _col)
        {
            if (_col.GetComponent<DriveAndSeek>())
            {
                if (_col.GetComponent<DriveAndSeek>().m_bRunner)
                {
					if (m_goEventObject)
                    {
                        Kojima.GameController.s_singleton.m_players[_col.gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex - 1].PlayerEXP.AddEXP(100);
                        m_goEventObject.GetComponent<DriveAndSeekMode>().m_hiderWon = true;
						m_goEventObject.GetComponent<DriveAndSeekMode>().m_currentPhase = m_goEventObject.GetComponent<DriveAndSeekMode>().GetPhase("Reset");
						m_goEventObject.GetComponent<DriveAndSeekMode>().InitializePhase();
						FinishManager.s_pFinishManager.ResetFinish();
					}
                }
                else
                {
                    //the runner isn't colliding
                }
            }
            else
            {
                //ignore the collider
            }
        }
    }
}