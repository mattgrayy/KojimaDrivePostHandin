using UnityEngine;
using System.Collections;

namespace HF
{
    public class testExp : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {

                for (int iter = 0; iter <= Kojima.GameController.s_singleton.m_players.Length - 1; iter++)
                {
					HF.PlayerExp.GetPlayerEXP(iter).AddEXP(50);
                }
                //GameObject.Find("ExperienceManager").GetComponent<ExperienceManager>().saveAllCarChanges();
            }
        }
    }
}