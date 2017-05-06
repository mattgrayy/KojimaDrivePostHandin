using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bird;

namespace GCSharp
{
    public class LBCheckpointManager : MonoBehaviour
    {
        private List<GameObject> m_checkpoints;
        private GameObject m_currentCheckpoint;
        public LBMode m_lbMode;
        public float m_gameTime;
        private float m_timer;
        private GameObject[] m_players;
        //private List<GameObject> m_arrows;

        // Use this for initialization
        private void Start()
        {
            m_timer = 0f;
            //m_arrows = new List<GameObject>();
            m_players = GameObject.FindGameObjectsWithTag("Player");
            //for (int i = 0; i < m_players.Length; i++)
            //{
            //    GameObject t_arrow = m_players[i].transform.FindChild("Arrow(Clone)").gameObject;
            //    m_arrows.Add(t_arrow);
            //}

            m_checkpoints = new List<GameObject>();
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                m_checkpoints.Add(gameObject.transform.GetChild(i).gameObject);
                gameObject.transform.GetChild(i).gameObject.GetComponent<LBCheckpoint>().SetManager(gameObject);
            }
            GetNewCheckpoint();
        }

        // Update is called once per frame
        private void Update()
        {
            m_timer += Time.deltaTime;
            if (m_timer >= m_gameTime)
            {
                m_lbMode.EndGame();
            }
        }

        public void CheckPointReached()
        {
            print("Getting new cp");
            GetNewCheckpoint();
        }

        public GameObject GetCurrent()
        {
            return m_currentCheckpoint;
        }

        private void GetNewCheckpoint()
        {
            int t_rand = Random.Range(0, m_checkpoints.Count);
            if (m_currentCheckpoint == null)
            {
                m_currentCheckpoint = m_checkpoints[t_rand];
                m_currentCheckpoint.AddComponent<Bird.Checkpoint>();
                m_currentCheckpoint.GetComponent<LBCheckpoint>().TurnParticleOn();
                //for (int i = 0; i < m_arrows.Count; i++)
                //{
                //    m_arrows[i].GetComponent<Bam.ArrowScript>().m_target = m_currentCheckpoint;
                //}
            }
            else
            {
                if (m_checkpoints[t_rand] != m_currentCheckpoint)
                {
                    Destroy(m_currentCheckpoint.GetComponent<Bird.Checkpoint>());
                    m_currentCheckpoint = m_checkpoints[t_rand];
                    m_currentCheckpoint.GetComponent<LBCheckpoint>().TurnParticleOn();

                    m_currentCheckpoint.AddComponent<Bird.Checkpoint>();
                    //for (int i = 0; i < m_arrows.Count; i++)
                    //{
                    //    m_arrows[i].GetComponent<Bam.ArrowScript>().m_target = m_currentCheckpoint;
                    //}
                }
                else
                {
                    GetNewCheckpoint();
                }
            }
        }

        public float GetTimer()
        {
            return m_timer;
        }
    }
}