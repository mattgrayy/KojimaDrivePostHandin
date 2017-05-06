using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class TornadoPhys : MonoBehaviour
    {
        GameObject tornado;
        public bool m_bToOrbit;
        private GameObject m_gPulledGO;
        private bool m_bMoveToward;
        public float m_fRangeSpeed;

        void Start()
        {
            tornado = GameObject.FindWithTag("Tornado");
            m_bMoveToward = true;
        }

        public void OnTriggerStay(Collider coll)
        {
            //Set objects to be affected by the tornado physics
            if (coll.gameObject.tag == "Pullable")
            {
                m_gPulledGO = coll.gameObject;

                //This pulls objects to the centre of the Tornado
                //This will be edited to circle objects around the emitter
                //_center = this.transform;
                if (m_bMoveToward == true)
                {
                    m_gPulledGO.transform.position = Vector3.MoveTowards(m_gPulledGO.transform.position, this.transform.position, m_fRangeSpeed * Time.deltaTime);
                }
               
                if (m_gPulledGO.transform.position == tornado.transform.position)
                {
                    m_bMoveToward = false;
                }
            }
        }
    }
}
