using UnityEngine;
using System.Collections;

namespace KRace
{
    public class CarScript : MonoBehaviour
    {

        Transform wheels;

        public float enginePower = 150.0f;
        public float power = 0.0f;
        public float brake = 0.0f;
        public float steer = 0.0f;
        public float maxSteer = 0.0f;

        public WheelCollider frontLeft, frontRight, backLeft, backRight;

        Rigidbody rigidBody;

        // Use this for initialization
        void Start()
        {
            rigidBody = gameObject.GetComponent<Rigidbody>();
            rigidBody.centerOfMass = new Vector3(0.0f, -0.5f, 0.3f);
        }

        // Update is called once per frame
        void Update()
        {
            power = Input.GetAxis("Vertical") * enginePower * Time.deltaTime * 250.0f;
            steer = Input.GetAxis("Horizontal") * maxSteer;
            brake = Input.GetKey("space") ? rigidBody.mass * 0.1f : 0.0f;

            frontLeft.steerAngle = steer;
            frontRight.steerAngle = steer;

            if (brake > 0.0f)
            {
                frontLeft.brakeTorque = brake;
                frontRight.brakeTorque = brake;
                backLeft.brakeTorque = brake;
                backRight.brakeTorque = brake;

                backLeft.motorTorque = 0.0f;
                backRight.motorTorque = 0.0f;
            }
            else
            {
                frontLeft.brakeTorque = 0.0f;
                frontRight.brakeTorque = 0.0f;
                backLeft.brakeTorque = 0.0f;
                backRight.brakeTorque = 0.0f;

                backLeft.motorTorque = power;
                backRight.motorTorque = power;
            }
        }

        //WheelCollider GetCollider(int n)
        //{
        //    return wheels[n].gameObject.GetComp
        //}
    }
}