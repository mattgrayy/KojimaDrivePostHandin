using UnityEngine;
using System.Collections;

namespace HF
{
    public class ShootBeam : MonoBehaviour
    {

        public static ShootBeam sb_instance;
        LineRenderer line;
        // Use this for initialization
        void Start()
        {
            line = GetComponent<LineRenderer>();

            if (sb_instance == null)
            {
                sb_instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DisplayLine()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                line.SetPosition(1, new Vector3(0, 500, 0));
            }
        }
    }
}