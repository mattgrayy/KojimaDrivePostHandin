using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GCSharp
{
    public class PowerUpSpawner : MonoBehaviour
    {

        public GameObject m_powerUp;
        private List<Transform> m_spawnPoints;
        private Transform[] m_children;

        // Use this for initialization
        void Start()
        {


            SetUpSpawnPoints();
            SpawnPowerUps();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void SetUpSpawnPoints()
        {
            m_spawnPoints = new List<Transform>();
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                m_spawnPoints.Add(gameObject.transform.GetChild(i).transform);
            }
        }

        void SpawnPowerUps()
        {
            GameObject m_tempGameObjects = (GameObject)Instantiate(m_powerUp, gameObject.transform.position, gameObject.transform.rotation);
            for (int i = 0; i < m_spawnPoints.Count; i++)
            {
                Instantiate(m_powerUp, m_spawnPoints[i].transform.position, m_spawnPoints[i].transform.rotation);
            }
        }
    }
}