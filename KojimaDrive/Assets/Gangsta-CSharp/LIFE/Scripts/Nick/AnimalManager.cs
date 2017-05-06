using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GCSharp
{
    public class AnimalManager : MonoBehaviour
    {
        private float m_fABreedTime;
        private bool m_bSetup;
        private int m_iInitSpawnCount;
        private int m_MaxAnimalcount;

        public List<GameObject> m_lRabbit;
        public List<GameObject> m_lChicken;
        public List<GameObject> m_lGoat;


        public GameObject m_Rabbit;
        public GameObject m_Chicken;
        public GameObject m_Goat;

        public bool m_bclean;
        private bool m_toggle;
        // Use this for initialization
        void Start()
        {


            m_lRabbit = new List<GameObject>();
            m_lChicken = new List<GameObject>();
            m_lGoat = new List<GameObject>();

            m_fABreedTime = 10.0f; //Default value
            m_iInitSpawnCount = 2; //Default value
            m_MaxAnimalcount = m_iInitSpawnCount + (m_iInitSpawnCount / 2);
            m_bSetup = true;
            m_bclean = false;

            Spawn();

        }

        // Update is called once per frame
        void Update()
        {


            /*if (gameObject.activeSelf && m_bSetup)
            {
                Spawn();
                m_bSetup = false;
            }*/
            if (m_lRabbit.Count >= m_MaxAnimalcount)
            {
                Destroy(m_lRabbit[0]);
                m_lRabbit.RemoveAt(0);
            }
            if (m_lChicken.Count >= m_MaxAnimalcount)
            {
                Destroy(m_lChicken[0]);
                m_lChicken.RemoveAt(0);
            }


            /*  if (m_bclean)
              {
                  Cleaner();
                  m_bclean = false;
              }*/


        }

        public void AddTolist(GameObject _GO, string _Animal)
        {
            if (_Animal == "Chicken")
            {
                m_lChicken.Add(_GO);
            }
            if (_Animal == "Rabbit")
            {
                m_lRabbit.Add(_GO);
            }
        }


        //spawn all animals into the chunk
        public void Spawn()
        {

            Vector3 m_pos;
            // if (Random.Range(0, 5) == 1)
            // {
            for (int i = 0; i <= m_iInitSpawnCount - 1; i++)
            {
                m_pos = PosCheck();
                GameObject t_newrabbit = (GameObject)Instantiate(m_Rabbit,  m_pos, Quaternion.identity);
                t_newrabbit.GetComponentInChildren<Breeder>().SetTime(m_fABreedTime);
                t_newrabbit.GetComponentInChildren<Breeder>().SetManager(gameObject);
                t_newrabbit.transform.parent = gameObject.transform;
                m_lRabbit.Add(t_newrabbit);


                m_pos = PosCheck();
                GameObject t_newchicken = (GameObject)Instantiate(m_Chicken, m_pos, Quaternion.identity);
                t_newchicken.GetComponentInChildren<Breeder>().SetTime(m_fABreedTime);
                t_newchicken.GetComponentInChildren<Breeder>().SetManager(gameObject);
                t_newchicken.transform.parent = gameObject.transform;


                m_lChicken.Add(t_newchicken);
            }

            for (int i = 0; i <= m_iInitSpawnCount - 1; i++)
            {
                m_pos = PosCheck();
                GameObject t_newgoat = (GameObject)Instantiate(m_Goat,m_pos, Quaternion.identity);
                t_newgoat.transform.parent = gameObject.transform;
                m_lGoat.Add(t_newgoat);
            }
            // }
        }

        Vector3 PosCheck()
        {
            Vector3 m_VectorVal;

            Vector2 m_val = Random.insideUnitCircle * 100;
            m_VectorVal = new Vector3(m_val.x, 0, m_val.y);

            return m_VectorVal;

        }

        float RandomFloat(float _min, float _max)
        {
            float m_value = Random.Range(_min, _max);
            return m_value;
        }

        //public void ToggleAnimals(bool _toggle)
        //{
        //    m_toggle = _toggle;
        //    for (int i = 0; i < m_lChicken.Count; i++)
        //    {
        //        m_lChicken[i].SetActive(_toggle);
        //    }
        //    for (int i = 0; i < m_lRabbit.Count; i++)
        //    {
        //        m_lRabbit[i].SetActive(_toggle);
        //    }
        //    for (int i = 0; i < m_lGoat.Count; i++)
        //    {
        //        m_lGoat[i].SetActive(_toggle);
        //    }

        //}


        //public void Cleaner()
        //{

        //    for (int i = m_lChicken.Count - 1; i >= 0; i--)
        //    {
        //        Destroy(m_lChicken[i]);
        //    }
        //    for (int i = m_lChicken.Count - 1; i >= 0; i--)
        //    {
        //        m_lChicken.RemoveAt(i);
        //    }


        //    for (int i = m_lRabbit.Count - 1; i >= 0; i--)
        //    {
        //        Destroy(m_lRabbit[i]);
        //    }
        //    for (int i = m_lRabbit.Count - 1; i >= 0; i--)
        //    {
        //        m_lRabbit.RemoveAt(i);
        //    }

        //    for (int i = m_lGoat.Count - 1; i >= 0; i--)
        //    {
        //        Destroy(m_lGoat[i]);
        //    }
        //    for (int i = m_lGoat.Count - 1; i >= 0; i--)
        //    {
        //        m_lGoat.RemoveAt(i);
        //    }



        //    m_bSetup = true;
        //}


        /* public override void enableState()
         {
             print("enable");
             if (gameObject.activeSelf && m_bSetup)
             {

                 Spawn();
                 m_bSetup = false;
                 m_bclean = false;
             }
         }
         public override void disableState()
         {
             print("disable1");
             if (!m_bclean)
             {
                 Cleaner();
                 m_bclean = true;
             }
         }

         public override void translateState()
         {

         }*/
    }
}