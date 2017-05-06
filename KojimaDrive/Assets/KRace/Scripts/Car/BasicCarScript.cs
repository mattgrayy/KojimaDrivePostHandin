using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KRace
{
    public class BasicCarScript : MonoBehaviour
    {
        public GameObject playerCar;
        public Rigidbody rb;

        public WheelCollider frontLeft, frontRight, backLeft, backRight;

        float enginePower = 160.0f;

        float power = 0.0f;
        float brake = 0.0f;
        float steer = 0.0f;

        float maxSteer = 25.0f;

        // Use this for initialization
        void Start()
        {
            rb.centerOfMass = new Vector3(0.0f, 0.5f, 0.3f);
        }

        // Update is called once per frame
        void Update()
        {
            power = Input.GetAxis("Vertical") * enginePower * Time.deltaTime * 200.0f;
            steer = Input.GetAxis("Horizontal") * maxSteer;
            brake = Input.GetKey("space") ? rb.mass * 0.1f : 0.0f;
            //Debug.Log(power);
            frontLeft.steerAngle = steer;
            frontRight.steerAngle = steer;

            if (brake > 0.0)
            {
                frontLeft.brakeTorque = brake;
                frontRight.brakeTorque = brake;
                backLeft.brakeTorque = brake;
                backRight.brakeTorque = brake;
                //RWD MotorTorque
                backLeft.motorTorque = 0.0f;
                backRight.motorTorque = 0.0f;
            }
            else
            {
                frontLeft.brakeTorque = 0.0f;
                frontRight.brakeTorque = 0.0f;
                backLeft.brakeTorque = 0.0f;
                backRight.brakeTorque = 0.0f;
                //RWD MotorTorque
                backLeft.motorTorque = power;
                backRight.motorTorque = power;
            }
        }
    }

}