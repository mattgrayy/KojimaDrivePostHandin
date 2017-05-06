using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GCSharp
{
    public class RotationComponent : MonoBehaviour
    {

        [SerializeField]
        private List<Quaternion> m_recordedRot;
        private int m_recordLimit;
        private Quaternion m_lastRot;
        private float m_rotTimer;
        [SerializeField]
        private float m_timeInterval;
        [SerializeField]
        private float m_rateOfChange;
        private int m_rotCounter;
        private bool m_rotCounterSet;
        public float m_valueBoost;

        // Use this for initialization
        void Start()
        {
            m_recordedRot = new List<Quaternion>();
            m_rotTimer = 0.0f;
            m_rotCounterSet = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetRecordLimit(int _newLimit)
        {
            m_recordLimit = _newLimit;
        }

        public void SetRateOfChange(float _newROC)
        {
            m_rateOfChange = _newROC;
        }

        public void SetTimeInterval(float _newTimeInterval)
        {
            m_timeInterval = _newTimeInterval;
        }

        public void ResetData()
        {
            m_recordedRot.Clear();
        }

        public void AddRot(Quaternion _newRot)
        {
            m_recordedRot.Add(_newRot);
        }

        public Quaternion GetRot(int _index)
        {
            return m_recordedRot[_index];
        }

        public int GetRotListCount()
        {
            return m_recordedRot.Count;
        }

        public void RemoveFirstRotValue()
        {
            m_recordedRot.RemoveAt(0);
        }

        //rewind objects rotation
        public void RecordRot()
        {
            if (m_rotTimer > m_timeInterval)
            {
                if (gameObject.transform.rotation != m_lastRot)
                {
                    AddRot(gameObject.transform.rotation);
                    m_lastRot = gameObject.transform.rotation;
                }
                m_rotTimer = 0;
            }

            m_rotTimer += Time.deltaTime;

            if (GetRotListCount() > m_recordLimit)
            {
                RemoveFirstRotValue();
            }
        }

        //rewind objects rotation
        public void RewindRot()
        {
            if (!m_rotCounterSet)
            {
                print("setting counter");
                m_rotCounter = GetRotListCount() - 1;
                m_rotCounterSet = true;
            }
            print("Counter: " + m_rotCounter);
            if (transform.rotation == GetRot(m_rotCounter) && m_rotCounter > 0)
            {
                m_rotCounter--;
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, GetRot(m_rotCounter), Time.deltaTime * (m_rateOfChange * m_valueBoost));
        }

        public void ResetDirtyFlags()
        {
            m_rotCounterSet = false;
        }
    }
}