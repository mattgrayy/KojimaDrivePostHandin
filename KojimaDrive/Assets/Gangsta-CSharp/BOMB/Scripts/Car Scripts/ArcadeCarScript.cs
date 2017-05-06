using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class ArcadeCarScript : MonoBehaviour
    {
        public float m_acceleration = 10.0f;
        public float m_maxSpeed = 30.0f;
        public float m_airResistance = 1.0f;
        public float m_friction = 5.0f;
        public float m_breakFriction = 15.0f;
        public float m_maxWheelAngle = 35.0f;
        public float m_twistSpeed = 35.0f;
        public float m_flipSpeed = 35.0f;
        public float m_emergencyTorqueForce = 30.0f;
        public float m_emergencyJumpForce = 30.0f;
        public float m_controlledVelocity;
        private bool m_offGround;

        private bool m_speedBoostActive;
        [SerializeField]
        float m_multiplier, m_timeInterval;
        private float m_timer;
        private GameObject m_rewindManager;
        private RewindManager m_rewindManagerScript;

        public Transform[] m_wheels = new Transform[4];
        public float m_wheelRayLength = 1.0f;
        public float m_upperRayLength = 1.0f;
        private int m_wheelCount;
        private int m_wheelsOnGround;
        private bool m_upsideDown;

        private Vector3 m_lastGroundedDir;
        public int playerID;

        private Rigidbody m_rb;
        private float m_wheelAngle;
        // Use this for initialization
        void Start()
        {
            m_rb = GetComponent<Rigidbody>();
            Debug.Assert(m_rb, "Rigidbody component not found");
            m_offGround = false;
            m_wheelsOnGround = 0;
            m_wheelCount = m_wheels.Length;
            m_speedBoostActive = false;
            m_rewindManager = GameObject.FindGameObjectWithTag("RewindManager");
            m_rewindManagerScript = m_rewindManager.GetComponent<RewindManager>();
        }

        void Update()
        {
            if (m_speedBoostActive)
            {
                if (m_timer > m_timeInterval)
                {
                    m_timer = 0.0f;
                    m_speedBoostActive = false;
                    print("speed boost finished");
                }
                m_timer += Time.deltaTime;
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (m_rewindManagerScript.GetMode() == RewindManager.Mode.Record)
            {
                m_rb.isKinematic = false;
                m_rb.useGravity = true;
                IsInAir();
                ApplyFriction();

                if (m_offGround && m_wheelsOnGround == 0)
                {
                    ManeuverOffGround();
                    if (m_upsideDown)
                    {
                        AllowFlip();
                    }
                }
                else
                {
                    Accelerate();
                    TurnWheels();
                    m_lastGroundedDir = transform.forward;
                }
                Vector3 newPosition = transform.position + m_controlledVelocity * Time.fixedDeltaTime * m_lastGroundedDir;
                m_rb.MovePosition(newPosition);
            }
            if (m_rewindManagerScript.GetMode() == RewindManager.Mode.Rewind)
            {
                m_rb.isKinematic = true;
                m_rb.useGravity = false;
            }
        }

        void IsInAir()
        {
            m_offGround = true;
            m_upsideDown = false;
            m_wheelsOnGround = 0;
            RaycastHit[] results;
            foreach (Transform t in m_wheels)
            {
                results = Physics.RaycastAll(t.position, -transform.up, m_wheelRayLength);
                Debug.DrawLine(t.position, t.position - transform.up * m_wheelRayLength, Color.green);
                foreach (RaycastHit col in results)
                {
                    if (col.transform.gameObject.tag != "Player")
                    {
                        m_offGround = false;
                        m_wheelsOnGround += 1;
                        break;
                    }
                }
            }
            results = Physics.RaycastAll(transform.position, transform.up, m_upperRayLength);
            Debug.DrawLine(transform.position, transform.position + transform.up * m_upperRayLength, Color.green);
            foreach (RaycastHit col in results)
            {
                if (m_offGround && col.transform.gameObject.tag != "Player")
                {
                    m_upsideDown = true;
                }
            }
        }

        void AllowFlip()
        {
            if (Input.GetButton("Jump"))
            {
                m_rb.velocity = (new Vector3(0, m_emergencyJumpForce, 0));
                m_rb.AddTorque(transform.right * m_emergencyTorqueForce);
            }
        }

        void ManeuverOffGround()
        {
            float twist = Input.GetAxis("Horizontal") * m_twistSpeed;
            float flip = Input.GetAxis("Vertical") * m_flipSpeed;
            Quaternion rot = Quaternion.Euler(flip * Time.fixedDeltaTime, 0, twist * Time.fixedDeltaTime) * m_rb.rotation;
            m_rb.MoveRotation(rot);
            //m_rb.AddTorque(transform.right * flip);
            //m_rb.AddTorque(transform.forward * twist);
        }

        void ApplyFriction()
        {
            float friction = m_friction;
            if (Input.GetButton("Jump"))
            {
                friction = m_breakFriction;
            }
            if (m_offGround && !m_upsideDown)
            {
                friction = m_airResistance;
            }
            if (m_controlledVelocity != 0.0f)
            {
                float directionMultiplier = m_controlledVelocity / Mathf.Abs(m_controlledVelocity); //1 if pos -1 if neg
                m_controlledVelocity -= friction * Time.fixedDeltaTime * directionMultiplier;
                if (directionMultiplier == 1 && m_controlledVelocity < 0)
                {
                    m_controlledVelocity = 0;
                }
                if (directionMultiplier == -1 && m_controlledVelocity > 0)
                {
                    m_controlledVelocity = 0;
                }
            }
        }

        void Accelerate()
        {
            float acc = m_acceleration;
            float maxSpeed = m_maxSpeed;
            if (m_speedBoostActive)
            {
                float origAccel = m_acceleration;
                float origMaxSpeed = m_maxSpeed;
                acc = origAccel * m_multiplier;
                maxSpeed = origMaxSpeed * m_multiplier;
                print("new accel: " + acc + " new max speed: " + maxSpeed);
            }
            float thrustInput = Input.GetAxis("Vertical" + playerID);
            m_controlledVelocity += thrustInput * acc * Time.fixedDeltaTime;
            if (m_controlledVelocity > maxSpeed)
            {
                m_controlledVelocity = maxSpeed;
            }
            if (m_controlledVelocity < -maxSpeed)
            {
                m_controlledVelocity = -maxSpeed;
            }
        }

        void TurnWheels()
        {
            m_wheelAngle = Input.GetAxis("Horizontal" + playerID) * m_maxWheelAngle;
            if (m_controlledVelocity != 0.0f)
            {
                Vector3 rotation = m_rb.rotation.eulerAngles;
                float wheelRelRot = rotation.y + m_wheelAngle;
                float rot = rotation.y;
                rot += m_controlledVelocity / (Mathf.PI * 2) * m_wheelAngle * Time.fixedDeltaTime;    //just throw a pi in there somewhere lul
                rotation.y = rot;
                Quaternion qRot = m_rb.rotation;
                qRot.eulerAngles = rotation;
                m_rb.MoveRotation(qRot);
            }
        }

        public void SpeedBoost(bool _isActive)
        {
            m_speedBoostActive = _isActive;
        }
    }
}