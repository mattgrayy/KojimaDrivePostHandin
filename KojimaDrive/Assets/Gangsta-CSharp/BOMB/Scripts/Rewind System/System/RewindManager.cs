using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GCSharp
{
    public class RewindManager : MonoBehaviour
    {

        public List<GameObject> m_rewindableGameObjects;
        public float m_timeToRewind;
        public int m_recordLimit;
        public enum Mode { Rewind, Record, Reset };
        public Mode mode = Mode.Record;
        private float m_stopwatch;
        public Text m_stateText;

        // Use this for initialization
        void Start()
        {
            m_stopwatch = 0;
            foreach (GameObject rewindable in m_rewindableGameObjects)
            {
                print("Test 1");
                rewindable.GetComponent<Rewindable>().SetRecordLimit(m_recordLimit);
            }
            m_stateText.text = "State: Record";
            m_stateText.color = Color.red;
        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                mode = Mode.Rewind;
            }

            //if rewind true start rewind system
            if (mode == Mode.Rewind)
            {
                m_stateText.text = "State: Rewind";
                m_stateText.color = Color.green;
                m_stopwatch += Time.deltaTime;
                if (m_stopwatch > m_timeToRewind)
                {
                    print("Time Up");
                    mode = Mode.Reset;
                }
                else
                {
                    Rewind();
                }
            }
            else if (mode == Mode.Record)
            {
                foreach (GameObject rewindable in m_rewindableGameObjects)
                {
                    //print("Test 2");
                    rewindable.GetComponent<Rewindable>().SetRecordLimit(m_recordLimit);
                }
                m_stateText.text = "State: Record";
                m_stateText.color = Color.red;
                Record();
            }
            else if (mode == Mode.Reset)
            {
                m_stateText.text = "State: Reset";
                m_stateText.color = Color.white;
                ResetData();
            }
        }

        void Rewind()
        {
            //loop through each rewindable object and rewind specific traits
            foreach (GameObject rewindable in m_rewindableGameObjects)
            {
                rewindable.GetComponent<Rewind>().RewindData();
            }
        }

        void Record()
        {
            //loop through each rewindable object and rewind specific traits
            foreach (GameObject rewindable in m_rewindableGameObjects)
            {
                rewindable.GetComponent<Record>().RecordData();
            }
        }

        void ResetData()
        {
            m_stopwatch = 0;
            foreach (GameObject rewindable in m_rewindableGameObjects)
            {
                rewindable.GetComponent<Rewindable>().ResetData();
                rewindable.GetComponent<Rewind>().ResetDirtyFlags();
            }
            mode = Mode.Record;
        }

        public void SetMode(Mode _newMode)
        {
            mode = _newMode;
        }

        public Mode GetMode()
        {
            return mode;
        }
    }
}