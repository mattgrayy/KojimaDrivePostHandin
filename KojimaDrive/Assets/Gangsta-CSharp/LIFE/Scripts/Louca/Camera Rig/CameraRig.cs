using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GCSharp
{
    public class CameraRig : MonoBehaviour
    {

        [SerializeField]
        private GameObject m_camera;
        public bool m_UseMainCamera = true, m_looping = false;
        [SerializeField]
        private List<Transform> m_rigPoints;
        [SerializeField]
        private int m_counter;
        public float m_maxDist;
        private float m_moveSpeed, m_rotSpeed;
        private bool m_runRig;

        // Use this for initialization
        void Start()
        {
            m_runRig = false;
            if (m_UseMainCamera)
            {
                m_camera = Camera.main.gameObject;
            }
            int m_numRigPoints = gameObject.transform.childCount;
            for (int i = 0; i < m_numRigPoints; i++)
            {
                m_rigPoints.Add(gameObject.transform.GetChild(i));
            }
            m_counter = 0;
            m_camera.transform.position = m_rigPoints[m_counter].position;
            m_camera.transform.rotation = m_rigPoints[m_counter].rotation;
            m_moveSpeed = m_rigPoints[m_counter].GetComponent<RigPoints>().GetMoveSpeed();
            m_rotSpeed = m_rigPoints[m_counter].GetComponent<RigPoints>().GetRotSpeed();
            //StartCameraRig();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_runRig)
            {
                RunRig();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_runRig = true;
            }
        }

        void RunRig()
        {
            print("starting rig");
            print("Dist: " + Vector3.Distance(m_camera.transform.position, m_rigPoints[m_counter].position));
            print("Count: " + m_rigPoints.Count);
            if (Vector3.Distance(m_camera.transform.position, m_rigPoints[m_counter].position) <= m_maxDist)
            {
                m_moveSpeed = m_rigPoints[m_counter].GetComponent<RigPoints>().GetMoveSpeed();
                m_rotSpeed = m_rigPoints[m_counter].GetComponent<RigPoints>().GetRotSpeed();
                m_counter += 1;
                print("reached point");
            }
            if (m_counter >= m_rigPoints.Count)
            {
                if (!m_looping)
                {
                    m_runRig = false;
                    m_counter = 0;
                }
                else
                {
                    m_counter = 0;
                    //m_camera.transform.position = m_rigPoints[m_counter].position;
                    //m_camera.transform.rotation = m_rigPoints[m_counter].rotation;
                    //m_moveSpeed = m_rigPoints[m_counter].GetComponent<RigPoints>().GetMoveSpeed();
                    //m_rotSpeed = m_rigPoints[m_counter].GetComponent<RigPoints>().GetRotSpeed();
                    m_runRig = true;
                }
            }
            m_camera.transform.position = Vector3.MoveTowards(m_camera.transform.position, m_rigPoints[m_counter].position, Time.deltaTime * m_moveSpeed);
            m_camera.transform.rotation = Quaternion.RotateTowards(m_camera.transform.rotation, m_rigPoints[m_counter].rotation, Time.deltaTime * m_rotSpeed);
        }

        public void StartCameraRig()
        {
            m_runRig = true;
        }
    }
}