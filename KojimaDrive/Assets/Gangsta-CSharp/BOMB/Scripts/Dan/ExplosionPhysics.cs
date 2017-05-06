using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class ExplosionPhysics : MonoBehaviour
    {
        public GameObject bombRef;
        public float radius;
        public float powerMultiplier;
        public bool m_testing;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (m_testing)
            {
                if (Input.GetKeyDown("space"))
                { trigger(); }
            }
        }
        public void trigger()
        {

            getObjInRadius(bombRef.transform.position, radius);
        }



        private void getObjInRadius(Vector3 center, float rad)
        {
            //gets all colliders within radius

            Collider[] hitColliders = Physics.OverlapSphere(center, rad);

            int i = 0;
            float distance;
            float power;

            //loops all colliders
            while (i < hitColliders.Length)
            {
                //gets distance between bomb and object
                distance = getDistance(hitColliders[i].transform.position);
                //gets inverted % of distance away
                //so edge of radius is 0% & center is 100% 
                power = 100 - ((distance / rad) * 100);
                //pushes away based on power
                pushAway(power, hitColliders[i].gameObject);
                i++;
            }


        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, radius);
        }
        private float getDistance(Vector3 dis)
        {
            return Vector3.Distance(bombRef.transform.position, dis);
        }

        private void pushAway(float force, GameObject objectToPush)
        {
            if (objectToPush.GetComponent<Rigidbody>() != null)
            {
                Rigidbody rb = objectToPush.GetComponent<Rigidbody>();
                //calculate direction to push object
                Vector3 direction = (objectToPush.transform.position - bombRef.transform.position).normalized;
                //adds force to object away from bomb by %power
                rb.AddForce(direction * (force * powerMultiplier));
            }

        }

        //-get all cars/ enviroment objects in radius 
        //-add force in oposite direction to bomb
        //-stronger on closer objects

    }
}