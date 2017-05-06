using UnityEngine;
using System.Collections;

namespace Bird
{
    public class RotateCarArrow : MonoBehaviour
    {
        private Transform target;
        private ArrowCheckpoints checkpointList;
        public float speed;

        void Start()
        {
            checkpointList = GetComponent<ArrowCheckpoints>();
            target = checkpointList.CurrentActiveCheckpoint;
        }

        void Update()
        {
            if (target != null)
            {
                gameObject.transform.LookAt(target);
            }

            if (target != checkpointList.CurrentActiveCheckpoint)
            {
                target = checkpointList.CurrentActiveCheckpoint;
            }
        }
    }
}