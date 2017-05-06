using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Bam
{
    public class SabotageHUDScript : MonoBehaviour
    {
        [SerializeField]
        Text timerText;
        [SerializeField]
        Text hintText2;
        [SerializeField]
        Text hintText1;

        bool m_fadeHints, m_isWarned;
        float m_fFadeTimer = 0.0f;
        Color hintColor;
        float m_fCircleTimer;

        public static SabotageHUDScript singleton;

        void Awake()
        {
            singleton = this;
            timerText.text = "";
            timerText.enabled = false;
            hintColor = hintText2.color;

            hintText1.text = "";
            hintText2.text = "";
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (timerText.text == "ROUND OVER!")
            {
                timerText.transform.localScale = Vector3.Lerp(timerText.transform.localScale, Vector3.one * (1.2f + (Mathf.Sin(Time.timeSinceLevelLoad*2) * 0.3f)), 10 * Time.deltaTime);
            }
            else
            {
                timerText.transform.localScale = Vector3.Lerp(timerText.transform.localScale, Vector3.one, 10 * Time.deltaTime);
            }

            if(m_fadeHints)
            {
                HandleFade();
            }

            if(m_fCircleTimer == 0)
            {
                HandleFade();
            }
        }

        public void GiveOutOfCircleTimer(float t)
        {
            m_fCircleTimer = t;
        }

        void HandleFade()
        {
            if (m_fFadeTimer > 0)
            {
                m_fFadeTimer -= Time.deltaTime;
            }
            else
            {
                FadeHints();

                if (hintText1.color.a <= 0.0f)
                {
                    m_fadeHints = false;
                    m_fFadeTimer = 5.0f;
                }
            }
        }

        public float GetFadeTimer
        {
            get { return m_fFadeTimer; }
        }

        public void Toggle(bool on)
        {
            timerText.enabled = on;
        }

        public void GiveTimeValue(int seconds)
        {
            if (seconds>=0)
            {

                string oldText = timerText.text;

                timerText.text = "00:" + seconds;

                if (seconds < 10)
                {
                    timerText.text = "00:0" + seconds;
                }

                if (timerText.text != oldText && timerText.text!="00:00")
                {
                    timerText.transform.localScale = Vector3.one * 1.25f;
                }

                if (timerText.text == "00:00")
                {
                    timerText.text = "ROUND OVER!";
                }

                if(!Kojima.CarScript.s_playersCanMove)
                {
                    timerText.text = "";
                }
            }
        }

        public void DisplayStartHints(int currentRunner)
        {
            currentRunner++;

            hintText2.text = "Keep P" + currentRunner + " out of the zone";
            hintText1.text = "P" + currentRunner + " stay in the safe zone";
            m_fadeHints = true;
            m_fFadeTimer = 4;
        }

        void FadeHints()
        {
            hintText1.color = Color.Lerp(hintText1.color, Color.clear, Time.deltaTime * 1.5f);
            hintText2.color = Color.Lerp(hintText2.color, Color.clear, Time.deltaTime * 1.5f);
        }

        public void WarnChaser()
        {
            hintText1.color = hintColor;
        }

        public void ResetHints()
        {
            hintText1.color = hintColor;
            hintText2.color = hintColor;
        }
    }
}