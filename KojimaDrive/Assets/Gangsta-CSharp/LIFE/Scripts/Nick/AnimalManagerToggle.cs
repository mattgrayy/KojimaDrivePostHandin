using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class AnimalManagerToggle : BaseWorldChange
    {
        public GameObject m_GAM;
        private AnimalManager m_AM;
        private bool m_btrigger;
        public Vector3 m_chunkLoc;
        // Use this for initialization
        void Start()
        {
            m_AM = m_GAM.GetComponent<AnimalManager>();
            //m_chunkLoc = 
 
        }


        public override void enableState()
        {
            RaycastHit hit;
            foreach (GameObject GR in m_AM.m_lRabbit)
            {
                GR.SetActive(true);
                
                if (GR.transform.position.y <= 1f)
                {
                    
                    if (Physics.Raycast(GR.transform.position, Vector3.back, out hit))
                    {
                       // print(GR.transform.position);
                        //Debug.DrawRay(GR.transform.position, Vector3.up);
                        GR.transform.position = hit.point+new Vector3(0f,0f,10f);
                        // Debug.Log(hit.collider.gameObject);
                    }

                }


            }
            foreach (GameObject GC in m_AM.m_lChicken)
            {

                GC.SetActive(true);
                if (GC.transform.position.y <= 1f)
                {
                    if (Physics.Raycast(GC.transform.position, Vector3.back, out hit))
                    {
                        GC.transform.position = hit.point + new Vector3(0f, 0f, 10f);
                        // Debug.Log(hit.collider.gameObject);
                    }

                }


            }
            foreach (GameObject GG in m_AM.m_lGoat)
            {

                GG.SetActive(true);
                if (GG.transform.position.y <= 1f)
                {
                    if (Physics.Raycast(GG.transform.position, Vector3.back, out hit))
                    {
                        GG.transform.position = hit.point + new Vector3(0f, 0f, 10f);
                        // Debug.Log(hit.collider.gameObject);
                    }

                }


            }




        }


        public override void disableState()
        {
            foreach (GameObject GR in m_AM.m_lRabbit)
            {
                GR.SetActive(false);
            
            }
            foreach (GameObject GC in m_AM.m_lChicken)
            {

                GC.SetActive(false);
             

            }
            foreach (GameObject GG in m_AM.m_lGoat)
            {

                GG.SetActive(false);
              
            }

        }

        public override void translateState()
        {

        }
    }
}