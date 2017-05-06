using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;


namespace KRace
{
	//sets up our enum for ease of use when making races
	public enum RP_Type
	{
		START, FINISH, CHECKPOINT, DICOFBOOLS
	}

	public class RacePoint : MonoBehaviour
	{
		
		public RP_Type types;

		public Transform[] startGrid;
		public GameObject[] playerList;

		public Vector3[] gridPositions;
		public List<GameObject> checkPointList = new List<GameObject>();
		public int size;
		public Text checkpointsText;

		public bool m_bVisible = false;	//If the checkpoint visisble
		public bool m_bPassed = false;	//Has the point been passed (set to true on first trigger with player with correct ID)

		public int m_playerNumber = 0;	//ID of the player that can see/interact with this instance of the checkpoint

		Renderer rend;

		// Use this for initialization
		void Start()
		{
			

			Material matStart = Resources.Load("wpStart") as Material;
			Material matCheck = Resources.Load("wpCheckPoint") as Material;
			Material matFinish = Resources.Load("wpFinish") as Material;
			rend = GetComponent<Renderer>();
			switch (types)
			{
				case RP_Type.START:
					rend.material = matStart;
					//InitialiseGrid(playerList.Length);
					//gameObject.tag = "StartPoint";
					SpawnAtGrid();
					break;
				case RP_Type.FINISH:
					rend.material = matFinish;
					//gameObject.tag = "FinishPoint";
					break;
				case RP_Type.CHECKPOINT:
					rend.material = matCheck;
					//gameObject.tag = "CheckPoint";

					break;
			}

			//Commented out because tag doesn't exist and it's causing errors - Ryan
			//foreach (GameObject racePoint in GameObject.FindGameObjectsWithTag("CheckPoint"))
			//{

			//	checkPointList.Insert(0, racePoint);
			//}


			


		}

		// Update is called once per frame
		void Update()
		{
			rend.enabled = m_bVisible;
		}


        public void setVisible(bool _visible = true)
        {
            m_bVisible = _visible;
        }

		void SpawnAtGrid()
		{


			for (int i = 0; i < playerList.Length; i++)
			{
				playerList[i].transform.position = gridPositions[i];
				//playerList [i].transform.rotation = startGrid[i].rotation;


			}
		}

		void InitialiseGrid(int gridSize)
		{
			//this sets up where the starting grid positions are from our start point
			gridPositions[0] = (this.transform.position - new Vector3(-2.0f, -0.3f, 0.0f));
			gridPositions[1] = (this.transform.position - new Vector3(-4.0f, -0.3f, 0.0f));
			gridPositions[2] = (this.transform.position - new Vector3(-6.0f, -0.3f, 0.0f));
			gridPositions[3] = (this.transform.position - new Vector3(-8.0f, -0.3f, 0.0f));



			//this loop creates the starting grid as children of the start point so it inherits its transform
			for (int i = 0; i < gridSize; i++)
			{
				GameObject gp = Instantiate(Resources.Load("GridPoint") as GameObject, gridPositions[i], this.transform.rotation) as GameObject;
				gp.transform.parent = this.transform;
			}

		}

		void OnTriggerEnter(Collider col)
		{
			//Process collisions for players
			if(col.gameObject.tag == ("Player"))
			{
				//Get the player number
				int nPlayerIndex = col.gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex;

				//Only do something if the player that collided is the player that can see this checkpoint
				if (nPlayerIndex == m_playerNumber)
				{
					if (!m_bPassed && m_bVisible)
					{
						m_bPassed = true;
					}

				}
			}


			

			
		}


		void CheckPointCount(int checkNum)
		{

			//checkpointsText.text = checkNum.ToString();
		}


	}

}