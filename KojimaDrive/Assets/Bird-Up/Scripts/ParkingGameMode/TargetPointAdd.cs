using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bird
{
    public class TargetPointAdd : MonoBehaviour
    {
        float countdown = 1;
        public float currentCountDown;
        Kojima.GameController GC;
        public List<GameObject> objectsInSpace;
        Parkour baseParking;
        Color myCol;
        void Start()
        {
            baseParking = GameObject.Find("ParkingGameMode").GetComponent<Parkour>();
            myCol = GetComponent<Renderer>().material.color;
            currentCountDown = countdown;
            objectsInSpace = new List<GameObject>();
            GC = FindObjectOfType<Kojima.GameController>();
        }

        void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.tag == "Player")
            {
                objectsInSpace.Add(other.gameObject);
            }
        }

        void OnCollisionExit(Collision other)
        {
            if (other.gameObject.tag == "Player")
            {
                for(int i = 0; i < objectsInSpace.Count;i++)
                {
                    if(objectsInSpace[i] == other.gameObject)
                    {
                        objectsInSpace.Remove(objectsInSpace[i]);
                        break;
                    }

                }
            }
        }


        void Update()
        {
            if (baseParking.currentState == Parkour.ParkingState.PLAYING)
            {
                currentCountDown -= Time.deltaTime;
                if (currentCountDown < 0)
                {

                    currentCountDown = countdown;
                    testDistance();
                    //foreach(GameObject go in objectsInSpace)
                    //{
                    //    go.GetComponent<ParkingGameModeTracker>().addScore(10);
                    //}
                }
            }
        }


        void testDistance()
        {
            float dist = 0;
            int count = 0;
            int closest= -1;
            float closestDist = 99;
            for(int i = 0; i < GC.m_players.Length; i++)
            {
                if (GC.m_players[i] != null)
                {
                    dist = Vector3.Distance(GC.m_players[i].transform.position, transform.position);
                    if (dist < 10)
                    {
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            closest = count;
                        }
                    }
                    count++;
                }
            }
            if(closest != -1)
            {
                Color newCol = Color.white;
                switch(closest)
                {
                    case 0:
                        newCol = Color.red;                                
                            HF.PlayerExp.AddEXP(0, 10);        
                        break;
                    case 1:
                        newCol = Color.blue;
                        HF.PlayerExp.AddEXP(1, 10);
                        break;
                    case 2:
                        newCol = Color.green;
                        HF.PlayerExp.AddEXP(2, 10);
                        break;
                    case 3:
                        newCol = Color.yellow;
                        HF.PlayerExp.AddEXP(3, 10);
                        break;
                }
                transform.parent.GetComponent<Renderer>().material.color = newCol;


                GC.m_players[closest].gameObject.GetComponent<ParkingGameModeTracker>().addScore(10);
            }
            else
            {
                transform.parent.GetComponent<Renderer>().material.color = myCol;
            }


        }

    }
}