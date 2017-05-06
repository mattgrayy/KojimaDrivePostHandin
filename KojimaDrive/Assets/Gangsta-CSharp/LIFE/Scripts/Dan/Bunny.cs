using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class Bunny : MonoBehaviour
    {
        Rigidbody rb;
        public Vector3 dir;
        public float force;
        public int raycast;
        public GameObject bunnys;
        private bool male = true;
        void Start()
        {

            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            force = Random.Range(1, 3);
            if (Physics.Raycast(transform.position, Vector3.down, raycast))
            {
                //wait
                rndXValue();
                rb.AddForce(dir * force);
            }
            else
            {
                rb.AddForce(-dir * force / 2);
            }
        }

        private void rndXValue()
        {
            dir.x = Random.Range(-45f, 45f);
        }
        void OnCollisionEnter(Collision col)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.x += 10f;
            spawnPos.y += 10f;


            if (col.gameObject.tag == "Player")
            {
                Instantiate(bunnys, spawnPos, transform.rotation);
            }

        }
    }
}