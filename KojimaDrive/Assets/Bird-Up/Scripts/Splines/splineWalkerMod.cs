using UnityEngine;
using System.Collections;

namespace Bird {
    public class splineWalkerMod : MonoBehaviour {

        public bool lookForward;
        public enum SplineWalkerMode {
            Once,
            Loop,
            PingPong
        }
        public SplineWalkerMode mode;

        public bool goingForward = true;

        public BezierSpline spline;

        public float duration;

        public float progress;



        bool Direction;

        public GameObject carToMake;
        GameObject go;
        bool begin = true;

        void LateUpdate() {
            if (begin) {
                go.transform.position = transform.position;
                go.GetComponent<moveTowards>().currentlyFollow = true;
                begin = false;
            }

        }

        public void passSpD(SplineDecorMod _spDIn, float _position, bool _direction) {
            spline = _spDIn.GetComponent<BezierSpline>();


            progress = _position;
            goingForward = _direction;
            go = Instantiate(carToMake);

            go.GetComponent<moveTowards>().target = (gameObject);
            go.GetComponent<moveTowards>().setup();
        }


        private void Update() {
            if (goingForward) {
                progress += Time.deltaTime / duration;
                if (progress > 1f) {
                    if (mode == SplineWalkerMode.Once) {
                        progress = 1f;
                    } else if (mode == SplineWalkerMode.Loop) {
                        progress -= 1f;
                    } else {
                        progress = 2f - progress;
                        goingForward = false;
                    }
                }
            } else {
                progress -= Time.deltaTime / duration;
                if (progress < 0f) {
                    progress = 1;
                    //goingForward = true;
                }
            }

            Vector3 position = spline.GetPoint(progress);
            transform.localPosition = position;
            if (lookForward) {
                transform.LookAt(position + spline.GetDirection(progress));
            }
        }
    }

}