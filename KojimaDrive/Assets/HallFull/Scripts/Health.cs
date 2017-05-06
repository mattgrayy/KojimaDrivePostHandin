using UnityEngine;
using System.Collections;

namespace HF
{
    //===================== Kojima Drive - Half-Full 2017 ====================//
    //
    // Author: ADAM MOOREY
    // Purpose: Manages the health of the object the script is attached too
    // Namespace: HALF-FULL
    //
    //===============================================================================//

    public class Health : MonoBehaviour
    {
        public float m_fMaxHealth = 100.0f;
        public float m_fCurrentHealth = 0.0f;
        public float m_fDamageAmount = 20.0f;

        public int m_iDamageCounter;
        private GameObject m_goHealthText;

        public DriveAndSeek m_dasDriveAndSeek;

        void Awake()
        {
            foreach (Transform child in gameObject.transform)
            {
                if (child.name == "PTBScoreText")
                {
                    m_goHealthText = child.gameObject;
                    m_goHealthText.SetActive(true);
                    break;
                }
            }
        }

        void Start()
        {
            m_fCurrentHealth = m_fMaxHealth;
            m_iDamageCounter = 0;

            UpdateText();
            m_dasDriveAndSeek = gameObject.GetComponent<DriveAndSeek>();
        }

        void Update()
        {
            CheckHealth();
        }

        void OnDestroy()
        {
            m_goHealthText.GetComponent<TypogenicText>().Text = "Score";
            m_goHealthText.SetActive(false);
            m_goHealthText = null;
        }

        //check the current health value
        void CheckHealth()
        {
            if (m_fCurrentHealth <= 0.0f)
            {
                m_fCurrentHealth = 0.0f;
                m_dasDriveAndSeek.m_bDead = true;
            }
            else
            {
                m_dasDriveAndSeek.m_bDead = false;
            }
        }

        //decrement the current health
        public void DecreaseHealth()
        {
            if (m_fCurrentHealth > 0.0f)
            {
                m_fCurrentHealth -= m_fDamageAmount;
                UpdateText();
            }
        }

        //set the health to the max health
        public void ResetHealth()
        {
            m_fCurrentHealth = m_fMaxHealth;
            UpdateText();
        }

        //update the score text with current health
        public void UpdateText()
        {
            m_goHealthText.GetComponent<TypogenicText>().Text = "HP: " + m_fCurrentHealth;
        }
    }
}