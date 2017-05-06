using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HF
{
    public class BarrierManager : MonoBehaviour
    {
        [SerializeField]
        public List<GameObject> m_barrier;
        public float m_heightOffset;
        public GameObject m_barrierPrefab;
        private Transform m_barrierHolder;

        void Start()
        {
            m_barrierHolder = new GameObject("BarrierHolder").transform;
            m_barrierHolder.transform.SetParent(transform);

            Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.DS_SETUP, EvFunc_OnDSSetup);
            Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.DS_RESET, EvFunc_OnDSFinish);
        }

        void EvFunc_OnDSSetup()
        {
            CreateBarrier();
            return;
        }

        void EvFunc_OnDSFinish()
        {
            DestroyBarrier();
            return;
        }

        void CreateBarrier()
        {
            Rect eventRect = Kojima.GameModeManager.m_instance.m_currentGameMode.GetEventRect();
            Vector3 min = new Vector3(eventRect.min.x, m_heightOffset, eventRect.min.y);
            Vector3 max = new Vector3(eventRect.max.x, m_heightOffset, eventRect.max.y);
            m_barrier.Add(Instantiate(m_barrierPrefab, min, Quaternion.identity) as GameObject);
            m_barrier.Add(Instantiate(m_barrierPrefab, min, Quaternion.identity) as GameObject);
            m_barrier.Add(Instantiate(m_barrierPrefab, max, Quaternion.identity) as GameObject);
            m_barrier.Add(Instantiate(m_barrierPrefab, max, Quaternion.identity) as GameObject);

            foreach (GameObject barrier in m_barrier)
            {
                barrier.transform.SetParent(m_barrierHolder);
            }

            m_barrier[0].transform.position += new Vector3(eventRect.width / 2, 0.0f, 0.0f);
            m_barrier[1].transform.position += new Vector3(0.0f, 0.0f, eventRect.height / 2);
            m_barrier[2].transform.position += new Vector3(-eventRect.width / 2, 0.0f, 0.0f);
            m_barrier[3].transform.position += new Vector3(0.0f, 0.0f, -eventRect.height / 2);

            m_barrier[0].transform.localScale = new Vector3(eventRect.width, m_heightOffset * 3, 1.0f);
            m_barrier[1].transform.localScale = new Vector3(1.0f, m_heightOffset * 3, eventRect.height);
            m_barrier[2].transform.localScale = new Vector3(eventRect.width, m_heightOffset * 3, 1.0f);
            m_barrier[3].transform.localScale = new Vector3(1.0f, m_heightOffset * 3, eventRect.height);
        }

        void DestroyBarrier()
        {
            for (int iter = 0; iter <= m_barrier.Count - 1; iter++)
            {
                Destroy(m_barrier[iter]);
            }

            m_barrier.Clear();
        }
    }
}
