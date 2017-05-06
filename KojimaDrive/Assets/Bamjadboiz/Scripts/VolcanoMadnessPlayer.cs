using UnityEngine;
using System.Collections;
namespace Bam
{
    [System.Serializable]
    public class VolcanoMadnessPlayer
    {
        public GameObject myObject;
        int myID;
        public float myAirtime;

        public enum Role
        {
            Runner,
            Blocker
        }

        public Role myRole;

        public VolcanoMadnessPlayer(GameObject gameobject, int ID)
        {
            myObject = gameobject;
            myID = ID;
            myRole = Role.Blocker;
        }



    }
}

