using UnityEngine;
using System.Collections;

namespace Bam
{
    public class CarSockets : MonoBehaviour
    {

        public enum Sockets { Bonnet, Top, LeftDoor, RightDoor, Light_BL, Light_BR, Light_FL, Light_FR, LowFront, LowRear }

        [Header("Bonnet, Top, LeftDoor, RightDoor, Light_BL, Light_BR, Light_FL, Light_FR, LowFront, LowRear")]
        public Transform[] sockets;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public Transform GetSocket(Sockets whichSocket)
        {
            return sockets[(int)whichSocket];
        }
    }
}