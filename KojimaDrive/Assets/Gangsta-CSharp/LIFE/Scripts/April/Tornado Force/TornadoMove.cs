using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class TornadoMove : MonoBehaviour
    {

        private bool m_bDirRight = true;
        public float m_fOffest;
        public float m_fSpeed = 50.0f;

        void Update()
        {
            if (m_bDirRight)
            {
                transform.Translate(Vector2.right * m_fSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(-Vector2.right * m_fSpeed * Time.deltaTime);
            }
            if (transform.position.x >= 40.0f)
            {
                m_bDirRight = false;
                m_fOffest = -5.0f;
            }
            if (transform.position.x <= -40.0f)
            {
                m_bDirRight = true;
                m_fOffest = 5.0f;
            }
        }
    }
}

       