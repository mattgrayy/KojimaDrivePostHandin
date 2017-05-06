using UnityEngine;
using System.Collections;

namespace Bird
{
    public class NonWorldUICam : MonoBehaviour
    {

        public int camNumber;
        public Transform baseArrow;
        private Camera m_cam;
        public HUD_NavArrow camArrow;

        void Start()
        {
            m_cam = GetComponent<Camera>();
            SetMaskingLayer();
            camArrow.carArrow = baseArrow;
            //camArrow.SetMaskingLayer(camNumber);
        }

        void SetMaskingLayer()
        {
            m_cam.cullingMask |= 1 << LayerMask.NameToLayer("InWorldUIP" + camNumber);
        }

    }
}