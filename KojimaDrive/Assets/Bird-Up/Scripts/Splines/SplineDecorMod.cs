using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bird {

    //Taken From:
    //http://catlikecoding.com/unity/tutorials/curves-and-splines/

    public class SplineDecorMod : MonoBehaviour {
        public List<Transform> CreatedObjects;
        public BezierSpline spline;

        public int frequency;

        public bool lookForward;

        public Transform[] items;

        public float interSpace;

        private void Awake() {
            interSpace = 1.0f / frequency;
            bool dir = false;
            CreatedObjects = new List<Transform>();
            if (frequency <= 0 || items == null || items.Length == 0) {
                return;
            }
            float stepSize = frequency * items.Length;
            if (spline.Loop || stepSize == 1) {
                stepSize = 1f / stepSize;
            } else {
                stepSize = 1f / (stepSize - 1);
            }

            for (int p = 0, f = 0; f < frequency; f++) {
                for (int i = 0; i < items.Length; i++, p++) {
                    Transform item = Instantiate(items[i]) as Transform;

                    float hold = f + 1;


                    dir = !dir;
                    Vector3 position = spline.GetPoint(p * stepSize);
                    item.transform.localPosition = position;
                    if (lookForward) {
                        item.transform.LookAt(position + spline.GetDirection(p * stepSize));
                    }
                    //   item.transform.parent = transform;
                    CreatedObjects.Add(item);
                    item.GetComponent<splineWalkerMod>().passSpD(this, (hold * interSpace), dir);

                }
            }
        }

        public Transform findClosest(Transform _inTr) {
            float currentClosest = 9999;
            int closestPos = 0;
            int i = 0;
            foreach (Transform tr in CreatedObjects) {
                float testDist = Vector3.Distance(tr.position, _inTr.position);
                if (testDist < currentClosest) {
                    currentClosest = testDist;
                    closestPos = i;
                }
                i++;

            }
            return CreatedObjects[closestPos];

        }



    }


}