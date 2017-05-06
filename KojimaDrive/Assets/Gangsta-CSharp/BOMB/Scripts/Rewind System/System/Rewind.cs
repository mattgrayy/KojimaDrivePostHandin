using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class Rewind : MonoBehaviour
    {

        private Rewindable m_rewindable;
        [SerializeField]
        private float m_rateOfChange;

        // Use this for initialization
        void Start()
        {
            m_rewindable = GetComponent<Rewindable>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void PassRateOfChange()
        {
            if (m_rewindable.GetMatBool())
            {
                m_rewindable.GetMatComp().SetRateOfChange(m_rateOfChange);
            }
            if (m_rewindable.GetRotBool())
            {
                m_rewindable.GetRotComp().SetRateOfChange(m_rateOfChange);
            }
            if (m_rewindable.GetPosBool())
            {
                m_rewindable.GetPosComp().SetRateOfChange(m_rateOfChange);
            }
            if (m_rewindable.GetAnimBool())
            {
                m_rewindable.GetAnimComp().SetRateOfChange(m_rateOfChange);
            }
        }

        public void RewindData()
        {
            PassRateOfChange();
            if (m_rewindable.GetPosBool())
            {
                m_rewindable.GetPosComp().RewindPos();
            }
            if (m_rewindable.GetMatBool())
            {
                m_rewindable.GetMatComp().RewindMat();
            }
            if (m_rewindable.GetRotBool())
            {
                m_rewindable.GetRotComp().RewindRot();
            }
            if (m_rewindable.GetAnimBool())
            {
                m_rewindable.GetAnimComp().RewindAnim();
            }
        }

        public void ResetDirtyFlags()
        {
            if (m_rewindable.GetPosBool())
            {
                m_rewindable.GetPosComp().ResetDirtyFlags();
            }
            if (m_rewindable.GetRotBool())
            {
                m_rewindable.GetRotComp().ResetDirtyFlags();
            }
            if (m_rewindable.GetMatBool())
            {
                m_rewindable.GetMatComp().ResetDirtyFlags();
            }
            if (m_rewindable.GetAnimBool())
            {
                m_rewindable.GetAnimComp().ResetDirtyFlags();
            }
        }
    }
}