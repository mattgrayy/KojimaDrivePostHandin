using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class PowerUpCollisionSystem : MonoBehaviour
    {

        private ParticleSpawner m_particleSpawner;

        public GameObject m_audioObjectOne, m_audioObjectTwo;
        public int m_max, m_num;
        public string m_type;

        // Use this for initialization
        void Start()
        {
            m_particleSpawner = gameObject.GetComponent<ParticleSpawner>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                if (m_type == "SpeedBoost")
                {
                    other.GetComponent<ArcadeCarScript>().SpeedBoost(true);
                }
                m_particleSpawner.SpawnParticle();
                int rand = (int)Random.Range(0, m_max);
                if (rand < m_num)
                {
                    Instantiate(m_audioObjectOne, transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(m_audioObjectTwo, transform.position, Quaternion.identity);
                }
                Destroy(gameObject);
            }
        }

    }
}
