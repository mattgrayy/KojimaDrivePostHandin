using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HF
{
    public class DeadCarManager : MonoBehaviour
    {
        public enum SectioningMode
        {
            SQUARE = 0,
            SPECIFIC = 1,
            Length
        };

        public static DeadCarManager m_instance = null;
        public List<GameObject> m_prefabCars;
        public List<GameObject> m_sections;
        public List<Rect> m_sectionBoundaries;
        public List<int> m_sectionsToActivate;
        public SectioningMode m_mode;

        public List<GameObject> m_cars = new List<GameObject>();
        private Transform m_deadCarHolder;
        private int m_hiderNumber;


        // Use this for initialization
        void Start()
        {
            if (m_instance)
            {
                Destroy(this.gameObject);
            }
            else
            {
                m_instance = this;
            }

            for (int i = 0; i <= transform.childCount - 1; i++)
            {
                m_sections.Add(transform.GetChild(i).gameObject);
            }

            m_deadCarHolder = new GameObject("DeadCarHolder").transform;

            Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.DS_SETUP, EvFunc_OnDSSetup);
            Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.DS_RESET, EvFunc_OnDSReset);

            foreach (GameObject sections in m_sections)
            {
                Vector2 sectionPosition = new Vector2(sections.transform.position.x - 50.0f, sections.transform.position.z - 50.0f);
                Rect tempRect = new Rect(sectionPosition, new Vector2(100.0f, 100.0f));
                m_sectionBoundaries.Add(tempRect);
            }
        }

        void EvFunc_OnDSSetup()
        {
            SetSections();
            return;
        }

        void EvFunc_OnDSReset()
        {
            ResetSections();
            return;
        }

        public void SetHiderNumber(int _num)
        {
            m_hiderNumber = _num;
        }

        void SetSections()
        {
            switch (m_mode)
            {
                case SectioningMode.SQUARE:
                    {
                        int index = 0;
                        foreach (Rect sections in m_sectionBoundaries)
                        {
                            if (Kojima.GameModeManager.m_instance.m_currentGameMode.GetEventRect().Overlaps(sections))
                            {
                                SpawnCars(index);
                            }
                            index++;
                        }
                        break;
                    }
                case SectioningMode.SPECIFIC:
                    {
                        foreach (int index in m_sectionsToActivate)
                        {
                            SpawnCars(index - 1);
                        }

                        m_sectionsToActivate.Clear();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        void ResetSections()
        {
            if (m_cars.Count != 0)
            {
                for (int iter = 0; iter <= m_cars.Count - 1; iter++)
                {
                    Destroy(m_cars[iter].gameObject);
                }
                m_cars.Clear();
            }
        }

        public void AddSectionIndex(int _index)
        {
            m_sectionsToActivate.Add(_index);
        }

        public void AddSectionIndex(List<int> _indicies)
        {
            foreach (int index in _indicies)
            {
                m_sectionsToActivate.Add(index);
            }
        }

        void SpawnCars(int _section)
        {
            for (int iter = 0; iter <= m_sections[_section].transform.childCount - 1; iter++)
            {
                Transform spawnLocation = m_sections[_section].transform.GetChild(iter).transform;
                int range = Random.Range(1, 11);
                Quaternion rotation = spawnLocation.localRotation;
                if (range <= 5)
                {
                    Vector3 euler = rotation.eulerAngles;
                    euler = new Vector3(euler.x, euler.y + 180, euler.z);
                    rotation = Quaternion.Euler(euler);
                }
                GameObject tempObject = (GameObject)Instantiate(m_prefabCars[m_hiderNumber], spawnLocation.position, rotation);
                tempObject.transform.SetParent(m_deadCarHolder);
                m_cars.Add(tempObject);
            }
        }

        int GetRandomCar()
        {
            return Random.Range(0, m_prefabCars.Count);
        }
    }
}
