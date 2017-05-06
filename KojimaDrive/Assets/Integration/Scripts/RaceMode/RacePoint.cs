using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kojima
{
    public class RacePoint : MonoBehaviour
    {
        public RP_Type types;

        public Transform[] startGrid;
        public GameObject[] playerList;
        public RP_Type startType;
        public Vector3[] gridPositions;
        public List<GameObject> checkPointList = new List<GameObject>();
        public int size;
        public Text checkpointsText;
        public GameObject flagPrefab;
        public bool m_bVisible = false; //If the checkpoint visisble

        public bool m_bPassed =
            false; //Has the point been passed (set to true on first trigger with player with correct ID)

        public int m_playerNumber = 0; //ID of the player that can see/interact with this instance of the checkpoint

        private Renderer rend;

        // Use this for initialization
        private void Start()
        {
            rend = GetComponent<Renderer>();
            Material matCheck = Resources.Load("wpCheckPoint") as Material;
            rend.material = matCheck;
            startType = types;
            ChangeMat();
        }

        // Update is called once per frame
        private void Update()
        {
            rend.enabled = m_bVisible;
            if (this.types != startType)
            {
                ChangeMat();
            }
        }

        public void setVisible(bool _visible = true)
        {
            m_bVisible = _visible;
        }

        private void OnTriggerEnter(Collider col)
        {
            //Process collisions for players
            if (col.gameObject.tag == ("Player"))
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

        public void ChangeMat()
        {
            switch (types)
            {
                case RP_Type.START:
                    Material matStart = Resources.Load("wpStart") as Material;
                    rend.material = matStart;
                    break;

                case RP_Type.FINISH:
                    Material matFinish = Resources.Load("wpFinish") as Material;
                    rend.material = matFinish;
                    break;

                case RP_Type.CHECKPOINT:
                    Material matCheck = Resources.Load("wpCheckPoint") as Material;
                    rend.material = matCheck;
                    break;

                default:
                    //do nothing
                    break;
            }
        }
    }
}