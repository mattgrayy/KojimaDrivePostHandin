using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kojima;

namespace GCSharp
{

    public class Bomb_System : MonoBehaviour
    {
        private float m_timer;
        private int m_countdown;
        private List<GameObject> m_spawnPoints, m_players;
        private GameObject m_bomb, m_bombPrefab;
        private GameObject m_bombSpawn;
        Kojima.RaceScript m_raceScript;
        private bool m_bombSpawned = false;
        public bool m_useRandSpawn = true;
        int t_rand;
        private GameObject m_ptbGO;
        private PTBSetUp m_ptbSetUp;
        private bool m_gameOver = false;

        private void Start()
        {
            
        }

        public void Init()
        {
            m_spawnPoints = new List<GameObject>();
            m_players = new List<GameObject>();
            m_timer = m_countdown;
            m_bombSpawned = false;
            m_ptbGO = GameObject.Find("PTBSet Up");
            m_ptbSetUp = m_ptbGO.GetComponent<PTBSetUp>();
        }

        private void Update()
        {
            if (m_bombSpawn == null)
            {
                m_bombSpawn = GameObject.Find("BombSpawnPoint");
            }
            m_timer -= Time.deltaTime;
            if (m_timer < 0 && !m_bombSpawned)
            {
                m_bomb = (GameObject)Instantiate(m_bombPrefab, m_bombSpawn.transform.position, Quaternion.identity);
                //m_bomb.GetComponent<BombScript>().SetNewBombHolder(m_players[t_rand], m_spawnPoints[t_rand]);
                m_bombSpawned = true;
            }
        }

        public void AddSpawnPoint(GameObject _spawnPoint)
        {
            m_spawnPoints.Add(_spawnPoint);
        }

        public void AddPlayer(GameObject _player)
        {
            m_players.Add(_player);
        }

        public void SetCountDown(int _countdown)
        {
            m_countdown = _countdown;
        }

        public void SetBombPrefab(GameObject _bombPrefab)
        {
            m_bombPrefab = _bombPrefab;
        }

        public void SetGameOver()
        {
            m_gameOver = true;
            m_ptbSetUp.GameOver();
        }
    }

}

