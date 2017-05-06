using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace KRace
{

	public class DraftDetector : MonoBehaviour
	{

		Dictionary<string, int> m_storeTrigger;
		float m_timeClear;
		float m_fadeTime;

        bool m_bDraftTimeout;
        float m_fDraftTimeout;
        float m_draftStopped;

		// Use this for initialization
		void Start()
		{
			m_fadeTime = 1.0f;

			m_storeTrigger = new Dictionary<string, int>();
			m_timeClear = Time.time + m_fadeTime;
            m_fDraftTimeout = 3.0f;
            m_draftStopped = 0.0f;
        }

		// Update is called once per frame
		void Update()
		{
			//List storing all dictionary keys at start of this tick
			List<string> triggerKeys = new List<string>(m_storeTrigger.Keys);

			//Minus total trigger count for each car by 1 every m_fadeTime seconds
			if (Time.time >= m_timeClear)
			{
				foreach (string trigger in triggerKeys)
				{
					if (m_storeTrigger[trigger] > 0)
					{
						m_storeTrigger[trigger] -= 1;
					}
				}

				m_timeClear = Time.time + m_fadeTime;
			}

			//Add boost if drafting count is >= 3
			foreach (string trigger in triggerKeys)
			{
				if (m_storeTrigger[trigger] >= 3)
				{
					transform.GetComponentInParent<Kojima.CarScript>().SmallBoost(0.5f);
                    Debug.Log("Draft");
                    m_storeTrigger[trigger] = 0;
                }
			}

            // True if the car is on a timeout from draft bonus
            m_bDraftTimeout = Time.time <= m_draftStopped;
            
           
        }


		// When the draft detector collides
		void OnTriggerEnter(Collider other)
		{
			string collidedName = other.gameObject.name;

			//Check if collision is with a draft trigger (emmited by a car)
			if (other.gameObject.GetComponent<DraftTrigger>() && !m_bDraftTimeout)
			{
				DraftTrigger collidedTrigger = other.gameObject.GetComponent<DraftTrigger>();

				//If the trigger is from another car increment the draft score against that car by 1
				if (transform.name != collidedTrigger.parentName)
				{
                    //If the car hits a bumper set a timeout for m_fDraftTimeout seconds
                    if (collidedTrigger.m_isBumper)
                    {
                        m_storeTrigger.Clear();
                        Debug.Log("Clear Draft");
                        m_draftStopped = Time.time + m_fDraftTimeout;
                    }
                    else
                    {

                        if (!m_storeTrigger.ContainsKey(collidedTrigger.parentName))
                        {
                            m_storeTrigger.Add(collidedTrigger.parentName, 1);
                        }
                        else
                        {
                            m_storeTrigger[collidedTrigger.parentName]++;
                        }

                        collidedTrigger.setDead();
                    }

				}

			}
		}

	}

}