using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using Kojima;

namespace GCSharp
{
    public class GodCamera : MonoBehaviour
    {

        private Rewired.Player m_rewiredPlayer;
        public GameObject m_rewiredGO;

        public enum CameraStates
        {
            FREE,
            POINTS,
            SPLINE,
            CAR
        }

        private CameraStates m_cameraState = CameraStates.FREE;
        private CameraStates m_prevCamState = CameraStates.FREE;
        public GameObject[] m_pointsOfInterest, m_splinePoints;
        private GameObject m_currentPoint, m_nextPoint, m_previousPoint;
        private int m_currentPointID;
        private bool m_pointsSetUp;
        public GameObject m_godCamera, m_debugCameraPref;
        public bool m_enableDebugCam = true;

        public GameObject m_rigPointsParent;
        public bool m_looping = false;
        private bool m_splineSetUp;
        [SerializeField]
        private List<Transform> m_rigPoints;
        [SerializeField]
        private int m_counter;
        public float m_maxDist;
        private float m_moveSpeed, m_rotSpeed;

        public float mouseSensitivity = 5.0f;
        private float verticalRotation = 0;

        private GameObject[] m_allPlayers;
        private List<GameObject> m_allPlayerMinusLast;
        public List<GameObject> m_rotPoints;
        public Vector3 m_camOffsetForCars;
        private GameObject m_currentCar, m_nextCar, m_previousCar;
        private int m_currentCarID;

        float t_zRot = 0;

        public bool m_disableOnStart = false;

        // Use this for initialization
        void Start()
        {
            if (m_disableOnStart)
            {
                this.enabled = false;
            }
            
            m_rewiredPlayer = gameObject.GetComponent<Kojima.CarScript>().GetRewiredPlayer();
            int t_controllerID = gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex;
            if (t_controllerID != 1 /*m_allPlayers.Length - 1*/)
            {
                this.enabled = false;

            }
            if (m_godCamera == null && t_controllerID == 1)
            {
                m_godCamera = (GameObject)Instantiate((m_debugCameraPref));
                m_godCamera.transform.rotation = Quaternion.identity;
            }
            //POI set up
            m_currentPointID = 0;
            m_currentPoint = m_pointsOfInterest[m_currentPointID];
            m_nextPoint = m_pointsOfInterest[m_currentPointID + 1];
            m_previousPoint = m_pointsOfInterest[m_pointsOfInterest.Length - 1];

            //spline set up
            m_splineSetUp = false;
            if (m_rigPointsParent != null)
            {
                int m_numRigPoints = m_rigPointsParent.transform.childCount;
                for (int i = 0; i < m_numRigPoints; i++)
                {
                    m_rigPoints.Add(m_rigPointsParent.transform.GetChild(i));
                }
                m_counter = 0;
            }

            //car set up
            m_currentCarID = 0;
            m_allPlayerMinusLast = new List<GameObject>();
            m_rotPoints = new List<GameObject>();
            m_allPlayers = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < m_allPlayers.Length; i++)
            {
                GameObject t_rotPoint = new GameObject("Rot Point");
                t_rotPoint.transform.position = m_allPlayers[i].transform.position;
                t_rotPoint.transform.parent = m_allPlayers[i].transform;
                m_rotPoints.Add(t_rotPoint);
            }
            m_nextCar = m_rotPoints[m_currentCarID];
            m_previousCar = m_rotPoints[m_rotPoints.Count-1];

            
        }

        // Update is called once per frame
        void Update()
        {
            //print("Camera Enabled: " + m_godCamera.GetComponent<Camera>().enabled);
            if (m_rewiredPlayer == null)
            {
                m_rewiredPlayer = gameObject.GetComponent<Kojima.CarScript>().GetRewiredPlayer();
            }
            if (m_rewiredPlayer != null)
            {
                if (m_rewiredPlayer.GetButtonDown("ActivateDebugCam"))
                {
                    m_enableDebugCam = !m_enableDebugCam;
                }
            }
            if (!m_enableDebugCam)
            {
                m_godCamera.GetComponent<Camera>().enabled = false;
            }
            if (m_enableDebugCam)
            {
                if (!m_godCamera.GetComponent<Camera>().enabled)
                {
                    m_godCamera.GetComponent<Camera>().enabled = true;
                }
                 
                CheckForStateChange();
                if (m_cameraState == CameraStates.POINTS)
                {
                    PointOfInterestMode();
                }
                if (m_cameraState == CameraStates.SPLINE)
                {
                    SplineMode();
                }
                if (m_cameraState == CameraStates.FREE)
                {
                    FreeMode();
                }
                if (m_cameraState == CameraStates.CAR)
                {
                    CarMode();
                }
            }
        }

