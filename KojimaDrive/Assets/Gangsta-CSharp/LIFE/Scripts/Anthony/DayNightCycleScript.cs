//If you have any issues or questions talk to me (Anthony Arian / DixiE)

using UnityEngine;
using System;


    public enum DayNightState
    {
        SUNRISE,
        DAYTIME,
        SUNSET,
        NIGHT
    };

    [RequireComponent(typeof(TimeOfDayScript))]
    public class DayNightCycleScript : MonoBehaviour
    {
        public static DayNightCycleScript s_singleton;

        //True when sun is up, false when sun is down
        [SerializeField]
        private bool m_sunUp;

        //Reference to TimeOfDayScript
        private TimeOfDayScript m_timeOfDayScript;

        //Specifies at which hour this time of day begins, they have to be sensible values
        [SerializeField]
        private int m_sunriseHour, m_daytimeHour, m_sunsetHour, m_nightHour;

        [SerializeField]
        private DayNightState m_state;

        // Use this for initialization
        void Awake()
        {
            s_singleton = this;
            m_sunUp = true;
            m_timeOfDayScript = GetComponent<TimeOfDayScript>();
            m_state = DayNightState.DAYTIME;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateState();
        }

        private void UpdateState()
        {
            int hour = m_timeOfDayScript.GetTime().Hour;
            DayNightState prevState = m_state;
            if (hour >= m_sunriseHour && hour < m_daytimeHour)
            {
                m_state = DayNightState.SUNRISE;
            }
            if (hour >= m_daytimeHour && hour < m_sunsetHour)
            {
                m_state = DayNightState.DAYTIME;
            }
            if (hour >= m_sunsetHour && (hour < m_nightHour || m_sunsetHour > m_nightHour))
            {
                m_state = DayNightState.SUNSET;
            }
            if (hour >= m_nightHour && (hour < m_sunriseHour || m_nightHour > m_sunriseHour))
            {
                m_state = DayNightState.NIGHT;
            }
            if (m_state == DayNightState.DAYTIME)
            {
                m_sunUp = true;
            }
            else
            {
                m_sunUp = false;
            }
        }

        public bool IsDay()
        {
            return m_sunUp;
        }

        public DayNightState GetState()
        {
            return m_state;
        }

        public int GetSunriseHour()
        {
            return m_sunriseHour;
        }

        public int GetDaytimeHour()
        {
            return m_daytimeHour;
        }

        public int GetSunsetHour()
        {
            return m_sunsetHour;
        }

        public int GetNightHour()
        {
            return m_nightHour;
        }

        public DateTime GetDateTime()
        {
            return m_timeOfDayScript.GetTime();
        }
    }

