using UnityEngine;
using System.Collections;

//===================== Kojima Drive - Half-Full 2017 ====================//
//
// Author: SAM BRITNALL
// Purpose: Twist script to manage the specifics of the tornado
// Namespace: HALF-FULL
//
//===============================================================================//

namespace HF
{
    public class TornadoTwist : TwistBase
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        new
        public void SpawnTwistPrefab()
        {
            

            for (int iter2 = 0; Kojima.GameController.s_singleton.m_players.Length > iter2; iter2++)
            {
                Kojima.GameController.s_singleton.m_players[iter2].gameObject.AddComponent<GCSharp.Orbiting>();

            }
        }
    }
}