        void CheckForStateChange()
        {
            if (m_rewiredPlayer.GetButtonDown("Change Camera"))
            {
                
                m_cameraState++;
                print(m_cameraState);
                if (m_cameraState == CameraStates.POINTS)
                {
                    m_pointsSetUp = false;
                }
                if (m_cameraState == CameraStates.CAR)
                {
                    MoveToNextCar();
                }
                
                if ((int)m_cameraState > 3)
                {
                    m_cameraState = CameraStates.FREE;
                    m_godCamera.transform.parent = null;
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                if (m_cameraState != CameraStates.POINTS)
                {
                    m_prevCamState = m_cameraState;
                    m_cameraState = CameraStates.POINTS;
                    m_pointsSetUp = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                if (m_cameraState != CameraStates.SPLINE)
                {
                    m_prevCamState = m_cameraState;
                    m_cameraState = CameraStates.SPLINE;
                    m_splineSetUp = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                if (m_cameraState != CameraStates.FREE)
                {
                    m_prevCamState = m_cameraState;
                    m_cameraState = CameraStates.FREE;
                }
            }
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                if (m_cameraState != CameraStates.CAR)
                {
                    m_prevCamState = m_cameraState;
                    m_cameraState = CameraStates.CAR;
                }
            }
        }

        void PointsSetUp()
        {
            m_currentPointID = 0;
            m_currentPoint = m_pointsOfInterest[m_currentPointID];
            m_nextPoint = m_pointsOfInterest[m_currentPointID + 1];
            m_previousPoint = m_pointsOfInterest[m_pointsOfInterest.Length - 1];
            SetToClosestPOI();
        }

        void PointOfInterestMode()
        {
            if (!m_pointsSetUp)
            {
                PointsSetUp();
                m_pointsSetUp = true;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                MoveToPrevPoint();
            }
            if (m_rewiredPlayer.GetButtonDown("Action"))
            {
                MoveToNextPoint();
            }

            Vector3 t_rotation = new Vector3(-m_rewiredPlayer.GetAxis("Look Vertical"), m_rewiredPlayer.GetAxis("Look Horizontal"), 0f);
            m_godCamera.gameObject.transform.localEulerAngles += t_rotation * mouseSensitivity * Time.deltaTime;
        }

        void SetToClosestPOI()
        {
            m_godCamera.transform.position = m_currentPoint.transform.position;
        }

        void MoveToNextPoint()
        {
            m_godCamera.transform.position = m_nextPoint.transform.position;
            m_previousPoint = m_currentPoint;
            m_currentPoint = m_nextPoint;
            m_currentPointID++;
            if (m_currentPointID >= m_pointsOfInterest.Length)
            {
                m_currentPointID = 0;
            }
            m_nextPoint = m_pointsOfInterest[m_currentPointID];
        }

        void MoveToPrevPoint()
        {
            m_godCamera.transform.position = m_previousPoint.transform.position;
            m_nextPoint = m_currentPoint;
            m_currentPoint = m_previousPoint;
            m_currentPointID--;
            if (m_currentPointID < 0)
            {
                m_currentPointID = m_pointsOfInterest.Length - 1;
            }
            m_previousPoint = m_pointsOfInterest[m_currentPointID];
        }

        void SplineMode()
        {
            if (!m_splineSetUp)
            {
                m_godCamera.transform.position = m_rigPoints[m_counter].position;
                m_godCamera.transform.rotation = m_rigPoints[m_counter].rotation;
                m_moveSpeed = m_rigPoints[m_counter].GetComponent<RigPoints>().GetMoveSpeed();
                m_rotSpeed = m_rigPoints[m_counter].GetComponent<RigPoints>().GetRotSpeed();
                m_splineSetUp = true;
            }
            else
            {
                if (Vector3.Distance(m_godCamera.transform.position, m_rigPoints[m_counter].position) <= m_maxDist)
                {
                    m_moveSpeed = m_rigPoints[m_counter].GetComponent<RigPoints>().GetMoveSpeed();
                    m_rotSpeed = m_rigPoints[m_counter].GetComponent<RigPoints>().GetRotSpeed();
                    m_counter += 1;
                    print("reached point");
                }
                if (m_counter >= m_rigPoints.Count)
                {
                    if (m_looping)
                    {
                        m_counter = 0;
                    }
                }
                m_godCamera.transform.position = Vector3.MoveTowards(m_godCamera.transform.position, m_rigPoints[m_counter].position, Time.deltaTime * m_moveSpeed);
                m_godCamera.transform.rotation = Quaternion.RotateTowards(m_godCamera.transform.rotation, m_rigPoints[m_counter].rotation, Time.deltaTime * m_rotSpeed);
            }
        }

        void FreeMode()
        {
            if (m_rewiredPlayer.GetButtonDown("Look Behind"))
            {
                t_zRot += 1.0f;
            }
            if (m_rewiredPlayer.GetButtonDown("Grapple"))
            {
                t_zRot -= 1.0f;
            }
            Vector3 t_rotation = new Vector3(-m_rewiredPlayer.GetAxis("Look Vertical"), m_rewiredPlayer.GetAxis("Look Horizontal"), t_zRot);
            m_godCamera.gameObject.transform.localEulerAngles += t_rotation * mouseSensitivity * Time.deltaTime;

            float moveHorizontal = m_rewiredPlayer.GetAxis("Move Horizontal");
            float moveVertical = m_rewiredPlayer.GetAxis("Move Vertical");
            float moveUpDown = Input.GetAxis("MoveDebug");

            Vector3 movement = new Vector3(moveHorizontal, moveUpDown, moveVertical);

            movement = m_godCamera.gameObject.transform.rotation * movement;
            m_godCamera.gameObject.transform.position += movement;
        }

        void CarMode()
        {
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                MoveToPrevCar();
            }
            if (m_rewiredPlayer.GetButtonDown("Action"))
            {
                MoveToNextCar();
            }
            m_godCamera.transform.LookAt(m_currentCar.transform);
            
            Vector3 t_rotation = new Vector3(-m_rewiredPlayer.GetAxis("Look Vertical"), -m_rewiredPlayer.GetAxis("Look Horizontal"), 0f);
            
            int t_currentID = m_currentCarID - 1;
            if (t_currentID < 0)
            {
                t_currentID = m_rotPoints.Count - 1;
            }
            print(t_currentID);
            m_rotPoints[t_currentID].gameObject.transform.localEulerAngles += t_rotation * mouseSensitivity * Time.deltaTime;
        }

        void MoveToNextCar()
        {
            m_godCamera.transform.position = m_nextCar.transform.position + m_camOffsetForCars;
            m_godCamera.transform.parent = m_nextCar.transform;
            m_previousCar = m_currentCar;
            m_currentCar = m_nextCar;
            m_currentCarID++;
            if (m_currentCarID >= m_rotPoints.Count)
            {
                m_currentCarID = 0;
            }
            m_nextCar = m_rotPoints[m_currentCarID];
        }

        void MoveToPrevCar()
        {
            m_godCamera.transform.position = m_previousCar.transform.position + m_camOffsetForCars;
            m_godCamera.transform.parent = m_previousCar.transform;
            m_nextCar = m_currentCar;
            m_currentCar = m_previousCar;
            m_currentCarID--;
            if (m_currentCarID < 0)
            {
                m_currentCarID = m_rotPoints.Count - 1;
            }
            m_previousCar = m_rotPoints[m_currentCarID];
        }

        public void ClearRotPoints()
        {
            for (int i = 0; i < m_rotPoints.Count; i++)
            {
                Destroy(m_rotPoints[i]);
            }
        }
    }
}