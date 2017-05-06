using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class DelBombPoint : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_particle, m_bomb;

        // Use this for initialization
        private void Start()
        {
            m_particle = gameObject.transform.FindChild("BombpointParticle").gameObject;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (m_bomb == null)
            {
                m_bomb = GameObject.Find("LowPolyBomb(Clone)").gameObject;
            }
            else
            {
                print(m_bomb.GetComponent<BombScript>().IsHeld());
                if (m_bomb.GetComponent<BombScript>().IsHeld())
                {
                    m_particle.SetActive(true);
                }
                else
                {
                    m_particle.SetActive(false);
                }
            }
        }
    }
}