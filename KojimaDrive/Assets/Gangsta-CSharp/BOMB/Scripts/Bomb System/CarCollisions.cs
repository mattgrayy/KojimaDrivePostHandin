using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class CarCollisions : MonoBehaviour
    {
        private GameObject m_BombSystemObject;
        private Bomb_System m_BombSystem;
        public bool m_TriggerOnce;

        void Start()
        {
            m_BombSystemObject = GameObject.FindGameObjectWithTag("BombSystem");
            m_BombSystem = m_BombSystemObject.GetComponent<Bomb_System>();
            m_TriggerOnce = true;
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.tag == "Player")

                if (m_TriggerOnce)
                {

                    m_TriggerOnce = false;

                }

        }

        //void OnTriggerEnter(Collider car)
        //{


        //    if (m_TriggerOnce)
        //    {

        //        m_TriggerOnce = false;
        //        m_BombSystem.SetSpawnNum();

        //    }

        //}

        //void OnTriggerExit(Collider car)
        //{ 
        //    m_TriggerOnce = true;
        //}
    }
}
