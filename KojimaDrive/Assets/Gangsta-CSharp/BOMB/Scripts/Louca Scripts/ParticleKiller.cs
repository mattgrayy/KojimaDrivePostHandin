using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class ParticleKiller : MonoBehaviour
    {

        public float m_lifeTime;

        // Use this for initialization
        void Start()
        {
            Destroy(gameObject, m_lifeTime);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}