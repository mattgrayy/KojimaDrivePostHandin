using UnityEngine;
using System.Collections;

namespace GCSharp
{
    public class Score : MonoBehaviour
    {

        [SerializeField]
        private int score;
        private GameObject scoreText;
        private TypogenicText scoreTT;

        // Use this for initialization
        void Start()
        {
            scoreText = gameObject.transform.FindChild("PTBScoreText").gameObject;
            scoreTT = scoreText.GetComponent<TypogenicText>();
            scoreTT.Text = "Score: 0";
            score = 0;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddScore(int _newScore, int _playerId)
        {
            score += _newScore;
            HF.PlayerExp.AddEXP(_playerId, _newScore, true, true, "Bomb Carrier", false);
            scoreTT.Text = "Score: " + score;
        }

        public void ResetScore()
        {
            score = 0;
            scoreTT.Text = "Score: " + score;
        }

        public int GetPlayerScore()
        {
            return score;
        }
    }
}