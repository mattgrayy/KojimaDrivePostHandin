//Author:       Tom Moreton
//Description:  Script that calculates how long the car has been in the air for
//Last edit:    Tom Moreton @ 05/02/2017

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bam
{
    public class CalculateAirtime : MonoBehaviour
    {
        public float m_fCurrentAirtime;
        public float m_fLastAirtime;
        public List<float> m_lPreviousAirtimes;

        float m_fAirtimeCalculateDelay;
        float m_fCurrentTimer;
        bool m_bInAir;
        Kojima.CarScript m_csCarScript;
        Rigidbody m_rbRigidbody;

        // Use this for initialization
        void Start()
        {
            m_csCarScript = gameObject.GetComponent<Kojima.CarScript>();
            m_rbRigidbody = gameObject.GetComponent<Rigidbody>();
            m_bInAir = false;
            m_fAirtimeCalculateDelay = 2.0f;
            m_fCurrentTimer = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            //Checks to see if the object is touching the ground. Velocity is checked to ensure airtime isn't calculated whilst the vehicle is upside down.
            if (m_csCarScript.InMidAir == true && m_rbRigidbody.velocity.magnitude > 2.0f)
            {
                //Won't start calculating airtime until an intial delay has taken place
                if (m_fCurrentTimer < m_fAirtimeCalculateDelay)
                {
                    m_fCurrentTimer += Time.deltaTime;
                }
                else
                {
                    m_fCurrentAirtime += Time.deltaTime;
                }

                if (VolcanoMadnessCanvas.singleton)
                {
                    VolcanoMadnessCanvas.singleton.GiveAirTime(m_fCurrentAirtime + m_fCurrentTimer);
                }
            }
            else
            {
                //Adds the previous airtime to a public list that can be accessed
                if (m_fCurrentAirtime > 0.0f)
                {
                    m_fLastAirtime = m_fCurrentAirtime;
                    //m_lPreviousAirtimes.Add(m_fLastAirtime);
                    m_fCurrentTimer = 0.0f;
                }
                m_fCurrentAirtime = 0.0f;
            }
        }
    }
}
