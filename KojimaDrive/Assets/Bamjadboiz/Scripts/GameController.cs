using UnityEngine;
using System.Collections;

//This is basically all debug stuff right now
namespace Kojima
{
    public class GameController : MonoBehaviour
    {

		public static GameController s_singleton;

		public enum island_e {
			NULL,
			KOJIMA,
			OHIYAKU
		}
		public static island_e s_eIsland = island_e.NULL;
		public island_e m_eIsland;

		public enum loadMode_e {
			STANDARD,			// Will spawn cars at a random spawn location on the map
			ISLAND_TRANSITION	// Will spawn cars at specific transition location
		}
		public static loadMode_e s_eLoadMode = loadMode_e.STANDARD;


		/// <summary>Maximum number of players.</summary>
		public static int s_nMaxPlayers = 4;

		/// <summary>Number of players currently playing.</summary>
		public static int s_ncurrentPlayers = 0;

		public class playerCreation_t {
			public bool m_bOptedIn;
			public CarSwapManager.CarType m_eChosenCar;
			public int m_nControllerID;
			public Vector3 m_SpawnPosition = Vector3.zero;
			public Vector3 m_SpawnOrientation = Vector3.zero;
		}

		public static playerCreation_t[] s_PlayerCreationData = new playerCreation_t[s_nMaxPlayers];
		public static bool SpawnPlayers() {
			// Spawn our players based on their selections
			int nSpawnedPlayers = 0;
			for (int i = 0; i < s_PlayerCreationData.Length; i++) {
				if (s_PlayerCreationData[i].m_bOptedIn) {
					GameObject prefab = CarSwapManager.m_sInstance.GetCar(s_PlayerCreationData[i].m_eChosenCar);
					GameObject instantiated = Instantiate(prefab);
					instantiated.transform.position = s_PlayerCreationData[i].m_SpawnPosition;
					instantiated.transform.rotation = Quaternion.Euler(s_PlayerCreationData[i].m_SpawnOrientation);
					nSpawnedPlayers++;
					CarScript carscr = instantiated.GetComponent<CarScript>();
					carscr.m_nControllerID = s_PlayerCreationData[i].m_nControllerID; // Controller ID and Player ID should have been decoupled from the start
					carscr.m_nplayerIndex = nSpawnedPlayers; // Basically, we can't do dropin/out otherwise.
				}
			}

			return nSpawnedPlayers != 0; // Did we spawn players? We might need to spawn some backup ones.
		}

		static public void PurgeGame() {
			// Unhook menu input
			Bird.BaseMenuScreen.UnhookInput();
			// Purge persistent (DontDestroyOnLoad) obbjects
			PurgePersistentObjects();
		}
		static public void PurgePersistentObjects() {
			System.Collections.Generic.List<Object> golist = ObjectDB.GetDontDestroyOnLoadObjects();
			for (int i = 0; i < golist.Count; i++) {
				if (golist[i] != null) {
					GameObject go = null;
					if (typeof(GameObject).IsAssignableFrom(golist[i].GetType())) {
						go = (GameObject)golist[i];
					} else if (typeof(Component).IsAssignableFrom(golist[i].GetType())) {
						go = ((Component)golist[i]).gameObject;
					}

					if (go == null) {
						Destroy(golist[i]);
					} else {
						Destroy(go);
					}

					golist[i] = null;
				}
			}
		}

		/// <summary>A collection of all CarScripts where m_players[0] is player 1, m_players[1] is player 2 and so on.</summary>
		public CarScript[] m_players;

		public CarScript GetCarByControllerID(int nControllerID) {
			for(int i = 0; i < m_players.Length; i++) {
				if(m_players[i] != null) {
					if (m_players[i].m_nControllerID == nControllerID) {
						return m_players[i];
					}
				}
			}

			return null;
		}

        /// <summary>Add really important singletons like HUD and camera manager to this array.</summary>
        public GameObject[] createTheseOnAwake;
		public static GameObject[] m_CreatedObjectReferences; // Hold these references so we can execute order 66 on this and purge it on return to menu

        public GameObject resultsScreenPrefab;

		// Adding this so we can clean up after returning to the main menu
		public static void Purge() {
			for(int i = 0; i < m_CreatedObjectReferences.Length; i++) {
				if(m_CreatedObjectReferences[i] != null) {
					DestroyImmediate(m_CreatedObjectReferences[i]);
					m_CreatedObjectReferences[i] = null;
				}

				m_CreatedObjectReferences = null;
				Destroy(s_singleton.gameObject);
				s_singleton = null;
			}
		}

        // Use this for initialization
        void Awake()
        {
            if (!s_singleton)
            {
                s_singleton = this;
				ObjectDB.DontDestroyOnLoad_Managed(gameObject);
                m_players = new CarScript[4];

				m_CreatedObjectReferences = new GameObject[createTheseOnAwake.Length];

				if(s_eIsland == island_e.NULL) {
					s_eIsland = m_eIsland; // If our island hasn't been set properly, set it from the prefab
				}


				for (int i = 0; i < createTheseOnAwake.Length; i++)
                {
                    if (createTheseOnAwake[i])
                    {
                        GameObject newObj = Instantiate<GameObject>(createTheseOnAwake[i]);
						ObjectDB.DontDestroyOnLoad_Managed(newObj);
						m_CreatedObjectReferences[i] = createTheseOnAwake[i];

					}
                    else
                    {
						m_CreatedObjectReferences[i] = null;

					}
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {

            //CameraManagerScript.singleton.SetupThirdPersonForAllPlayers();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PlayerHasDroppedOut(int playerIndex)
        {
            s_ncurrentPlayers--;

            for (int i=playerIndex; i<=m_players.Length; i++)
            {
                if(m_players[i-1])
                {
                    if(m_players[i-1].m_nplayerIndex>playerIndex)
                    {
                        CarScript playerToMove = m_players[i - 1];
                        m_players[i - 2] = playerToMove;
                        m_players[i - 1] = null;

                        playerToMove.SetNewPlayerIndex(playerToMove.m_nplayerIndex - 1);
                    }
                }
            }

            CameraManagerScript.singleton.SetupThirdPersonForAllPlayers();
        }

        public void SwapPlayers(int index1, int index2)
        {
            CarScript temp = m_players[index1];

            m_players[index1] = m_players[index2];

            if (m_players[index1])
            {
                m_players[index1].SetNewPlayerIndex(index2);
            }

            ////////////////////////////

            m_players[index2] = temp;

            if (m_players[index2])
            {
                m_players[index2].SetNewPlayerIndex(index1);
            }
        }

        public void AllCarsCanMove(bool _b)
        {
            foreach (var car in m_players)
            {
                if (car != null)
                {
                    car.CanMove = _b;
                }
            }
        }

    }
}