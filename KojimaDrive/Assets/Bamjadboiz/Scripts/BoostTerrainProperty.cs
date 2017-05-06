using UnityEngine;
using System.Collections;
using Kojima;

namespace Bam
{
    public class BoostTerrainProperty : ITerrainProperty
    {
        public bool m_useObjectDirection;
        public Vector3 m_boostDirection;
        public float m_maxSpeedModifier;
        public float m_minSpeedModifier;
        public float m_slipModifier;
        public override float GetSpeedModifier(CarScript car)
        {
            Vector3 dir = m_useObjectDirection ? transform.forward : m_boostDirection;
            //Find angle between car and boost 
            float angle = Vector3.Angle(m_boostDirection, car.GetComponent<Rigidbody>().velocity.normalized);
            //Ignore cars going back, full boost to cars going in the right direction
            float power = Mathf.Clamp01(1 - (angle / 180));
            return Mathf.Lerp(m_minSpeedModifier, m_maxSpeedModifier, power);
        }

        public override float GetSlipModifier(CarScript car)
        {
            return m_slipModifier;
        }
    }
}