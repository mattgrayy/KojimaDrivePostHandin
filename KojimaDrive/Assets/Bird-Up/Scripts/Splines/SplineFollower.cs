using UnityEngine;
using System.Collections;
namespace Bird {

    public enum SplineWalkerMode {
        Once,
        Loop,
        PingPong
    }
    public class SplineFollower : MonoBehaviour {
        public BezierSpline spline;

        public float duration;
        public bool lookForward;

        private float progress;
        public float Progress {
            get {
                return progress;
            }
        }

        public SplineWalkerMode mode;

        public bool goingForward = true;

        public bool m_bEaseForwardLooking = false; // Added this to "ease" the turning of an object following a path -sam 17/01/2017
        public float m_fMaximumDegreesOfRotationPerTick = 3.0f;

        [Range(0, 1)]
        public float m_fStartingProgress = 0.0f;

        public void Start() {
            resetProgress(); /*/ duration*/;
        }

        public void resetProgress()
        {
            progress = m_fStartingProgress;
        }


        // Tweaked this here, to allow me to overload things in a child class -sam 17/01/2017
        public void Update() {
            FollowSpline();
        }
        public void FollowSpline() {
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
                if (mode == SplineWalkerMode.Loop) {
                    if (progress < 0f) {
                        progress += 1.0f;
                    }

                    if (progress > 1.0f) {
                        progress -= 1.0f;
                    }
                } else {
                    if (progress < 0f) {
                        progress = -progress;
                        goingForward = true;
                    }
                }
            }

            Vector3 position = spline.GetPoint(progress);
            transform.position = position;
            if (lookForward) {
                if (m_bEaseForwardLooking) {
                    Quaternion rotation = transform.rotation;
                    Quaternion targetRotation = transform.rotation;
                    targetRotation.SetLookRotation(spline.GetDirection(progress));

                    // TODO: Add banking! -sam 17/01/2017
                    transform.rotation = Quaternion.RotateTowards(rotation, targetRotation, m_fMaximumDegreesOfRotationPerTick);
                } else {
                    transform.LookAt(position + spline.GetDirection(progress));
                }
            }
        }
    }

}