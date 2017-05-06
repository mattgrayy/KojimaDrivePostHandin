using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GCSharp
{
    public class Boulders : MonoBehaviour
    {

        public Transform[] m_spawnHazard;
        public GameObject m_hazard;
        public List<GameObject> m_spawnedHazards;

        private bool m_isTriggered;

        // Use this for initialization
        void Start()
        {
            m_isTriggered = false;
        }

        // Update is called once per frame
        void Update()
        {
            SpawnHazards();
        }

        public void Trigger(bool _isTriggered)
        {
            m_isTriggered = _isTriggered;
        }

        void SpawnHazards()
        {
            if (m_isTriggered)
            {
                print("boulders spawning");
                for (int i = 0; i < m_spawnHazard.Length; i++)
                {
                    GameObject m_tempGameObjects = (GameObject)Instantiate(m_hazard, m_spawnHazard[i].transform.position, m_spawnHazard[i].transform.rotation);
                }
                m_isTriggered = false;
            }
        }
    }
}