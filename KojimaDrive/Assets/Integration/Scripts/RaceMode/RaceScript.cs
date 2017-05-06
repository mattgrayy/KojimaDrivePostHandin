using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Kojima
{
    //sets up our enum for ease of use when making races
    public enum RP_Type
    {
        START, FINISH, CHECKPOINT
    }

    public class RaceScript : GameMode
    {
        private DropPointSpawn dpSpawn;
        public GameObject m_originalRacePoint;
        public List<GameObject> m_lRacePointPos;
        public List<GameObject> m_lRacePointClones;
        private List<Waypoint> m_lRacePointScripts;
        private Dictionary<int, int> m_currentWaypoint;
        public Dictionary<int, bool> m_dicOfBools;
        private Dictionary<int, int> m_racePositions;
        private Kojima.GameController m_gameController;
        public Text positionsText;
        public List<GameObject> m_lSortedRacePointPos;
        public bool debugShowPositionsInConsole = false;
        public bool positionAboveHead = false;
        private List<KeyValuePair<int, int>> playerPositionsSorted;
        public List<int> finishedPositions = new List<int>();
        public GameObject[] startGrid;
        private Waypoint wp;

        private List<GameObject> holdList = new List<GameObject>();

        // Use this for initialization

        private new void Start()
        {
             base.Start();

            EventManager.m_instance.SubscribeToEvent(Events.Event.GM_RACE, RaceSetup);


			// Setting up the race position HUD object - I had no better way of getting access to the race script -sam
			for(int i = 0; i < GameController.s_ncurrentPlayers; i++) {
				Bird.HUD_RacePosition racePos = GameController.s_singleton.m_players[i].m_PlayerHUD.GetHUDElement<Bird.HUD_RacePosition>();
				racePos.m_RaceScript = this;
			}

            //SceneManager.LoadScene("Race1Additive", LoadSceneMode.Additive);
            //Initialise everything
            m_lSortedRacePointPos = new List<GameObject>();
            m_lRacePointClones = new List<GameObject>();
            m_lRacePointScripts = new List<Waypoint>();
            m_currentWaypoint = new Dictionary<int, int>();
            m_dicOfBools = new Dictionary<int, bool>();
            m_racePositions = new Dictionary<int, int>();
            m_gameController = FindObjectOfType<Kojima.GameController>();
            finishedPositions.Reverse();

            foreach (GameObject dropPoint in GameObject.FindGameObjectsWithTag("DropPoint"))
            {
                m_lRacePointPos.Add(dropPoint);
            }

            m_lSortedRacePointPos = m_lRacePointPos.OrderBy(go => go.name).ToList();

            //for(int i = 0; i < dpSpawn.dropPointList.Count; i++)
            //{
            //    m_lRacePointPos.Add( dpSpawn.dropPointList[i]);
            //}

            for (int i = 0; i < m_lRacePointPos.Count; i++)
            {
                GameObject newRacePoint = Instantiate(m_originalRacePoint, m_lSortedRacePointPos[i].transform.position, m_lSortedRacePointPos[i].transform.rotation) as GameObject;
                newRacePoint.transform.parent = m_lSortedRacePointPos[i].transform;
                //newRacePoint.transform.localScale = m_lSortedRacePointPos[i].transform.localScale;
                newRacePoint.transform.localScale = new Vector3(1, 1, 1);

                m_lRacePointClones.Add(newRacePoint);
                
                m_lRacePointScripts.Add(m_lRacePointClones[i].transform.GetComponent<Waypoint>());
            }

            //Change the points of the checkpoints to their correct points
            //m_lRacePointScripts[0].setType(RP_Type.START);

            setCheckpoints();

            //Setup the dictionaries
            m_lRacePointScripts[0].setAllVisible(true);
            foreach (Kojima.CarScript player in m_gameController.m_players)
            {
                if (player != null)
                {
                    m_currentWaypoint.Add(player.m_nplayerIndex, 0);
                    m_dicOfBools.Add(player.m_nplayerIndex, false);
                    m_racePositions.Add(player.m_nplayerIndex, 0);
                    for (int i = 0; i < player.m_nplayerIndex; i++)
                    {
                        //player.SetCanMove(false);

                    player.transform.position = startGrid[i].transform.position;
                        player.transform.rotation = startGrid[i].transform.rotation;
                        
                    }
                }
            }
        }

        private void OnDestroy()
        {
            EventManager.m_instance.UnsubscribeToEvent(Events.Event.GM_RACE, RaceSetup);
        }

        private void RaceSetup()
        {
            SceneManager.LoadScene("Race1Additive", LoadSceneMode.Additive);
            
        }

        private new
        // Update is called once per frame
        void Update()
        {
            base.Update();

            //if (m_active)
            //{
            foreach (Kojima.CarScript player in m_gameController.m_players)
            {
                if (player != null)
                {
                    if (m_lRacePointScripts[m_currentWaypoint[player.m_nplayerIndex]]
                            .getPassed(player.m_nplayerIndex) && !m_dicOfBools[player.m_nplayerIndex])
                    {
                        //Set the current (passed) checkpoint visibility to false
                        m_lRacePointScripts[m_currentWaypoint[player.m_nplayerIndex]]
                            .setVisible(player.m_nplayerIndex, false);
                        if (m_lRacePointScripts[m_currentWaypoint[player.m_nplayerIndex]].getType() == RP_Type.START
                            || m_lRacePointScripts[m_currentWaypoint[player.m_nplayerIndex]].getType() ==
                            RP_Type.CHECKPOINT)
                        {
                            //if checkpoint is start or passed, iterate current checkpoint and show
                            Debug.Log("passed START or CHECKPOINT");
                            m_currentWaypoint[player.m_nplayerIndex] += 1;
                            m_lRacePointScripts[m_currentWaypoint[player.m_nplayerIndex]]
                                .setVisible(player.m_nplayerIndex, true);
                        }
                        else if ((m_lRacePointScripts[m_currentWaypoint[player.m_nplayerIndex]].getType() ==
                                  RP_Type.FINISH ||
                                  m_currentWaypoint[player.m_nplayerIndex] >= m_lRacePointScripts.Count) &&
                                 !m_dicOfBools[player.m_nplayerIndex])
                        {
                            //if checkpoint is finish, simply hide and set m_dicOfBools to true
                            Debug.Log("passed FINISH");
                            m_dicOfBools[player.m_nplayerIndex] = true;
                            finished(player.m_nplayerIndex);
                        }
                    }
                }
            }

            updatePostions(); //Don't look insied this function
        }

        //}

        //Update Positions
        private void updatePostions()
        {
            List<KeyValuePair<int, int>> playerRacePositions = new List<KeyValuePair<int, int>>();

            foreach (Kojima.CarScript player in m_gameController.m_players)
            {
                if (player != null)
                {
                    playerRacePositions.Add(new KeyValuePair<int, int>(player.m_nplayerIndex, m_currentWaypoint[player.m_nplayerIndex]));
                }
            }

            playerPositionsSorted = playerRacePositions.OrderBy(x => x.Value).ToList();
            playerPositionsSorted.Reverse();

            List<KeyValuePair<int, int>> samePosSort = new List<KeyValuePair<int, int>>();
            List<KeyValuePair<int, float>> samePosDists = new List<KeyValuePair<int, float>>();

            for (int i = 0; i < playerPositionsSorted.Count; i++)
            {
                //Clear list
                samePosSort.Clear();

                //Add this player
                samePosSort.Add(playerPositionsSorted[i]);

                //Loop through the rest of the players
                for (int j = i + 1; j < playerPositionsSorted.Count; j++)
                {
                    if (playerPositionsSorted[j].Value == playerPositionsSorted[i].Value)
                    {
                        samePosSort.Add(playerPositionsSorted[j]);
                    }
                    else
                    {
                        break;
                    }
                }

                //If multiple are in the same place
                if (samePosSort.Count > 1)
                {
                    //Work out order of players with same waypoint
                    samePosDists.Clear();
                    for (int h = 0; h < samePosSort.Count; h++)
                    {
                        //Get gameobjects
                        int playerIndex = samePosSort[h].Key;
                        Kojima.CarScript playerCarSc = m_gameController.m_players[playerIndex - 1];

                        GameObject playerWaypoint = m_lRacePointClones[m_currentWaypoint[samePosSort[h].Key]];
                        GameObject playerCar = playerCarSc.gameObject;

                        //Calculate Distance
                        float distance = Vector3.Distance(playerCar.transform.position, playerWaypoint.transform.position);

                        samePosDists.Add(new KeyValuePair<int, float>(samePosSort[h].Key, distance));
                    }

                    //Sort distance list by distance
                    samePosDists = samePosDists.OrderBy(x => x.Value).ToList();

                    //Store current waypoint
                    int waypoint = samePosSort[0].Value;

                    //Add sorted distance list back in to unsorted same position list using waypoint
                    for (int h = 0; h < samePosSort.Count; h++)
                    {
                        samePosSort[h] = new KeyValuePair<int, int>(samePosDists[h].Key, waypoint);
                    }

                    //Update old list
                    for (int k = 0; k < samePosSort.Count; k++)
                    {
                        playerPositionsSorted[i + k] = samePosSort[k];
                    }
                }
            }

            for (int i = 0; i < playerPositionsSorted.Count; i++)
            {
                if (debugShowPositionsInConsole)
                {
                    Debug.Log("Player (" + playerPositionsSorted[i].Key + ") is at position: " + i);
                }

                if (!positionAboveHead)
                {
                    int playerNo = playerPositionsSorted[i].Key;
                    Kojima.CarScript playerCarSc = m_gameController.m_players[playerNo - 1];
                    TypogenicText playerCarText = playerCarSc.gameObject.GetComponentInChildren<TypogenicText>();
                    playerCarText.Text = (i + 1).ToString();
                }
            }

            if (finishedPositions.Count == playerPositionsSorted.Count)
            {
                for (int i = 0; i < finishedPositions.Count; i++)
                {
                    Debug.Log("Player (" + finishedPositions[i] + ") has finished at: " + (i + 1) + "st");
                    int playerNo = finishedPositions[i];
                    Kojima.CarScript playerCarSc = m_gameController.m_players[playerNo - 1];
                    TypogenicText playerCarText = playerCarSc.gameObject.GetComponentInChildren<TypogenicText>();
                    playerCarText.Text = (i + 1).ToString();
                    DestroyRace();
                    EndGame();
                    // RaceGameMode rgm;
                    //  rgm = (RaceGameMode)GameModeManager.m_instance.m_currentGameMode;
                    // rgm.EndGame();


                    //SceneManager.UnloadScene("Race1Additive");
                }
            }

            // for (int i = 0; i < finishedPositions.Count; i++)
            // {
            //     if (finishedPositions.Count > 0) Debug.Log("Player (" + playerPositionsSorted[i].Key + ") has finished at: " + finishedPositions[i]);
            // }
        }

        //Add a checkpoint on the fly at the specified index and position
        public void addCheckpoint(Vector3 _pos, Quaternion _rot, int _index = -1)
        {
            if (_index >= 0)
            {
                m_lRacePointClones.Insert(_index, Instantiate(m_originalRacePoint, _pos, _rot) as GameObject);
                m_lRacePointScripts.Insert(_index, m_lRacePointClones[_index].transform.GetComponent<Waypoint>());
            }
            else
            {
                m_lRacePointClones.Add(Instantiate(m_originalRacePoint, _pos, _rot) as GameObject);
                m_lRacePointScripts.Add(m_lRacePointClones[m_lRacePointClones.Count - 1].transform.GetComponent<Waypoint>());
            }
            setCheckpoints();
        }

        //check if the player (at index) has finished
        public bool finished(int _playerIndex)
        {
            //positionAboveHead = true;
            finishedPositions.Add(_playerIndex);
            return m_dicOfBools[_playerIndex];
        }

        private void setCheckpoints()
        {
            for (int i = 1; i < m_lRacePointScripts.Count - 1; i++)
            {
                m_lRacePointScripts[i].setType(RP_Type.CHECKPOINT);
            }
            m_lRacePointScripts[0].setType(RP_Type.START);
            m_lRacePointScripts[m_lRacePointScripts.Count - 1].setType(RP_Type.FINISH);
            for (int i = 0; i < m_lRacePointScripts.Count; i++)
            {
                m_lRacePointScripts[i].SetMat();
            }
        }

        public int GetFirstPerson()
        {
            return playerPositionsSorted[0].Key;
        }

		public int GetPlayerPosition(int nPlayerIdx) {
			for(int i = 0; i < playerPositionsSorted.Count; i++) {
				if(playerPositionsSorted[i].Key == nPlayerIdx) {
					return i + 1;
				}
			}

			return -1;
		}

        public int GetLastPerson()
        {
            return playerPositionsSorted[playerPositionsSorted.Count - 1].Key;
        }

        private void DestroyRace()
        {
            m_lSortedRacePointPos.Clear();
            m_lRacePointPos.Clear();
            m_lRacePointScripts.Clear();
            m_lRacePointClones.Clear();
            finishedPositions.Clear();
            //wp.DestroyPoints();
        }

        //void EndGame()
        //{
        //    //m_active = false;

        //    //Kojima.GameModeManager.m_instance.m_currentMode = Kojima.GameModeManager.GameModeState.FREEROAM;
        //}
    }
}