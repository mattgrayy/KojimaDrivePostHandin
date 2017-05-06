using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GCSharp
{
    public class LavaFlow : MonoBehaviour
    {
        public float m_Drag;
        public float m_mass;

        //creating a new mesh
      

        void Start()
        {

        }

        void Update()
        {
            foreach (Transform child in transform)
            {
                Rigidbody t_RB = child.GetComponent<Rigidbody>();
                t_RB.drag = m_Drag;
                t_RB.mass = m_mass;

            }


        }
    }
}
