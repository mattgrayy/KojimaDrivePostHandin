using UnityEngine;
using System.Collections;

namespace Bam
{
    public class CarSuspensionScript : MonoBehaviour
    {
        Kojima.CarScript m_carScr;
        Transform m_carBody;
        float m_suspensionAmount = 1;

        HingeJoint m_joint;
        Rigidbody m_bodyRB;

        public void Initialise(Transform newCarBody, float newStrength, Kojima.CarScript carScr)
        {
            m_carScr = carScr;
            m_carBody = newCarBody;
            m_suspensionAmount = newStrength;

            m_carBody.transform.SetParent(null);
            m_carBody.name = carScr.name + "'s Suspension Body";

            m_bodyRB = m_carBody.gameObject.AddComponent<Rigidbody>();
            m_bodyRB.useGravity = true;
            m_bodyRB.mass = 0;

            m_joint = m_carBody.gameObject.AddComponent<HingeJoint>();
            m_joint.connectedBody = gameObject.GetComponent<Rigidbody>();

            JointSpring newSpring = new JointSpring();
            newSpring.spring = 355 * m_suspensionAmount;
            newSpring.damper = 3;

            m_joint.useSpring = true;
            m_joint.spring = newSpring;

            m_joint.axis = new Vector3(0, 0, 1);

            JointLimits newLimits = new JointLimits();
            float limit = 7;

            newLimits.bounciness = 0.0f;
            newLimits.min = -limit;
            newLimits.max = limit;

            m_joint.limits = newLimits;
            m_joint.useLimits = true;
        }

        // Update is called once per frame
        void Update()
        {
            //m_carBody.gameObject.transform.position = Vector3.Lerp(m_carBody.transform.position, transform.position, 10 * Time.deltaTime);
        }

        void OnDestroy()
        {
            if (m_carBody)
            {
                Destroy(m_carBody.gameObject);
            }
        }

        public void ApplySteerForce(float steer)
        {
            m_bodyRB.AddTorque(transform.forward * steer, ForceMode.Acceleration);
        }
    }
}