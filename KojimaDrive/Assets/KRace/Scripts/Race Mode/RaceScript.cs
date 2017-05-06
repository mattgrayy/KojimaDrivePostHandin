using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace KRace
{
	public class RaceScript : MonoBehaviour
	{
        public GameObject m_originalRacePoint;
        public List<Vector3> m_lRacePointPos;
        List<GameObject> m_lRacePointClones;
        List<Waypoint> m_lRacePointScripts;
        Dictionary<int, int> m_currentWaypoint;
        Dictionary<int, bool> m_dicOfBools;
        Dictionary<int, int> m_racePositions;
        Kojima.GameController m_gameController;
        public Text positionsText;

        public bool debugShowPositionsInConsole = false;
        public bool positionAboveHead = false;

        // Use this for initialization
        void Start()
		{
            //Initialise everything
            m_lRacePointClones = new List<GameObject>();
            m_lRacePointScripts = new List<Waypoint>();
            m_currentWaypoint = new Dictionary<int, int>();
            m_dicOfBools = new Dictionary<int, bool>();
            m_racePositions = new Dictionary<int, int>();
            m_gameController = FindObjectOfType<Kojima.GameController>();

            for (int i = 0; i < m_lRacePointPos.Count; i++)
            {
                m_lRacePointClones.Add(Instantiate(m_originalRacePoint, m_lRacePointPos[i], Quaternion.Euler(0, 0, 0)) as GameObject);
                m_lRacePointScripts.Add(m_lRacePointClones[i].transform.GetComponent<Waypoint>());              
            }

            //Change the points of the checkpoints to their correct points
            m_lRacePointScripts[0].setType(RP_Type.START);
            for (int i = 1; i < m_lRacePointScripts.Count - 1; i++)
            {
                m_lRacePointScripts[i].setType(RP_Type.CHECKPOINT);
            }
            m_lRacePointScripts[m_lRacePointScripts.Count - 1].setType(RP_Type.FINISH);

            //Setup the dictionaries
            m_lRacePointScripts[0].setAllVisible(true);
            foreach (Kojima.CarScript player in m_gameController.m_players)
            {
                if (player != null)
                {
                    m_currentWaypoint.Add(player.m_nplayerIndex, 0);
                    m_dicOfBools.Add(player.m_nplayerIndex, false);
                    m_racePositions.Add(player.m_nplayerIndex, 0);
                }
            }   
        }

        // Update is called once per frame
        void Update()
        {
            foreach (Kojima.CarScript player in m_gameController.m_players)
            {
                if (player != null)
                {
                    if (m_lRacePointScripts[m_currentWaypoint[player.m_nplayerIndex]].getPassed(player.m_nplayerIndex) && !m_dicOfBools[player.m_nplayerIndex])
                    {
                        //Set the current (passed) checkpoint visibility to false
                        m_lRacePointScripts[m_currentWaypoint[player.m_nplayerIndex]].setVisible(player.m_nplayerIndex, false);
                        if (m_lRacePointScripts[m_currentWaypoint[player.m_nplayerIndex]].getType() == RP_Type.START
                            || m_lRacePointScripts[m_currentWaypoint[player.m_nplayerIndex]].getType() == RP_Type.CHECKPOINT)
                        {
                            //if checkpoint is start or passed, iterate current checkpoint and show
                            Debug.Log("passed START or CHECKPOINT");
                            m_currentWaypoint[player.m_nplayerIndex] += 1;
                            m_lRacePointScripts[m_currentWaypoint[player.m_nplayerIndex]].setVisible(player.m_nplayerIndex, true);
                        }
                        else if ((m_lRacePointScripts[m_currentWaypoint[player.m_nplayerIndex]].getType() == RP_Type.FINISH || m_currentWaypoint[player.m_nplayerIndex] >= m_lRacePointScripts.Count) && !m_dicOfBools[player.m_nplayerIndex])
                        {
                            //if checkpoint is finish, simply hide and set m_dicOfBools to true
                            Debug.Log("passed FINISH");
                            m_dicOfBools[player.m_nplayerIndex] = true;
                            positionsText.text = "finish!";
                        }
                    }

                }

            }

            updatePostions(); //Don't look insied this function

        }


        //Update Positions
        void updatePostions()
        {
            List<KeyValuePair<int, int>> playerRacePositions = new List<KeyValuePair<int, int>>();

            foreach (Kojima.CarScript player in m_gameController.m_players)
            {
                if (player != null)
                {
                    playerRacePositions.Add(new KeyValuePair<int, int>(player.m_nplayerIndex, m_currentWaypoint[player.m_nplayerIndex]));
                }
            }

            List<KeyValuePair<int, int>> playerPositionsSorted = playerRacePositions.OrderBy(x => x.Value).ToList();
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




                if (positionAboveHead)
                {
                    int playerNo = playerPositionsSorted[i].Key;
                    Kojima.CarScript playerCarSc = m_gameController.m_players[playerNo - 1];
                    TypogenicText playerCarText = playerCarSc.gameObject.GetComponentInChildren<TypogenicText>();
                    playerCarText.Text = (i + 1).ToString();
                }


            }
        }


        //Add a checkpoint on the fly at the specified index and position
        void addCheckpoint(int _index, Vector3 _pos)
        {
            m_lRacePointClones.Insert(_index, Instantiate(m_originalRacePoint, _pos, Quaternion.Euler(0, 0, 0)) as GameObject);
        }

        //check if the player (at index) has finished
        bool finished(int _playerIndex)
        {
            return m_dicOfBools[_playerIndex];
        }
	}
}