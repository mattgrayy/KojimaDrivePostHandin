using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace KRace
{
    public class CharacterScore : MonoBehaviour
    {

        public Text scoreText;
        int score;
        Timer gameTimer;

        // Use this for initialization
        void Start()
        {
            //Set start score (0)
            score = 0;

            //Find the timer object and script
            gameTimer = GameObject.Find("Timer").transform.GetComponent<Timer>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        //Increment Score and Update Text
        public void addScore()
        {
            //Only increment score if timer is > 0

            if (gameTimer.timerTime > 0.0f)
            {
                score++;
                updateText();
            }
        }

        //Update score text
        void updateText()
        {
            scoreText.text = score.ToString();
        }
    }
}