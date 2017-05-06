using UnityEngine;
using System.Collections;

namespace HF
{
    //===================== Kojima Drive - Half-Full 2017 ====================//
    //
    // Author: SAM BRITNALL, MATTHEW WYNTER, ADAM MOOREY
    // Purpose: Manages chaser activity, specifically in damaging chasers and runners
    // Namespace: HALF-FULL
    //
    //===============================================================================//

    public class Chaser : MonoBehaviour
    {
        public int m_iPlayerNumber;
        int m_iDamageApplied = 0;
        int m_iDamageDealt = 0;

        float m_fTimer = 2.0f;

        public string m_sHiderTag;
        public string m_sChaserTag;

        void Start()
        {
            m_iPlayerNumber = GetComponent<DriveAndSeek>().m_iPlayerNumber;
            m_sChaserTag = gameObject.tag;
        }

        void Update()
        {
            if (m_iDamageApplied > 0 && m_fTimer > 0.0f)
            {
                m_fTimer -= Time.deltaTime;
            }
            else
            {
                m_iDamageApplied = 0;
                m_fTimer = 2.0f;
            }
        }

        void OnTriggerEnter(Collider _collider)
        {
            CheckCollision(_collider);
        }

        //apply damage to this chaser
        void DamageSeeker()
        {
            if (m_iDamageApplied == 0)
            {
                GetComponent<Health>().DecreaseHealth();
                m_iDamageApplied++;
            }
        }

        //apply damage to the hider
        void DamageHider(Collider other)
        {
            if (m_iDamageApplied == 0)
            {
                other.gameObject.GetComponent<Health>().DecreaseHealth();
                m_iDamageApplied++;
                m_iDamageDealt++;
				Kojima.GameController.s_singleton.m_players[gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex - 1].PlayerEXP.AddEXP(50);

				//add XP for final blow
				if (other.gameObject.GetComponent<Health>().m_fCurrentHealth <= 0.0f) 
				{
					Kojima.GameController.s_singleton.m_players[gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex - 1].PlayerEXP.AddEXP(75);
				}
            }
        }

        //check for the collision between this gameobject and another
        void CheckCollision(Collider _collider)
        {
            //if hider hasn't been hit, damage the seeker, unless it's the ground or something random
            if (_collider.gameObject.GetComponent<Kojima.CarScript>())
            {
                if (_collider.gameObject.GetComponent<DriveAndSeek>().m_bRunner)
                {
                    DamageHider(_collider);
                }
                else if (_collider.gameObject.GetComponent<DriveAndSeek>().m_bChaser)
                {
                    DamageSeeker();
                }
            }
        }

        public int ReturnDamageDealt()
        {
            return m_iDamageDealt;
        }
    }
}