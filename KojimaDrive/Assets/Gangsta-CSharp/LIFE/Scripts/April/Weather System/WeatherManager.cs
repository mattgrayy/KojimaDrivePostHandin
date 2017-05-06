using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace GCSharp
{
    public class WeatherManager : MonoBehaviour
    {

        //CURRENT WEATHER SYSTEMS IN PLACE:
        //NULL = No weather, as in a standard overcast day
        //RAIN 
        //SNOW
        //FOG
        //THUNDERSTORM = Same as RAIN, with an added skybox flash/w directional light

        //To work, the manager prefab must be dragged in to the scene, as well as the 
        //particle systems required dragged and placed in to cover the island accordingly

        //This will require several instances of each particle system, but is intended to work
        //with the chunk manager, so not all are on at once (if activated)

        //Ensure the particle systems pulled in to the scene are all OFF as well as the emitter set of OFF
        //The script turns on the particle system, as well as the emitter itself when that weather is activated

        [SerializeField]
        private bool m_bUseNull = true;

        [SerializeField]
        private bool m_bUseRain = true;

        [SerializeField]
        private bool m_bUseThunderstorm = true;

        [SerializeField]
        private bool m_bUseSnow = true;

        [SerializeField]
        private bool m_bUseFog = true;

		private WeatherThemedSkyScript m_skyScript;

        public GameObject m_rainPrefab;
        public Vector3 m_rainOffset = new Vector3(0, 5, 0);
        public GameObject m_snowPrefab;
        public Vector3 m_snowOffset = new Vector3(0, 5, 0);
        public GameObject m_fogPrefab;
        public Vector3 m_fogOffset = new Vector3(0, 0, 0);
		public GameObject m_thunderPrefab;
		public Vector3 m_thunderOffset = new Vector3(0, 5, 0);

		//Length = Particle System count (4) * Max Player Count (4)
		private GameObject[] m_particleObjects = new GameObject[16];
        
        //Length = Max Player Count (4)
        private RainSystem[] m_rainSystems = new RainSystem[4];
        
        //Length = Max Player Count (4)
        private SnowSystem[] m_snowSystems = new SnowSystem[4];
        
        //Length = Max Player Count (4)
        private FogSystem[] m_fogSystems = new FogSystem[4];

		//Length = Max Player Count (4)
		private ThunderSystem[] m_thunderSystems = new ThunderSystem[4];

        private int m_prevPlayerCount;


        private bool m_bIsChanging;

        public int m_iDefaultHoursToNewWeather = 12;
        public int m_iWeatherSwapHourVariance = 2;
        [SerializeField]
        private int m_iHoursToNewWeather;
        private int m_iHoursSinceLastWeather;

        private int m_iNewWeather;
        private bool m_bStartWeatherChange;

        private int m_weatherTimerStart;
        [SerializeField]
        private int m_weatherTimerCounter;

        private TimeOfDayScript m_timeOfDayScript;

        public enum WeatherType
        {
            NULL,
            RAIN,
            THUNDERSTORM,
            SNOW,
            FOG,
            NUMBEROFWEATHERTYPES
        };

        public WeatherType m_enCurrentWeather;
        public WeatherType m_enLastWeather;

        public bool GetSet_bUseNull
        {
            get { return m_bUseNull; }
            set { m_bUseNull = value; }
        }

        public bool GetSet_bUseRain
        {
            get { return m_bUseRain; }
            set { m_bUseRain = value; }
        }

        public bool GetSet_bUseThunderstorm
        {
            get { return m_bUseThunderstorm; }
            set { m_bUseThunderstorm = value; }
        }

        public bool GetSet_bUseSnow
        {
            get { return m_bUseSnow; }
            set { m_bUseSnow = value; }
        }

        public bool GetSet_bUseFog
        {
            get { return GetSet_bUseFog; }
            set { GetSet_bUseFog = value; }
        }

        void Awake()
        {

			m_skyScript = GetComponent<WeatherThemedSkyScript>();
            for(int i=0; i<4; i++)
            {
                m_particleObjects[i] = Instantiate(m_rainPrefab);
                m_rainSystems[i] = m_particleObjects[i].GetComponent<RainSystem>();
                Debug.Assert(m_rainSystems[i], "Rain system " + i + " failed to be retrieved");

                m_particleObjects[i + 4] = Instantiate(m_snowPrefab);
                m_snowSystems[i] = m_particleObjects[i + 4].GetComponent<SnowSystem>();
                Debug.Assert(m_snowSystems[i], "Snow system " + i + " failed to be retrieved");

                m_particleObjects[i + 8] = Instantiate(m_fogPrefab);
                m_fogSystems[i] = m_particleObjects[i + 8].GetComponent<FogSystem>();
                Debug.Assert(m_fogSystems[i], "Fog system " + i + " failed to be retrieved");

				m_particleObjects[i + 12] = Instantiate(m_thunderPrefab);
				m_thunderSystems[i] = m_particleObjects[i + 12].GetComponent<ThunderSystem>();
				Debug.Assert(m_thunderSystems[i], "Thunder system " + i + " failed to be retrieved");
			}
        }

        void Start()
        {
            m_prevPlayerCount = Kojima.GameController.s_ncurrentPlayers;
            m_timeOfDayScript = TimeOfDayScript.s_singleton;
            ResetWeatherChangeTimer();
            PickRandomWeather();
        }

        void Update()
        {
            int currentPlayerCount = Kojima.GameController.s_ncurrentPlayers;
            if(currentPlayerCount < m_prevPlayerCount)
            {
                //disable a player's particle effects
                int leaverCount = m_prevPlayerCount - currentPlayerCount;
                for(int i=0; i<leaverCount; i++)
                {
                    m_rainSystems[m_prevPlayerCount - i - 1].ExitWeatherEffect();
                    m_snowSystems[m_prevPlayerCount - i - 1].ExitWeatherEffect();
                    m_fogSystems[m_prevPlayerCount - i - 1].ExitWeatherEffect();
                }
            }
            if(currentPlayerCount > m_prevPlayerCount)
            {
                m_prevPlayerCount = currentPlayerCount;
                SwapCurrentWeather((int)m_enCurrentWeather);
            }
            m_prevPlayerCount = currentPlayerCount;
            for(int i=0; i<m_prevPlayerCount; i++)
            {
                Vector3 playerPos = Kojima.GameController.s_singleton.m_players[i].transform.position;
                Vector3 rainPos = m_rainSystems[i].transform.position;
                rainPos = playerPos + m_rainOffset;
                m_rainSystems[i].transform.position = rainPos;
                Vector3 snowPos = m_snowSystems[i].transform.position;
                snowPos = playerPos + m_snowOffset;
                m_snowSystems[i].transform.position = snowPos;
                Vector3 fogPos = m_fogSystems[i].transform.position;
                fogPos = playerPos + m_fogOffset;
                m_fogSystems[i].transform.position = fogPos;
				Vector3 thunderPos = m_thunderSystems[i].transform.position;
				thunderPos = playerPos + m_thunderOffset;
				m_thunderSystems[i].transform.position = thunderPos;
			}

            int currentHour = m_timeOfDayScript.GetTime().Hour;
            if(m_weatherTimerStart > currentHour)
            {
                //if current hour is part of next day do this
                currentHour += 24;
            }
            m_weatherTimerCounter = currentHour - m_weatherTimerStart;

            if (m_weatherTimerCounter >= m_iHoursToNewWeather)
            {
                m_bIsChanging = true;
                PickRandomWeather();
                ResetWeatherChangeTimer();
                SwapCurrentWeather(m_iNewWeather);
            }
        }

		public WeatherType GetWeather()
		{
			return m_enCurrentWeather;
		}

        private void ResetWeatherChangeTimer()
        {
            m_weatherTimerCounter = 0;
            m_weatherTimerStart = m_timeOfDayScript.GetTime().Hour;
            m_iHoursToNewWeather = m_iDefaultHoursToNewWeather 
                                    + Random.Range(-m_iWeatherSwapHourVariance, m_iWeatherSwapHourVariance + 1);

        }

        private void PickRandomWeather()
        {
            int Weather;
            Weather = Random.Range(0, (int)WeatherType.NUMBEROFWEATHERTYPES);
            CheckIfWeatherTypeIsOn(Weather);
        }

        void CheckIfWeatherTypeIsOn(int NewWeatherType)
        {

            if (NewWeatherType == (int)WeatherType.NULL && m_bUseNull != false)
            {
                m_iNewWeather = NewWeatherType;
                m_bStartWeatherChange = true;
            }
            else if (NewWeatherType == (int)WeatherType.RAIN && m_bUseRain != false)
            {
                m_iNewWeather = NewWeatherType;
                m_bStartWeatherChange = true;
            }
            else if (NewWeatherType == (int)WeatherType.THUNDERSTORM && m_bUseThunderstorm != false)
            {
                m_iNewWeather = NewWeatherType;
                m_bStartWeatherChange = true;
            }
            else if (NewWeatherType == (int)WeatherType.SNOW && m_bUseSnow != false)
            {
                m_iNewWeather = NewWeatherType;
                m_bStartWeatherChange = true;
            }
            else if (NewWeatherType == (int)WeatherType.FOG && m_bUseFog != false)
            {
                m_iNewWeather = NewWeatherType;
                m_bStartWeatherChange = true;
            }
        }

        void ChangeWeatherToNull()
        {
            m_enCurrentWeather = WeatherType.NULL;
			m_skyScript.ResetTargets();
        }

        void ChangeWeatherToRain()
        {
            m_enCurrentWeather = WeatherType.RAIN;
            for(int i=0; i<m_prevPlayerCount; i++)
            {
                m_rainSystems[i].Init();
            }
			m_skyScript.SetRainTargets();
        }

        void ChangeWeatherToThunderstorm()
        {
            m_enCurrentWeather = WeatherType.THUNDERSTORM;
            for (int i = 0; i < m_prevPlayerCount; i++)
            {
                m_thunderSystems[i].Init();
            }
			m_skyScript.SetThunderTargets();
		}

        void ChangeWeatherToSnow()
        {
            m_enCurrentWeather = WeatherType.SNOW;
            for (int i = 0; i < m_prevPlayerCount; i++)
            {
                m_snowSystems[i].Init();
            }
			m_skyScript.SetSnowTargets();
		}

        void ChangeWeatherToFog()
        {
            m_enCurrentWeather = WeatherType.FOG;
            for (int i = 0; i < m_prevPlayerCount; i++)
            {
                m_fogSystems[i].Init();
            }
			m_skyScript.SetFogTargets();
		}

        void SwapCurrentWeather(int NewWeatherType)
        {
            m_enLastWeather = m_enCurrentWeather;

            if (m_enCurrentWeather == WeatherType.NULL)
            {
                //Enter new weather type
                EnterNewWeather(NewWeatherType);
                m_bStartWeatherChange = false;
            }
            else if (m_enCurrentWeather == WeatherType.RAIN)
            {
                //Call the exit function in the weather type
                for (int i = 0; i < m_prevPlayerCount; i++)
                {
                    m_rainSystems[i].ExitWeatherEffect();
                }
                EnterNewWeather(NewWeatherType);
                m_bStartWeatherChange = false;
            }
            else if (m_enCurrentWeather == WeatherType.THUNDERSTORM)
            {
                for (int i = 0; i < m_prevPlayerCount; i++)
                {
                    m_thunderSystems[i].ExitWeatherEffect();
                }
                EnterNewWeather(NewWeatherType);
                m_bStartWeatherChange = false;
            }
            else if (m_enCurrentWeather == WeatherType.SNOW)
            {
                //Call the exit function in the weather type
                for (int i = 0; i < m_prevPlayerCount; i++)
                {
                    m_snowSystems[i].ExitWeatherEffect();
                }
                EnterNewWeather(NewWeatherType);
                m_bStartWeatherChange = false;
            }
            else if (m_enCurrentWeather == WeatherType.FOG)
            {
                //Call the exit function in the weather type
                for (int i = 0; i < m_prevPlayerCount; i++)
                {
                    m_fogSystems[i].ExitWeatherEffect();
                }
                EnterNewWeather(NewWeatherType);
                m_bStartWeatherChange = false;

            }
        }

        private void EnterNewWeather(int NewWeather)
        {
            if (NewWeather == (int)WeatherType.NULL)
            {
                ChangeWeatherToNull();
            }
            else if (NewWeather == (int)WeatherType.RAIN)
            {
                ChangeWeatherToRain();
            }
            else if (NewWeather == (int)WeatherType.THUNDERSTORM)
            {
                ChangeWeatherToThunderstorm();
            }
            else if (NewWeather == (int)WeatherType.SNOW)
            {
                ChangeWeatherToSnow();
            }
            else if (NewWeather == (int)WeatherType.FOG)
            {
                ChangeWeatherToFog();
            }
        }
    }
}
