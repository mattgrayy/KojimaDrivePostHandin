using UnityEngine;
using System.Collections;

namespace Bird {

    public class moveTowards : MonoBehaviour {

        public GameObject target;
        private Transform targetAdjust;
        public Vector3 adjust;
        private float speed;
        public float damping;

        public bool currentlyFollow;

        public void setup() {
            currentlyFollow = true;
            //for opposite flow needs work before compleate
            //if(target.GetComponent<splineWalkerMod>().goingForward)
            //{
            //    adjust = new Vector3(30.0f, 0.0f, 0.0f);
            //}
            //else
            //{
            //    adjust = new Vector3(0.0f, 0.0f, 0.0f);
            //}
        }

        void Update() {
            if (Vector3.Distance(transform.position, target.transform.position) > 200) {

                currentlyFollow = false;
            }

            if (currentlyFollow) {
                targetAdjust = target.transform;
                targetAdjust.position = targetAdjust.position - adjust;
                speed = Vector3.Distance(transform.position, target.transform.position) - 5;

                Vector3 lookPos = targetAdjust.position - transform.position; ;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetAdjust.position, step);

                transform.LookAt(targetAdjust);
            } else {

            }
        }

        void OnCollisionEnter(Collision _other) {

            if (_other.gameObject.tag == "CAR") {
                print("HITBYCAR");
                currentlyFollow = false;
            }
        }



    }

}