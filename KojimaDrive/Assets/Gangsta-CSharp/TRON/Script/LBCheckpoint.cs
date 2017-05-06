using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class LBCheckpoint : MonoBehaviour
    {
        [SerializeField]
        private LBCheckpointManager m_lbcheckManager;

        private GameObject m_particle;

        // Use this for initialization
        private void Start()
        {
            m_particle = gameObject.transform.GetChild(0).gameObject;
            m_particle.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void SetManager(GameObject _managerObject)
        {
            m_lbcheckManager = _managerObject.GetComponent<LBCheckpointManager>();
        }

        public void TurnParticleOn()
        {
            m_particle.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && m_lbcheckManager.GetCurrent() == gameObject)
            {
                other.gameObject.GetComponent<LightBlade>().AddScore(100);
                m_particle.SetActive(false);
                m_lbcheckManager.CheckPointReached();
            }
        }
    }
}