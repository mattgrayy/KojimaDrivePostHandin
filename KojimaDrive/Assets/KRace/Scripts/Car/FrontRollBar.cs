using UnityEngine;
using System.Collections;

namespace KRace
{
    public class FrontRollBar : MonoBehaviour
    {

        public WheelCollider WheelL;
        public WheelCollider WheelR;
        float AntiRoll = 5000.0f;
        public Rigidbody rb;
        void FixedUpdate()
        {
            WheelHit hit;
            float travelL = 1.0f;
            float travelR = 1.0f;

            bool groundedL = WheelL.GetGroundHit(out hit);
            if (groundedL)
                travelL = (-WheelL.transform.InverseTransformPoint(hit.point).y - WheelL.radius) / WheelL.suspensionDistance;

            var groundedR = WheelR.GetGroundHit(out hit);
            if (groundedR)
                travelR = (-WheelR.transform.InverseTransformPoint(hit.point).y - WheelR.radius) / WheelR.suspensionDistance;

            var antiRollForce = (travelL - travelR) * AntiRoll;

            if (groundedL)
                rb.AddForceAtPosition(WheelL.transform.up * -antiRollForce,
                       WheelL.transform.position);
            if (groundedR)
                rb.AddForceAtPosition(WheelR.transform.up * antiRollForce,
                       WheelR.transform.position);

        }
    }
}