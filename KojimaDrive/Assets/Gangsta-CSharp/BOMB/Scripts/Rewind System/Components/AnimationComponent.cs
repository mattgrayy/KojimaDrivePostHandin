using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GCSharp
{
    public class AnimationComponent : MonoBehaviour
    {

        [SerializeField]
        private List<AnimationState> m_recordedAnim;
        [SerializeField]
        private float m_recordLimit;
        private AnimationState m_lastAnim;
        private float m_animTimer;
        [SerializeField]
        private float m_timeInterval;
        private Animator m_animator;
        [SerializeField]
        private float m_rateOfChange;
        private int m_animCounter;
        private bool m_animCounterSet;

        // Use this for initialization
        void Start()
        {
            m_recordedAnim = new List<AnimationState>();
            m_animTimer = 0.0f;
            m_animator = gameObject.GetComponent<Animator>();
            m_animCounterSet = false;
        }

        public void ResetData()
        {
            m_recordedAnim.Clear();
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

        public void AddAnim(AnimationState _newAnim)
        {
            print("recording anim");
            m_recordedAnim.Add(_newAnim);
        }

        public AnimationState GetAnim(int _index)
        {
            return m_recordedAnim[_index];
        }

        public int GetAnimListCount()
        {
            return m_recordedAnim.Count;
        }

        public void RemoveFirstAnimValue()
        {
            m_recordedAnim.RemoveAt(0);
        }

        //rewind objects material
        public void RecordAnim()
        {
            m_animator.StopPlayback();
            m_animator.StartRecording(60);
            m_animator.speed = 1.0f;

            //if (m_animTimer > m_timeInterval)
            //{
            //    if (m_animator.GetComponent<AnimationState>() != m_lastAnim)
            //    {
            //        AddAnim(m_animator.GetComponent<AnimationState>());
            //        m_lastAnim = m_animator.GetComponent<AnimationState>();
            //    }
            //    m_animTimer = 0;
            //}

            //m_animTimer += Time.deltaTime;

            //if (GetAnimListCount() > m_recordLimit)
            //{
            //    RemoveFirstAnimValue();
            //}
        }

        //rewind objects material
        public void RewindAnim()
        {
            m_animator.StopRecording();
            m_animator.StartPlayback();
            m_animator.speed = -1.0f;
        }

        public void ResetDirtyFlags()
        {
            m_animCounterSet = false;
        }
    }
}