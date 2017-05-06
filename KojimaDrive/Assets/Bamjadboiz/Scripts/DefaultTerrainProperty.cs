using UnityEngine;
using System.Collections;
using Kojima;
using System;

namespace Bam
{
    public class DefaultTerrainProperty : ITerrainProperty
    {
        [SerializeField]
        private float m_speedModifier;
        [SerializeField]
        private float m_slipModifier;

        public override float GetSpeedModifier(CarScript car)
        {
            return m_speedModifier;
        }

        public override float GetSlipModifier(CarScript car)
        {
            return m_slipModifier;
        }
    }
}
