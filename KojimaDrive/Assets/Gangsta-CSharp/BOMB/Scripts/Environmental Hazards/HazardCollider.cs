using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class HazardCollider : MonoBehaviour
    {

        private ParticleSpawner m_particleSpawner;
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
                if (m_type == "Boulder")
                {
                    other.GetComponent<Boulders>().Trigger(true);
                    print("boulders triggered");
                }
                //if (m_type == "Snare")
                //{
                //other.GetComponent<Snare>().Trigger(true);
                //print("snare triggered");
                //}
                //if (m_type == "Blockade")
                //{
                //other.GetComponent<Blockades>().Trigger(true);
                //print("blockades triggered");
                //}
                //if (m_type == "Barrel")
                //{
                //other.GetComponent<Barrels>().Trigger(true);
                //print("barrels triggered");
                //}

                m_particleSpawner.SpawnParticle();
                Destroy(gameObject);

            }
        }
    }
}