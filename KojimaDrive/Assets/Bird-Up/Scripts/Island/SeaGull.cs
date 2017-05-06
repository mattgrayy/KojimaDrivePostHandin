using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bird {

    public class SeaGull : SplineFollower {

        Animator m_AnimController;

        // Flock ID
        public int m_nFlock = 0;

        public bool m_bFleeing = false;
        bool m_bReturning = false;
        Vector3 m_FleePosition;
        float m_fFleeRadius = 20.0f;
        float m_fFleeSpeed = 5.0f;

        // If no spline is present, follow flock leader
        public SeaGull m_LeaderOfFlock;

        // Ideally this'd be static or somekind of singleton but I just wanna make this quick
        List<SeaGull> m_MyFlock = new List<SeaGull>();

        void LinkFlock() {
            SeaGull[] allGulls = FindObjectsOfType<SeaGull>();
            foreach (SeaGull gul in allGulls) {
                if (gul.m_nFlock == m_nFlock) {
                    m_MyFlock.Add(gul);
                }
            }
        }

        void Start() {
            LinkFlock();
            m_AnimController = transform.GetChild(0).GetComponent<Animator>();

            if (spline == null) {
                transform.position = m_LeaderOfFlock.spline.GetPoint(m_LeaderOfFlock.Progress);
                transform.TransformPoint(m_StartingFollowDist);
            } else {
                base.Start();
            }
        }

        public void Flee(bool bFlockFlee, Vector3 fleeDest) {
            if (!m_bFleeing) {
                Debug.Log("OH SHI");
                if (bFlockFlee) {
                    for (int i = 0; i < m_MyFlock.Count; i++) {
                        m_MyFlock[i].Flee(false, fleeDest);
                    }
                }

                m_FleePosition = (Random.insideUnitSphere * m_fFleeRadius) + fleeDest;
                m_AnimController.speed = m_AnimController.speed * 2.0f; // Speed multiplier
                m_bFleeing = true;
            }
        }

        private Vector3 velocity = Vector3.zero;
        public float m_fFollowSpeed = 2.0f;
        public Vector3 m_StartingFollowDist;

        // Update is called once per frame
        new void Update() {
            if (m_bFleeing) {
                if (m_bReturning) {
                    Vector3 pos = transform.position;
                    FollowSpline(); // doing this so I can go to back to the right place!
                    Vector3 targetPos = transform.position;
                    transform.position = Vector3.MoveTowards(pos, targetPos, m_fFleeSpeed * 0.75f);
                    if (transform.position == targetPos) {
                        m_bReturning = false;
                        m_bFleeing = false;
                    }
                } else {
                    Vector3 pos = transform.position;
                    FollowSpline(); // doing this so I can go to back to the right place!
                    transform.position = Vector3.MoveTowards(pos, m_FleePosition, m_fFleeSpeed);
                    if (transform.position == m_FleePosition) {
                        // We've escaped! Now return to our path
                        m_bReturning = true;
                    }
                }
            } else {
                if (spline == null) {
                    transform.position = Vector3.SmoothDamp(transform.position, m_LeaderOfFlock.transform.position, ref velocity, (1 / duration) * m_fFollowSpeed);

                    Quaternion rotation = transform.rotation;
                    Quaternion targetRotation = transform.rotation;
                    targetRotation.SetLookRotation(transform.position - m_LeaderOfFlock.transform.position);
                    transform.rotation = Quaternion.RotateTowards(rotation, targetRotation, m_fMaximumDegreesOfRotationPerTick);
                } else {
                    FollowSpline();
                }
            }
        }
    }


}