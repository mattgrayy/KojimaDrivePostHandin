using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KRace
{
	public class Waypoint : MonoBehaviour
	{

		Dictionary<int, GameObject> m_racePointsGo;	//The four game objects of each waypoint prefab for this waypoint
		Dictionary<int, RacePoint> m_racePoints;	//The four racepoint scripts for each game object
		public GameObject m_racePointPrefab;		//Prefab of racepoint

		Kojima.GameController m_gameController;		//Gamecontroller

		public RP_Type m_pointType = RP_Type.CHECKPOINT;	//The type that the four player race points will be set to

		// Use this for initialization
		void Awake()
		{
			m_gameController = FindObjectOfType<Kojima.GameController>();   //Get game controller

			//Init dictionaries
			m_racePointsGo = new Dictionary<int, GameObject>();
			m_racePoints = new Dictionary<int, RacePoint>();

			//For each player do the following
			foreach (Kojima.CarScript player in m_gameController.m_players)
			{
                if(player != null)
                {
                    m_racePointsGo.Add(player.m_nplayerIndex, Instantiate(m_racePointPrefab));                                              //Create racepoint object
                    m_racePoints.Add(player.m_nplayerIndex, m_racePointsGo[player.m_nplayerIndex].transform.GetComponent<RacePoint>());     //Store RacePoint script
                    m_racePoints[player.m_nplayerIndex].types = m_pointType;                                                                //Set the type to what this is set to
                    m_racePointsGo[player.m_nplayerIndex].layer = player.m_nplayerIndex + 14;                                                //Set the view layer (player ID + 7)
                    m_racePointsGo[player.m_nplayerIndex].transform.position = transform.position;                                          //Move the new point to the location of this object
                    m_racePointsGo[player.m_nplayerIndex].name = transform.name + "WaypointP" + player.m_nplayerIndex;                      //Name the waypoint based on the player name
                    m_racePoints[player.m_nplayerIndex].m_bVisible = false;                                                                  //Set it's visibility
                    m_racePoints[player.m_nplayerIndex].m_playerNumber = player.m_nplayerIndex;												//Set the ID of the player it can interact with
                }
				
			}


		}

		// Update is called once per frame
		void Update()
		{

		}

        //Has a player passed through the specified racepoint
        public bool getPassed(int _nplayerIndex)
        {
            return m_racePoints[_nplayerIndex].m_bPassed;
        }

        //Check if the waypoint is visisble to a specific player
        public bool getVisisble(int _nplayerIndex)
        {
            return m_racePoints[_nplayerIndex].m_bVisible;
        }

        //Set the waypoint visibility for a specified player
        public void setVisible(int _nplayerIndex, bool _visible = true)
        {
            m_racePoints[_nplayerIndex].m_bVisible = _visible;
        }

        //Set the waypoint visiblity for all players
        public void setAllVisible(bool _visible = true)
        {
            foreach (KeyValuePair<int, RacePoint> point in m_racePoints)
            {
                point.Value.setVisible(_visible);
            }
        }

        public void setType(RP_Type _pointType)
        {
            m_pointType = _pointType;
            foreach (KeyValuePair<int, RacePoint> point in m_racePoints)
            {
                point.Value.types = _pointType;
            }
        }

        public RP_Type getType()
        {
            return m_pointType;
        }

    }

}