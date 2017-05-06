using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GCSharp
{
    public class TestCollision : MonoBehaviour
    {
        public GameObject m_obj;
        private List<GameObject> m_objSpawned;
        public float m_time;
        private float m_timer;
        public int m_spawnAmount;
        private GameObject m_car;

        private void Start()
        {
            m_timer = 0;
            m_objSpawned = new List<GameObject>();
        }

        private void Update()
        {
            if (m_car != null)
            {
                m_timer += Time.deltaTime;
                if (m_timer >= m_time)
                {
                    for (int i = 0; i < m_objSpawned.Count; i++)
                    {
                        Destroy(m_objSpawned[i]);
                    }
                    m_timer = 0f;
                    m_objSpawned.Clear();
                    m_car = null;
                }
            }
        }

        private Vector3 RandomCircle(Vector3 center, float radius)
        {
            float ang = Random.value * 360;
            Vector3 pos;
            pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            pos.z = center.z;
            return pos;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.tag == "Player")
            {
                m_car = other.collider.gameObject;
                for (int i = 0; i < m_spawnAmount; i++)
                {
                    GameObject t_Obj =
                        (GameObject)Instantiate(m_obj, RandomCircle(m_car.transform.position, 10f), Quaternion.identity);
                    t_Obj.GetComponent<TestAttack>().SetCar(m_car);
                    m_objSpawned.Add(t_Obj);
                }
            }
        }
    }
}