using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Bam
{
    public class VolcanoMadnessCanvas : MonoBehaviour
    {
        [SerializeField]
        Text hintText;

        Color hintColour;
        float m_fFadeTimer = 7.0f;
        bool m_fadeHints;
        public Vector3 startPos, endPos;
        bool m_showHint;
        public static VolcanoMadnessCanvas singleton;

        [SerializeField]
        Text timerText;

        [SerializeField]
        TypogenicText airtimeText;

        float curTime = 0;

        // Use this for initialization
        void Awake()
        {
            singleton = this;
            hintColour = hintText.color;
            hintText.color = Color.clear;

            timerText.text = "";
            timerText.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (timerText.text == "ROUND OVER!")
            {
                timerText.transform.localScale = Vector3.Lerp(timerText.transform.localScale, Vector3.one * (1.2f + (Mathf.Sin(Time.timeSinceLevelLoad * 2) * 0.3f)), 10 * Time.deltaTime);
            }
            else
            {
                timerText.transform.localScale = Vector3.Lerp(timerText.transform.localScale, Vector3.one, 10 * Time.deltaTime);
            }

            if (m_fadeHints)
            {
                HandleFade();
            }

            if(m_showHint)
            {
                hintText.transform.localPosition = Vector3.Lerp(hintText.transform.localPosition, endPos, Time.deltaTime * 5.0f);

                if(Vector3.Distance(hintText.transform.localPosition, endPos) < 1.05f)
                {
                    m_showHint = false;
                }
            }
        }

        public void GiveAirTime(float time)
        {
            if(airtimeText)
            {
                airtimeText.Text = "";
            }

            //if(!airtimeText)
            //{
            //    return;
            //}

            //if (time > curTime)
            //{
            //    airtimeText.Text = "Airtime: 00:";

            //    //int timeInSeconds = Mathf.RoundToInt(time);

            //    if (time < 10)
            //    {
            //        airtimeText.Text += "0";
            //    }

            //    airtimeText.Text += time;

            //    curTime = time;
            //}
        }

        public void ResetAirTime()
        {
            curTime = 0;
            airtimeText.Text = "Airtime: 00:00";
        }

        public void Toggle(bool on)
        {
            timerText.enabled = on;
        }

        public void GiveTimeValue(int seconds, bool hide = false)
        {
            if (seconds >= 0)
            {
                string oldText = timerText.text;

                timerText.text = "00:" + seconds;

                if (seconds < 10)
                {
                    timerText.text = "00:0" + seconds;
                }

                if (timerText.text != oldText && timerText.text != "00:00")
                {
                    timerText.transform.localScale = Vector3.one * 1.25f;
                }

                if (timerText.text == "00:00")
                {
                    timerText.text = "ROUND OVER!";
                }

                if (!Kojima.CarScript.s_playersCanMove || hide)
                {
                    timerText.text = "";
                }
            }
        }

        public void SetHintText(string hintString)
        {
            ResetHints();
            hintText.transform.localPosition = startPos;
            hintText.text = hintString;
            m_showHint = true;
        }

        public void HideHints()
        {
            m_fadeHints = true;
            m_fFadeTimer = 3.0f;
        }

        public void ClearHints()
        {
            hintText.color = Color.clear;
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

                if (hintText.color.a <= 0.02f)
                {
                    m_fadeHints = false;
                    ClearHints();
                }
            }
        }

        void FadeHints()
        {
            hintText.color = Color.Lerp(hintText.color, Color.clear, Time.deltaTime * 5.3f);
        }

        public void ResetHints()
        {
            hintText.color = hintColour;
        }
    }
}
