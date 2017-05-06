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
                    volcanoMadness.EndRound(winCondition);
                }
            }
        }
    }
}