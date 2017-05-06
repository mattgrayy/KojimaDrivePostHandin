using UnityEngine;
using System.Collections;

namespace Bam
{
    public abstract class ITerrainProperty : MonoBehaviour
    {
        public abstract float GetSpeedModifier(Kojima.CarScript car);
        public abstract float GetSlipModifier(Kojima.CarScript car);
    }
}