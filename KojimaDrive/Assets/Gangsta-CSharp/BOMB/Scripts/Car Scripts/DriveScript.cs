using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class DriveScript : MonoBehaviour
    {

        public WheelCollider leftFrontWheel;
        public WheelCollider rightFrontWheel;
        public WheelCollider leftRearWheel;
        public WheelCollider rightRearWheel;
        public float maxMotorTorque;
        public float maxSteeringAngle;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            float motor = maxMotorTorque * Input.GetAxis("Vertical");
            float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

            leftFrontWheel.steerAngle = steering;
            rightFrontWheel.steerAngle = steering;

            leftRearWheel.motorTorque = -motor;
            rightRearWheel.motorTorque = -motor;
        }
    }
}