using UnityEngine;
using System.Collections;

namespace Bam
{
    public class BamBowlingPinScript : MonoBehaviour
    {
        bool hit = false;
        bool inWater = false;

        MultiAudioSource mySource;
        public GameObject splashPrefab;

        // Use this for initialization
        void Start()
        {
            mySource = GetComponent<MultiAudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if(!inWater)
            {
                if(transform.position.y <= Kojima.CarScript.m_waterYPosition + 2.2f)
                {
                    EnterWater();
                }
            }

            if(!hit)
            {
                if (transform.forward.y < 0.8f)
                {
                    GetHit();
                }
                    //Debug.Log(transform.up);
            }
        }

        void GetHit()
        {
            if (!hit)
            {
                hit = true;
                //Debug.Log(gameObject.name + " transofmr up is " + transform.up);
                BowlingHUDScript.singleton.PinHasBeenHit();
                //gameObject.SetActive(false);
            }
        }

        void EnterWater()
        {
            inWater = true;

            GameObject splashEffectInstance = Instantiate<GameObject>(splashPrefab);
            splashEffectInstance.transform.position = transform.position;
            splashEffectInstance.transform.rotation = Quaternion.LookRotation(Vector3.up);
            Destroy(splashEffectInstance, 4);

            GetHit();
        }

        void OnCollisionEnter(Collision col)
        {
            if (!hit)
            {
                if (col.relativeVelocity.magnitude >= 20 && col.gameObject.CompareTag("Player"))
                {
                    mySource.Play();
                    VolcanoMadness.singleton.EndRoundEarly(2);
                    //hit = true;

                    
                }
                else
                {
                    //Debug.Log(col.relativeVelocity + " not enough for the sound effect");
                }
            }

        }
    }
}