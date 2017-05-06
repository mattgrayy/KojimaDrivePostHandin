//If you have any issues or questions talk to me (Anthony Arian / DixiE)

using UnityEngine;
using System;


    public class TimeOfDayScript : MonoBehaviour
    {
        public static TimeOfDayScript s_singleton;

        //For debugging basically
        [SerializeField]
        private int m_hour, m_minute, m_second;

        //Current in-game date and time
        private DateTime m_time;
        //Counts real world seconds and resets when above or equal to 1
        private float m_secondsTimer;
        //Number of real-world seconds per in-game second
        private float m_secondsPerSecond;
        //Number of real-world seconds per in-game hour
        public float m_secondsPerHour = 60.0f;

        void Awake()
        {
            //Initialise datetime to first year, month, day at midday.
            //Can be changed to specific numbers for testing or loading purposes
            m_time = new DateTime(2017, 1, 1, 12, 0, 0);
            m_secondsTimer = 0.0f;
            m_secondsPerSecond = (m_secondsPerHour / 60.0f) / 60.0f;
            if(!s_singleton)
            {
                s_singleton = this;
            }
        }

        void Update()
        {
            m_secondsTimer += Time.deltaTime;
            while (m_secondsTimer >= m_secondsPerSecond)
            {
                int secondsPast = (int)(m_secondsTimer / m_secondsPerSecond);
                m_secondsTimer -= secondsPast * m_secondsPerSecond;
                m_time = m_time.AddSeconds(secondsPast);
            }
            m_hour = m_time.Hour;
            m_minute = m_time.Minute;
            m_second = m_time.Second;
        }

        public DateTime GetTime()
        {
            return m_time;
        }
    }

