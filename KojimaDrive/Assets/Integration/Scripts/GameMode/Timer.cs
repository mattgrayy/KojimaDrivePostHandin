using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Kojima
{
    //===================== Kojima Drive - Half-Full 2017 ====================//
    //
    // Author: HARRY DAVIES
    // Purpose: Manages a timer used for keeping track of phases
    // Namespace: HALF-FULL
    //
    //===============================================================================//

    public class Timer : MonoBehaviour
    {
        public bool m_bCounting = false;
        public bool m_bDelaying = false;
        public bool m_bMinutes = false;

        public string m_sName = "";
        public float m_fStartTime = 100000.0f;
        public float m_fTimerLength = 0.0f;
        public float m_fDelayLength = 0.0f;
        public float m_fDelayStart = 100000.0f;
        public float m_fPreCounted = 0.0f;

		// Added this to lerp the timer colour
		public float m_fRemainingTimeSeconds = 0.0f;

        public string m_sText = "";

        void Update()
        {
            if (m_bDelaying)
            {
                CheckDelayFinished();
            }

            if (!m_bDelaying && m_bCounting)
            {
                if (!m_bMinutes)
                {
                    m_sText = "" + (int)(m_fTimerLength - (Time.time - m_fStartTime));
                }
                else
                {
					/*float remainingTime*/ m_fRemainingTimeSeconds = m_fTimerLength - (Time.time - m_fStartTime);
					float minutes = m_fRemainingTimeSeconds / 60.0f;
                    float seconds = (minutes - (int)minutes) * 60.0f;

                    m_sText = "";
                    if ((int)minutes < 10)
                    {
                        m_sText = m_sText + 0;
                    }
                    m_sText = m_sText + (int)minutes + ":";
                    if((int)seconds < 10)
                    {
                        m_sText = m_sText + 0;
                    }
                    m_sText = m_sText + (int)seconds;
                }
            }
        }

        public void SetTimer(string _name, bool _minutes, float _timerLength, float _delayLength = 0.0f)
        {
            m_sName = _name;
            m_bMinutes = _minutes;
            m_fTimerLength = _timerLength;
            m_fDelayLength = _delayLength;
        }

        public void StartTimer()
        {
            if(m_fDelayLength == 0.0f)
            {
                BeginCount();
            }
            else
            {
                m_bDelaying = true;
                m_fDelayStart = Time.time;
            }
        }

        public void PauseTimer()
        {
            m_fPreCounted = Time.time - m_fStartTime;
            m_bCounting = false;
        }

        public void UnpauseTimer()
        {
            m_fStartTime = Time.time;
            m_bCounting = true;
        }

        void BeginCount()
        {
            m_bCounting = true;
            m_fStartTime = Time.time;
        }

        public void ResetTimer()
        {
            m_bCounting = false;
            m_fStartTime = 10000.0f;
            m_fPreCounted = 0.0f;
        }

        public bool CheckFinished()
        {
            if (Time.time - m_fStartTime >= m_fTimerLength - m_fPreCounted)
            {
                m_bCounting = false;
                return true;
            }
            return false;
        }

        void CheckDelayFinished()
        {
            if (Time.time - m_fDelayStart >= m_fDelayLength)
            {
                m_bDelaying = false;
                BeginCount();
            }
        }

        public int GetTimeLeft()
        {
            return (int)(m_fTimerLength - (Time.time - m_fStartTime));
        }

        public void StopTimer()
        {
            m_bCounting = false;
            ResetTimer();
        }
    }
}