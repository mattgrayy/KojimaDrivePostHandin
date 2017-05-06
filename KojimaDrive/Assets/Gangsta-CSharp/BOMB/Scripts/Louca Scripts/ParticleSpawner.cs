using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class ParticleSpawner : MonoBehaviour
    {

        public Transform m_spawnPoint;
        public GameObject m_particleEffect;
        public bool m_testing;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (m_testing)
            {
                TestingParticle();
            }
        }

        void TestingParticle()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Instantiate(m_particleEffect, m_spawnPoint.position, m_spawnPoint.rotation);
            }
        }

        public void SpawnParticle()
        {
            Instantiate(m_particleEffect, m_spawnPoint.position, m_spawnPoint.rotation);
        }
    }
}