using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GCSharp
{
    public class Rewindable : MonoBehaviour
    {

        // Bools for stating which components are being rewound
        // If bool is true then that component is being rewound
        [SerializeField]
        private bool m_rewindPos = false;
        [SerializeField]
        private bool m_rewindRot = false;
        [SerializeField]
        private bool m_rewindMat = false;
        [SerializeField]
        private bool m_rewindAnim = false;

        private int m_recordLimit; // How much data can be recorded

        // Component References
        private MaterialComponent m_matComp;
        private RotationComponent m_rotComp;
        private PositionComponent m_posComp;
        private AnimationComponent m_animComp;

        // Use this for initialization
        void Start()
        {
            // Check if there are any components attached
            CheckForComps();
        }

        // Update is called once per frame
        void Update()
        {

        }

        // Check if there are any components attached. If there is a component attached to the gameobject
        // Store a refence to that component and set the corresponding bool to true.
        void CheckForComps()
        {
            if (gameObject.GetComponent<MaterialComponent>())
            {
                m_matComp = gameObject.GetComponent<MaterialComponent>();
                m_rewindMat = true;
            }
            if (gameObject.GetComponent<RotationComponent>())
            {
                m_rotComp = gameObject.GetComponent<RotationComponent>();
                m_rewindRot = true;
            }
            if (gameObject.GetComponent<PositionComponent>())
            {
                m_posComp = gameObject.GetComponent<PositionComponent>();
                m_rewindPos = true;
            }
            if (gameObject.GetComponent<AnimationComponent>())
            {
                m_animComp = gameObject.GetComponent<AnimationComponent>();
                m_rewindAnim = true;
            }
        }

        // Pass the record limit to all the attached components
        void PassRecordLimit(int _recordLimit)
        {
            //print("Test 3");
            if (m_rewindMat)
            {
                //print("Test 4");
                m_matComp.SetRecordLimit(_recordLimit);
            }
            if (m_rewindRot)
            {
                //print("Test 5");
                m_rotComp.SetRecordLimit(_recordLimit);
            }
            if (m_rewindPos)
            {
                //print("Test 6");
                m_posComp.SetRecordLimit(_recordLimit);
            }
        }

        // Reset all of the attached components data
        public void ResetData()
        {
            if (m_rewindPos)
            {
                m_posComp.ResetData();
            }
            if (m_rewindMat)
            {
                m_matComp.ResetData();
            }
            if (m_rewindRot)
            {
                m_rotComp.ResetData();
            }
        }

        // Set the record limit and call the PassRecordLimit function so the
        // Components can get the limit too
        public void SetRecordLimit(int _newLimit)
        {
            m_recordLimit = _newLimit;
            PassRecordLimit(m_recordLimit);
        }

        // Get the record limit
        public int GetRecordLimit()
        {
            return m_recordLimit;
        }

        // Get the position bool value
        public bool GetPosBool()
        {
            return m_rewindPos;
        }

        // Get the rotation bool value
        public bool GetRotBool()
        {
            return m_rewindRot;
        }

        // Get the material bool value
        public bool GetMatBool()
        {
            return m_rewindMat;
        }

        // Get the animation bool value
        public bool GetAnimBool()
        {
            return m_rewindAnim;
        }

        // Get a reference to the material component
        // Returns null if there isnt one attached
        public MaterialComponent GetMatComp()
        {
            if (m_rewindMat)
            {
                return m_matComp;
            }
            else
            {
                return null;
            }
        }

        // Get a reference to the Animation component
        // Returns null if there isnt one attached
        public AnimationComponent GetAnimComp()
        {
            if (m_rewindAnim)
            {
                return m_animComp;
            }
            else
            {
                return null;
            }
        }

        // Get a reference to the Position component
        // Returns null if there isnt one attached
        public PositionComponent GetPosComp()
        {
            if (m_rewindPos)
            {
                return m_posComp;
            }
            else
            {
                return null;
            }
        }

        // Get a reference to the Rotation component
        // Returns null if there isnt one attached
        public RotationComponent GetRotComp()
        {
            if (m_rewindRot)
            {
                return m_rotComp;
            }
            else
            {
                return null;
            }
        }
    }
}
