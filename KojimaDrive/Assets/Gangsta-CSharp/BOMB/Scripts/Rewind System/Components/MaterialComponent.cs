using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GCSharp
{
    public class MaterialComponent : MonoBehaviour
    {

        [SerializeField]
        private List<Material> m_recordedMat;
        [SerializeField]
        private float m_recordLimit;
        private Material m_lastMat;
        private float m_matTimer;
        [SerializeField]
        private float m_timeInterval;
        private Renderer m_renderer;
        [SerializeField]
        private float m_rateOfChange;
        private int m_matCounter;
        private bool m_matCounterSet;
        private Material[] m_renderersMats;
        public int m_matNum;

        // Use this for initialization
        void Start()
        {
            m_recordedMat = new List<Material>();
            m_matTimer = 0.0f;
            m_renderer = gameObject.GetComponent<Renderer>();
            m_matCounterSet = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ResetData()
        {
            m_recordedMat.Clear();
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

        public void AddMat(Material _newMat)
        {
            print("recording mat");
            m_recordedMat.Add(_newMat);
        }

        public Material GetMat(int _index)
        {
            return m_recordedMat[_index];
        }

        public int GetMatListCount()
        {
            return m_recordedMat.Count;
        }

        public void RemoveFirstMatValue()
        {
            m_recordedMat.RemoveAt(0);
        }

        //rewind objects material
        public void RecordMat()
        {
            if (m_matTimer > m_timeInterval)
            {
                if (m_renderer.materials[m_matNum] != m_lastMat)
                {
                    AddMat(m_renderer.materials[m_matNum]);
                    m_lastMat = m_renderer.materials[m_matNum];
                }
                m_matTimer = 0;
            }

            m_matTimer += Time.deltaTime;

            if (GetMatListCount() > m_recordLimit)
            {
                RemoveFirstMatValue();
            }
        }

        //rewind objects material
        public void RewindMat()
        {
            if (!m_matCounterSet)
            {
                print("setting counter");
                m_matCounter = GetMatListCount() - 1;
                m_matCounterSet = true;
            }

            if (m_renderer.materials[m_matNum].color == GetMat(m_matCounter).color && m_matCounter > 0)
            {
                print("Mat Counter: " + m_matCounter);
                m_matCounter--;
            }
            m_renderer.materials[m_matNum].Lerp(m_renderer.materials[m_matNum], GetMat(m_matCounter), m_rateOfChange / 10);
        }

        public void ResetDirtyFlags()
        {
            m_matCounterSet = false;
        }
    }
}