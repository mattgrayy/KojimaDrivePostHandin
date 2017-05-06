using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HF
{
    public class AddOnManagerHF : MonoBehaviour
    {
        public static AddOnManagerHF m_instance = null;
        [SerializeField]

        Transform foxears;
        private GameObject foxEars;

        List<Transform> cars = new List<Transform>();

        public enum AddonType_e
        {
            NONE = 0,
            FOX_EARS = 1
        }

        // Use this for initialization
        void Start()
        {
            if (m_instance)
            {
                Destroy(this.gameObject);
            }
            else
            {
                m_instance = this;
            }
            setupPlayers();
        }

        public void addEarsToCar(Transform _Car)
        {
            addFoxEars(_Car);
        }

        //this will need to be tested with the new version
        public void removeAddOns(Transform _Car)
        {
            for (int i = 0; i <= _Car.transform.childCount - 1; i++)
            {
                if (_Car.transform.GetChild(i).name == "FoxEars(Clone)")
                {
                    foxEars = transform.GetChild(i).gameObject;
                    Destroy(foxEars);
                }
            }
        }

        private void setupPlayers()
        {
            int playerCount = Kojima.GameController.s_singleton.m_players.Length;

            for (int i = 0; i < playerCount - 1; i++)
            {
                if (Kojima.GameController.s_singleton.m_players[i] != null)
                {
                    cars.Add(Kojima.GameController.s_singleton.m_players[i].transform);
                }
            }
        }

        //add the ears to the top socket of the car
        public void addFoxEars(Transform _Car)
        {
            if (_Car != null)
            {
                Vector3 spawnPos = _Car.GetComponent<Bam.CarSockets>().GetSocket(Bam.CarSockets.Sockets.Top).position;
                Transform newFoxEars = Instantiate(foxears, spawnPos, Quaternion.identity) as Transform;
                newFoxEars.parent = _Car;

            }
        }
    }
}
