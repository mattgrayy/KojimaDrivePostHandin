using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GCSharp
{
    public class MaterialChanger : MonoBehaviour
    {
        private GameObject m_body, m_subBody;
        private Material m_initMat, m_bombHolderMat;
        private Material[] m_renderersMats;
        private int m_matNum;
        private Renderer m_renderer;

        // Use this for initialization
        void Start()
        {
            if (m_body.GetComponent<Renderer>())
            {
                m_renderer = m_body.GetComponent<Renderer>();
                m_renderersMats = m_renderer.materials;
                m_initMat = m_renderersMats[m_matNum];
            }
            else
            {
                m_subBody = m_body.transform.FindChild("Body").gameObject;
                if (m_subBody.GetComponent<Renderer>())
                {
                    m_renderer = m_subBody.GetComponent<Renderer>();
                    m_renderersMats = m_renderer.materials;
                    m_initMat = m_renderersMats[m_matNum];
                }
                else
                {
                    GameObject t_subBody = m_subBody.transform.FindChild("mini_body").gameObject;
                    m_renderer = t_subBody.GetComponent<Renderer>();
                    m_renderersMats = m_renderer.materials;
                    m_initMat = m_renderersMats[m_matNum];
                }

            }

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                UpdateMatToBHMat();
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                UpdateMatToInitMat();
            }
        }

        public void UpdateMatToInitMat()
        {
            m_renderersMats[m_matNum] = m_initMat;
            m_renderer.materials = m_renderersMats;
        }

        public void UpdateMatToBHMat()
        {
            m_renderersMats[m_matNum] = m_bombHolderMat;
            m_renderer.materials = m_renderersMats;
        }

        public void SetMatID(int _matID)
        {
            m_matNum = _matID;
        }

        public void SetBHMat(Material _bhMat)
        {
            m_bombHolderMat = _bhMat;
        }

        public void SetBody(GameObject _body)
        {
            m_body = _body;
        }
    }
}