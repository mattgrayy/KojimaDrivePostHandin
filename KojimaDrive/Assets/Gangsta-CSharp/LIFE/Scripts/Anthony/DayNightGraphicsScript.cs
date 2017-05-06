//If you have any issues or questions talk to me (Anthony Arian / DixiE)

using UnityEngine;
using System.Collections;


    [RequireComponent(typeof(DayNightCycleScript))]
    [RequireComponent(typeof(TimeOfDayScript))]

    public class DayNightGraphicsScript : MonoBehaviour
    {
        private GameObject m_sunlightSource;
        private Light m_sunlight;
        private GameObject m_moonlightSource;
        private Light m_moonlight;
        private DayNightCycleScript m_dayNightCycle;
        private TimeOfDayScript m_timeOfDay;

        //Detailed angles for the sun
        public float m_sunriseRotation, m_daytimeRotation, m_sunsetRotation, m_nightRotation;
        public float m_maxSunlightIntensity = 1.0f;
        public float m_maxMoonlightIntensity = 0.4f;

        void Awake()
        {
            m_sunlightSource = GameObject.FindGameObjectWithTag("Sun");
            m_sunlight = m_sunlightSource.GetComponent<Light>();
            m_moonlightSource = m_sunlightSource.transform.GetChild(0).gameObject;
            m_moonlightSource.transform.Rotate(Vector3.right, 180);
            m_moonlight = m_moonlightSource.GetComponent<Light>();
            m_dayNightCycle = GetComponent<DayNightCycleScript>();
            m_timeOfDay = GetComponent<TimeOfDayScript>();
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            UpdateLightRotation();
        }

        private void UpdateLightRotation()
        {
            DayNightState currentState = m_dayNightCycle.GetState();
            float currentStateRot = m_sunriseRotation;
            int currentStateStart = m_dayNightCycle.GetSunriseHour();
            switch (currentState)
            {
                case DayNightState.DAYTIME: currentStateRot = m_daytimeRotation; currentStateStart = m_dayNightCycle.GetDaytimeHour(); break;
                case DayNightState.SUNSET: currentStateRot = m_sunsetRotation; currentStateStart = m_dayNightCycle.GetSunsetHour(); break;
                case DayNightState.NIGHT: currentStateRot = m_nightRotation; currentStateStart = m_dayNightCycle.GetNightHour(); break;
                default: break;
            }

            float nextStateRot = m_daytimeRotation;
            int nextStateStart = m_dayNightCycle.GetDaytimeHour();
            switch (currentState)
            {
                case DayNightState.DAYTIME: nextStateRot = m_sunsetRotation; nextStateStart = m_dayNightCycle.GetSunsetHour(); break;
                case DayNightState.SUNSET: nextStateRot = m_nightRotation; nextStateStart = m_dayNightCycle.GetNightHour(); break;
                case DayNightState.NIGHT: nextStateRot = m_sunriseRotation; nextStateStart = m_dayNightCycle.GetSunriseHour(); break;
                default: break;
            }
            if (currentStateStart > nextStateStart)
            {
                nextStateStart += 24;
            }
            int secDifference = (nextStateStart - currentStateStart) * 60 * 60;
            int hour = m_timeOfDay.GetTime().Hour;
            if (hour < currentStateStart)
            {
                hour += 24;
            }
            int currentSec = (hour) * 60 * 60 + m_timeOfDay.GetTime().Minute * 60 + m_timeOfDay.GetTime().Second;
            int secSinceCurrentState = currentSec - (currentStateStart * 60 * 60);
            float progress = (float)secSinceCurrentState / secDifference;
            if (progress > 1.0f)
            {
                progress = 1.0f;
            }
            Quaternion initRotation = Quaternion.AngleAxis(currentStateRot, Vector3.right);
            Quaternion targetRotation = Quaternion.AngleAxis(nextStateRot, Vector3.right);
            m_sunlightSource.transform.rotation = Quaternion.Lerp(initRotation, targetRotation, progress);
            float angle = m_sunlightSource.transform.rotation.eulerAngles.x;
            float sunSin = Mathf.Sin(angle * Mathf.PI / 180.0f);
            float sunIntensity = sunSin * 10.0f;
            sunIntensity = Mathf.Clamp(sunIntensity, 0.0f, 1.0f);
            m_sunlight.intensity = sunIntensity * m_maxSunlightIntensity;
            m_moonlight.intensity = m_maxMoonlightIntensity - sunIntensity * m_maxMoonlightIntensity;
        }
    }

