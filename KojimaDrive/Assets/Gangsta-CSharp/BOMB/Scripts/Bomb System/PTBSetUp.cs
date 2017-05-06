using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bam;
using Bird;
using Kojima;
using UnityEngine.SceneManagement;

namespace GCSharp
{
    public class PTBSetUp : MonoBehaviour
    {
        public GameObject m_bombSystemPrefab, m_bombPrefab;
        public List<int> m_matIDs;
        public int m_countdown;
        public Material m_bhMat;
        private GameObject[] m_players;
        public GameObject m_arrowPref;
        public float m_yOffsetNormal = 1.2f;
        public float m_yOffsetIce, m_yOffsetZuk, m_yOffsetRv;

        [SerializeField]
        private List<GameObject> m_spawnPoints;

        [SerializeField]
        private List<GameObject> m_playerArrows;

        private GameObject m_bombSpawner;
        private Bomb_System m_bombSystemScript;

        private GameObject m_arrowManager;
        private Bird.CheckpointManager m_checkpointManager;

        // Use this for initialization
        private void Start()
        {
            //Init();
        }

        public void Init()
        {
            m_spawnPoints = new List<GameObject>();
            m_playerArrows = new List<GameObject>();
            m_matIDs = new List<int>();

            m_checkpointManager = CheckpointManager.s_pCheckpointManager;
            m_checkpointManager.c_Type = CheckpointManager.CheckpointType.LIST;

            m_players = GameObject.FindGameObjectsWithTag("Player");
            GetMatIDs();
            PlayerSetUp();
            BombSpawnerSetUp();
        }

        private void GetMatIDs()
        {
            // Need to modify this so that maybe it reads in from a file or
            //theres a script already attached to the cars that hold there mat id
            for (int i = 0; i < m_players.Length; i++)
            {
                int t_id = m_players[i].GetComponent<MaterialID>().GetID();
                m_matIDs.Add(t_id);
            }
        }

        private void PlayerSetUp()
        {
            for (int i = 0; i < m_players.Length; i++)
            {
                // Create a bomb point
                GameObject t_bombpoint = new GameObject("Bomb Point");

                // Set up bomb point to be a child of the player
                t_bombpoint.transform.parent = m_players[i].transform;
                print(m_players[i].GetComponent<Kojima.CarData>().m_type);
                if (m_players[i].GetComponent<Kojima.CarData>().m_type == CarSwapManager.CarType.ICECREAM)
                {
                    Vector3 t_bombPointPos = new Vector3(m_players[i].transform.position.x,
                        m_players[i].transform.position.y + m_yOffsetIce, m_players[i].transform.position.z);
                    t_bombpoint.transform.position = t_bombPointPos;
                }
                else if (m_players[i].GetComponent<Kojima.CarData>().m_type == CarSwapManager.CarType.RV)
                {
                    Vector3 t_bombPointPos = new Vector3(m_players[i].transform.position.x,
                        m_players[i].transform.position.y + m_yOffsetRv, m_players[i].transform.position.z);
                    t_bombpoint.transform.position = t_bombPointPos;
                }
                else if (m_players[i].GetComponent<Kojima.CarData>().m_type == CarSwapManager.CarType.ZUK)
                {
                    Vector3 t_bombPointPos = new Vector3(m_players[i].transform.position.x,
                        m_players[i].transform.position.y + m_yOffsetZuk, m_players[i].transform.position.z);
                    t_bombpoint.transform.position = t_bombPointPos;
                }
                else
                {
                    Vector3 t_bombPointPos = new Vector3(m_players[i].transform.position.x,
                        m_players[i].transform.position.y + m_yOffsetNormal, m_players[i].transform.position.z);
                    t_bombpoint.transform.position = t_bombPointPos;
                }
                // Store bomb point
                m_spawnPoints.Add(t_bombpoint);

                //GameObject t_newArrow = (GameObject)Instantiate(m_arrowPref, m_players[i].transform.position, Quaternion.identity);
                //Vector3 t_arrowPos = new Vector3(m_players[i].transform.position.x, m_players[i].transform.position.y + 4f, m_players[i].transform.position.z);
                //t_newArrow.transform.position = t_arrowPos;
                //t_newArrow.transform.parent = m_players[i].transform;
                //m_playerArrows.Add(t_newArrow);

                // Set up material changer script
                m_players[i].AddComponent<MaterialChanger>();
                GameObject t_body = m_players[i].GetComponent<Kojima.CarScript>().GetCarBody();
                m_players[i].GetComponent<MaterialChanger>().SetBody(t_body);
                m_players[i].GetComponent<MaterialChanger>().SetMatID(m_matIDs[i]);
                m_players[i].GetComponent<MaterialChanger>().SetBHMat(m_bhMat);

                // Set up bomb pass script
                m_players[i].AddComponent<BombPass>();
                m_players[i].GetComponent<BombPass>().SetBombPoint(t_bombpoint);

                // Set up score
                m_players[i].AddComponent<Score>();
            }
        }

        public void BombSpawnerSetUp()
        {
            m_bombSpawner = (GameObject)Instantiate(m_bombSystemPrefab);
            m_bombSystemScript = m_bombSpawner.GetComponent<Bomb_System>();
            m_bombSystemScript.SetCountDown(m_countdown);
            m_bombSystemScript.Init();
            m_bombSystemScript.SetBombPrefab(m_bombPrefab);
            for (int i = 0; i < m_spawnPoints.Count; i++)
            {
                m_bombSystemScript.AddPlayer(m_players[i]);
                m_bombSystemScript.AddSpawnPoint(m_spawnPoints[i]);
            }
        }

        public void GameOver()
        {
            Destroy(m_bombSpawner);
            gameObject.GetComponent<PTBGameMode>().EndGame();
        }

        public void Cleaner()
        {
            Destroy(m_bombSpawner);

            GameObject[] t_players = GameObject.FindGameObjectsWithTag("Player");

            for (int i = 0; i < t_players.Length; i++)
            {
                Destroy(m_spawnPoints[i]);
                //Destroy(m_playerArrows[i]);
                if (t_players[i].GetComponent<Score>())
                {
                    Destroy(t_players[i].GetComponent<Score>());
                }
                Destroy(t_players[i].GetComponent<MaterialChanger>());
                Destroy(t_players[i].GetComponent<BombPass>());
            }
        }
    }
}