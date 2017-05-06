using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GCSharp
{
    public class PositionComponent : MonoBehaviour
    {

        [SerializeField]
        private List<Vector3> m_recordedPos;
        [SerializeField]
        private int m_recordLimit;
        private Vector3 m_lastPos;
        private float m_posTimer;
        [SerializeField]
        private float m_timeInterval;
        [SerializeField]
        private float m_rateOfChange;
        private int m_posCounter;
        private bool m_posCounterSet;
        public float m_valueBoost;

        // Use this for initialization
        void Start()
        {
            m_recordedPos = new List<Vector3>();
            m_posTimer = 0.0f;
            m_posCounterSet = false;
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
            m_recordedPos.Clear();
        }

        public void AddPos(Vector3 _newRot)
        {
            m_recordedPos.Add(_newRot);
        }

        public Vector3 GetPos(int _index)
        {
            return m_recordedPos[_index];
        }

        public int GetPosListCount()
        {
            return m_recordedPos.Count;
        }

        public void RemoveFirstPosValue()
        {
            m_recordedPos.RemoveAt(0);
        }

        //rewind objects rotation
        public void RecordPos()
        {
            if (m_posTimer > m_timeInterval)
            {
                if (gameObject.transform.position != m_lastPos)
                {
                    AddPos(gameObject.transform.position);
                    m_lastPos = gameObject.transform.position;
                }
                m_posTimer = 0;
            }

            m_posTimer += Time.deltaTime;

            if (GetPosListCount() > m_recordLimit)
            {
                RemoveFirstPosValue();
            }
        }

        //rewind objects rotation
        public void RewindPos()
        {
            if (!m_posCounterSet)
            {
                print("setting counter");
                m_posCounter = GetPosListCount() - 1;
                m_posCounterSet = true;
            }
            print("Counter: " + m_posCounter);
            if (transform.position == GetPos(m_posCounter) && m_posCounter > 0)
            {
                m_posCounter--;
            }
            transform.position = Vector3.MoveTowards(transform.position, GetPos(m_posCounter), Time.deltaTime * (m_rateOfChange * m_valueBoost));
        }

        public void ResetDirtyFlags()
        {
            m_posCounterSet = false;
        }
    }
}