using UnityEngine;
using System.Collections;

namespace Bird
{

    public class ParkingGameModeTracker : MonoBehaviour
    {   
        [SerializeField]    
        int score = 0;
        int currentWins=0;

        bool currentHighest;

        public void setCurrentHigh(bool _in)
        {
            
            currentHighest = _in;
        }

        public void addScore(int _in)
        {
           
            score += _in;
        }

        public void resetScore()
        {
            score = 0;
        }

        public int getWins()
        {
            return currentWins;
        }

        public int getScore()
        {
            return score;
        }

        public void addWin()
        {
       
            currentWins++;
        }
        public void resetWins()
        {
            currentWins = 0;
        }


    }
}