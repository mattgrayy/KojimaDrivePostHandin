using UnityEngine;
using UnityEngine.UI;

namespace Bam
{
    public class CountdownScript : MonoBehaviour
    {
        public Text m_countdown;

        int m_nPrevNum; 

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetCountdown(int countdownVal)
        {
            if(countdownVal == 0)
            {
                m_countdown.text = "GO";
            }
            else
            {
                m_countdown.text = "" + countdownVal;
            }

            if(countdownVal == m_nPrevNum)
            {
                m_countdown.transform.localScale = Vector3.Lerp(m_countdown.transform.localScale, Vector3.zero, Time.deltaTime * 2.8f);
            }
            else
            {
                m_countdown.transform.localScale = Vector3.one;
            }

            m_nPrevNum = countdownVal;
        }
    }
}
