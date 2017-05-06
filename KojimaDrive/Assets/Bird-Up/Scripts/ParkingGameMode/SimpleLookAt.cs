using UnityEngine;
using System.Collections;

namespace Bird
{
    public class SimpleLookAt : MonoBehaviour
    {

        public Transform target;

        public void tick()
        {
            transform.LookAt(target);
        }

    }
}
