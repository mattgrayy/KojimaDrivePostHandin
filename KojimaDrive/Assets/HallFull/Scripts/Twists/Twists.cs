using UnityEngine;
using System.Collections;

//===================== Kojima Drive - Half-Full 2017 ====================//
//
// Author: SAM BRITNALL
// Purpose: Container for the settings of the twist manager
// Namespace: HALF-FULL
//
//===============================================================================//

namespace HF
{
    public struct Twists
    {
        public enum Twist
        {
            NULL,
            tsunami,
            test,
            tornado

        }

        public bool allOff;
        public bool tsunamiOff;
        public bool testOff;
        public bool tornadoOff;
        //public bool eruptionOff;
    }

}