using UnityEngine;
using System.Collections;
namespace GCSharp
{
    public class RewindPowerUp : MonoBehaviour
    {

        private RewindManager m_rewindManager;
        private GameObject m_rewindManagerGO;
        private ParticleSpawner m_particleSpawner;

        // Use this for initialization
        void Start()
        {
            m_rewindManagerGO = GameObject.FindGameObjectWithTag("RewindManager");
            m_rewindManager = m_rewindManagerGO.GetComponent<RewindManager>();
            m_particleSpawner = GetComponent<ParticleSpawner>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag == "Player")
            {
                m_particleSpawner.SpawnParticle();
                m_rewindManager.SetMode(RewindManager.Mode.Rewind);
                print("collision and now rewinding");
                Destroy(gameObject);
            }
        }
    }
}