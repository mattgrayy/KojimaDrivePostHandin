using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class Orbiting : MonoBehaviour
    {
        GameObject m_gTornado;
        TornadoMove m_cTornadoMove;
        private Transform m_tCenter;
        private bool m_bTrigger;
        //private Vector3 m_desiredPosition;
        public float m_fRadius = 2.0f;
        public float m_fRadiusSpeed = 0.5f;
        public float m_fRotationSpeed = 500.0f;
        private float m_fMOffset;

        //public float frequency = 20.0f;  // Speed of sine movement
        //public float magnitude = 0.5f;   // Size of sine movement

        void Start()
        {
            m_bTrigger = false;

            m_gTornado = GameObject.FindWithTag("Tornado");
            //Accessing offset setup
            //TornadoMove m_cTornadoMove = m_gTornado.GetComponent<TornadoMove>();
            //m_fMOffset = m_cTornadoMove.m_fOffest;

            m_tCenter = m_gTornado.transform;
        }

        void Update()
        {
            if (m_bTrigger = true)
            {
                m_fRadius = 2.0f;

                //For making objects move within tornado
                //transform.position += Vector3.up * Time.deltaTime;
                //transform.position += magnitude * (Mathf.Sin(2 * Mathf.PI * frequency * Time.time) - Mathf.Sin(2 * Mathf.PI * frequency * (Time.time - Time.deltaTime))) * transform.up;

                transform.RotateAround(m_tCenter.position, Vector3.up, m_fRotationSpeed * Time.deltaTime);
            }
        }

        public void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.tag == "Tornado")
            {
                m_bTrigger = true;
                transform.position = (transform.position - m_tCenter.position).normalized * m_fRadius + m_tCenter.position;
            }
        }
    }
}