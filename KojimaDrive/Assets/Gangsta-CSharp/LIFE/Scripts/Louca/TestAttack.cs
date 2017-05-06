using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class TestAttack : MonoBehaviour
    {
        public float m_moveSpeed;
        private GameObject m_car;

        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            if (m_car != null)
            {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, m_car.transform.position,
                    Time.deltaTime * m_moveSpeed);
            }
        }

        public void SetCar(GameObject _car)
        {
            m_car = _car;
        }
    }
}