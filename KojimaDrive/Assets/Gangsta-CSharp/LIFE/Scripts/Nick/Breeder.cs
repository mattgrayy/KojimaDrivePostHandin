using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class Breeder : MonoBehaviour
    {
        
        private float m_fBreedTime; //length of time before next conception
        private float m_fTimer; // timer variable
        private GameObject m_manager;

        void Start()
        {
            m_fTimer = 0f;
            
        }
        void Update()
        {
            m_fTimer += Time.deltaTime;
        }

       
        public void SetTime(float _newtime)
        {
            m_fBreedTime = _newtime;
        }

        public void SetManager(GameObject _manager)
        {
            m_manager = _manager;
        }


        /*public void OnCollisionStay(Collision col)
        {

            if (col.gameObject.tag == gameObject.tag)
            {

                //breed
                if (m_fTimer >= m_fBreedTime)
                {
                    Vector3 m_SpawnPos = transform.position;
                    m_SpawnPos.z += 5.0f;
                    m_SpawnPos.y += 15.0f;
                    m_SpawnPos.z += 5.0f;

                    GameObject t_newAnimal = (GameObject)Instantiate(gameObject, m_SpawnPos, transform.localRotation);
                    t_newAnimal.transform.parent = m_manager.transform;
                    t_newAnimal.GetComponent<Breeder>().SetManager(m_manager);
                    t_newAnimal.GetComponent<Breeder>().SetTime(m_fBreedTime);
                    m_manager.GetComponent<AnimalManager>().AddTolist(t_newAnimal, gameObject.tag);
                        
                    m_fTimer = 0.0f; //reset counter

                }
            }
            
        }*/
    }
}
