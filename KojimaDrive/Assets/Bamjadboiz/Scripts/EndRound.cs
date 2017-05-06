using UnityEngine;
using System.Collections;

namespace Bam
{
    public class EndRound : MonoBehaviour {

        public bool winCondition;

        [SerializeField]
        VolcanoMadness volcanoMadness;

        void Start()
        {
            //volcanoMadness = transform.parent.gameObject;
        }

        void OnTriggerEnter(Collider col)
        {
            Kojima.CarScript c;

            c = col.gameObject.GetComponent<Kojima.CarScript>();

            if (volcanoMadness.gamemodeRunning == true && c)
            {
                if (c.m_nplayerIndex == volcanoMadness.GetRunnerID+1)
                {
                    if (winCondition == false)
                    {
                        Debug.Log("Runner: " + col.gameObject.name + " is out of bounds!");
                    }
                    else
                    {
                        Debug.Log("Runner: " + col.gameObject.name + " reached the end!");
                    }

                    volcanoMadness.EndRound(winCondition);
                }
                else
                {
                    if (col.gameObject.tag == "Player")
                    {
                        Debug.Log("Blocker: " + col.gameObject.name + " is out of bounds!");
                    }
                }
            }
        }
    }
}