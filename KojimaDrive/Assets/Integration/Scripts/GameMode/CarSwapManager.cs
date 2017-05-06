using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kojima
{
    public class CarSwapManager : MonoBehaviour
    {
        public static CarSwapManager m_sInstance = null;

        public enum CarType
        {
            NA,
            CARLO,
            MANTA,
            MINI,
            JAGUAR,
            ICECREAM,
            LOUIS,
            METRO,
            OPEL,
            RV,
            CAPRI,
            ZUK,
            count
        }

#region CARSTATS
		public class carStats_e {
			public carStats_e(string strName, CarType type, int nEXPPrice) {
				name = strName;
				m_eType = type;
				m_nEXPPrice = nEXPPrice;
			}
			public string name;
			public CarType m_eType;
			public int m_nEXPPrice;
		}

		public static carStats_e[] m_arrCarStats = new carStats_e[] {
			new carStats_e("NULL",			CarType.NA,			9999999),
			new carStats_e("MONTECRISTO",	CarType.CARLO,		0),
			new carStats_e("ROY-RAY",	    CarType.MANTA,		3000),
			new carStats_e("TINY TAYLOR",	CarType.MINI,		1000),
			new carStats_e("LYNX",			CarType.JAGUAR,		15000),
			new carStats_e("WHIPPY",		CarType.ICECREAM,	6000),
			new carStats_e("HAMILTON",      CarType.LOUIS,		8000),
			new carStats_e("RAPIDO",        CarType.METRO,		10000),
			new carStats_e("PEARL",			CarType.OPEL,		12000),
			new carStats_e("HAR-V",         CarType.RV,			4500),
			new carStats_e("SOLARIS",		CarType.CAPRI,		7000),
			new carStats_e("BUSTO",         CarType.ZUK,		11000),
		};											

		public static carStats_e GetCarStats(CarType eType) {
			for(int i = 0; i < m_arrCarStats.Length; i++) {
				if(m_arrCarStats[i].m_eType == eType) {
					return m_arrCarStats[i];
				}
			}

			return null;
		}
#endregion


		public List<GameObject> m_carPrefab = new List<GameObject>();

        private bool m_bSwappingAllowed = true;

        void Awake()
        {
            if (m_sInstance)
            {
                Destroy(this.gameObject);
            }
            else
            {
                m_sInstance = this;
            }
        }

        GameObject GetCar(string _name)
        {
            GameObject carPrefab = null;
            foreach(GameObject prefab in m_carPrefab)
            {
                if(prefab.GetComponent<CarData>().m_name == _name)
                {
                    carPrefab = prefab;
                }
            }
            return carPrefab;
        }

        public GameObject GetCar(CarType _type)
        {
            GameObject carPrefab = null;
            foreach (GameObject prefab in m_carPrefab)
            {
                if (prefab.GetComponent<CarData>().m_type == _type)
                {
                    carPrefab = prefab;
                }
            }
            return carPrefab;
        }

        GameObject GetCar(int _carIndex)
        {
            GameObject carPrefab = null;
            foreach (GameObject prefab in m_carPrefab)
            {
                if (prefab.GetComponent<CarData>().m_index == _carIndex)
                {
                    carPrefab = prefab;
                }
            }
            return carPrefab;
        }

		// Car Swap event data object -sam
		public class carSwapEventData_t {
			public int m_nPlayerID;
		}

        public void ChangeCar(int _playerIndex, int _controllerId, string _name)
        {
            if(_playerIndex>3)
            {
                return;
            }

            GameObject prefab = GetCar(_name);
            if (prefab)
            {
                Vector3 pos = Kojima.GameController.s_singleton.m_players[_playerIndex].gameObject.transform.position;
                Quaternion rot = Kojima.GameController.s_singleton.m_players[_playerIndex].gameObject.transform.rotation;
                Kojima.GameController.s_singleton.m_players[_playerIndex].RemoveFromScene();
                GameObject newCar = Instantiate(prefab, pos, rot) as GameObject;
                newCar.GetComponent<Kojima.CarScript>().m_nControllerID = _controllerId;
                Kojima.GameController.s_singleton.m_players[_playerIndex] = newCar.GetComponent<Kojima.CarScript>();
                Kojima.CameraManagerScript.singleton.ResetPlayerCameraFocus(_playerIndex);

				carSwapEventData_t swapdata = new carSwapEventData_t();
				swapdata.m_nPlayerID = _playerIndex + 1;
				EventManager.m_instance.AddEvent(Events.Event.CAR_SWAPPED, swapdata);
            }
            else
            {
                Debug.LogWarning("No car of name: " + _name);
            }
        }

        public void ChangeCar(int _playerIndex, int _controllerId, CarType _type)
        {
            GameObject prefab = GetCar(_type);
            if (prefab)
            {
                Vector3 pos = Kojima.GameController.s_singleton.m_players[_playerIndex].gameObject.transform.position;
                Quaternion rot = Kojima.GameController.s_singleton.m_players[_playerIndex].gameObject.transform.rotation;
                Kojima.GameController.s_singleton.m_players[_playerIndex].RemoveFromScene();
                GameObject newCar = Instantiate(prefab, pos, rot) as GameObject;
                newCar.GetComponent<Kojima.CarScript>().m_nControllerID = _controllerId;
                Kojima.GameController.s_singleton.m_players[_playerIndex] = newCar.GetComponent<Kojima.CarScript>();
                Kojima.CameraManagerScript.singleton.ResetPlayerCameraFocus(_playerIndex);

				carSwapEventData_t swapdata = new carSwapEventData_t();
				swapdata.m_nPlayerID = _playerIndex + 1;
				EventManager.m_instance.AddEvent(Events.Event.CAR_SWAPPED, swapdata);
			}
            else
            {
                Debug.LogWarning("No car of type: " + _type);
            }
        }

        public void ChangeCar(int _playerIndex, int _controllerId, int _carIndex)
        {
            GameObject prefab = GetCar(_carIndex);
            if (prefab)
            {
                Vector3 pos = Kojima.GameController.s_singleton.m_players[_playerIndex].gameObject.transform.position;
                Quaternion rot = Kojima.GameController.s_singleton.m_players[_playerIndex].gameObject.transform.rotation;
                Kojima.GameController.s_singleton.m_players[_playerIndex].RemoveFromScene();
                GameObject newCar = Instantiate(prefab, pos, rot) as GameObject;
                newCar.GetComponent<Kojima.CarScript>().m_nControllerID = _controllerId;
                Kojima.GameController.s_singleton.m_players[_playerIndex] = newCar.GetComponent<Kojima.CarScript>();
                Kojima.CameraManagerScript.singleton.ResetPlayerCameraFocus(_playerIndex);

				carSwapEventData_t swapdata = new carSwapEventData_t();
				swapdata.m_nPlayerID = _playerIndex + 1;
				EventManager.m_instance.AddEvent(Events.Event.CAR_SWAPPED, swapdata);
			}
            else
            {
                Debug.LogWarning("No car of index: " + _carIndex);
            }
        }

        public bool GetSwapping()
        {
            return m_bSwappingAllowed;
        }

        public void SetSwapping(bool _state)
        {
            m_bSwappingAllowed = _state;
        }
    }
}
