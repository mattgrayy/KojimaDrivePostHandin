using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class RabbitMovement : MonoBehaviour
    {
        //Base Variables
        private Rigidbody m_rb;

        private Vector3 m_OriginLoc;

        public float m_fTimeForce;

        //movement
        private Vector3 m_vDir;
        private Vector3 m_dir;

        //Rabbit Variables
        private float m_force;
        public Vector3 m_vel;

        //ray cast variables
        private Ray m_rabbitRay;
        private Ray m_DownRay;
        private RaycastHit m_hit;
        private RaycastHit m_downhit;


        // Use this for initialization
        void Start()
        {

            m_OriginLoc = transform.position;
            m_rb = GetComponent<Rigidbody>();

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            m_vel = m_rb.velocity;
            Movement();
            
        }

        public void Movement()
        {
            //Chicken movement force
            m_force = Randomfloat(5, 10);


            int m_iRandRot = Random.Range(0, 200);
            if (m_iRandRot == 1)
            {
                float m_fRot = Randomfloat(60, 180);

                gameObject.transform.Rotate(new Vector3(0, m_fRot,0));
            }

            int m_iRandMov = Random.Range(0, 10);
            if (m_iRandMov == 1)
            {
                m_rb.AddForce(new Vector3(20, 20, 0) *m_force );
            }

            // m_vDir = new Vector3(5, RandomDirection(1f, 45f), 0);

        }

        float Randomfloat(float _min, float _max)
        {
            float m_fDir = Random.Range(_min, _max);
            if (Random.Range(0, 2) == 1)
            {
                return m_fDir;
            }
            else
            {
                return -m_fDir;
            }

        }


    }
}
