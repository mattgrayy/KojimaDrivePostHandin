using UnityEngine;
using System.Collections;
//using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Bam
{
    public class BamResultsScript : MonoBehaviour
    {
        public static BamResultsScript singleton;

        [SerializeField]
        Transform section1, section2;
        float spd = 1;

        [SerializeField]
        int section = 0;

        [SerializeField]
        float sectionTimer;

        [System.Serializable]
        public struct ScoreIdentity
        {
            public string playerName;
            public int playerScore;
        }

        void Awake()
        {
            if (singleton == null)
                singleton = this;
            else
                Destroy(gameObject);
        }

        [SerializeField]
        ScoreIdentity[] allScores;
    
        [SerializeField]
        TypogenicText winnerText, winnerIsText;
        [SerializeField]
        TypogenicText leaderboardText, gameNameText, returningToLobby;

        [SerializeField]
        SpriteRenderer background;

        Camera myCam;
        Vector3 camPos = new Vector3(-1015, 463, -967), camEuler = new Vector3(9.1f, 40.6f, 0);

        [SerializeField]
        AudioClip crowdSound, otherSnd;

        AudioSource snd;

        [SerializeField]
        Kojima.GameMode myGameMode;

        [SerializeField]
        ParticleSystem winnerName;

        // Use this for initialization
        void Start()
        {
            //int[] test = new int[4] { 23, 100, 12, 433 };
            //SetName("Bowling Madness");

            //GiveScores(test);

            winnerIsText.transform.localPosition += Vector3.up * 350;
            background.color = Color.clear;
            leaderboardText.Text = "";
            returningToLobby.Size = 0;

            myCam = GetComponentInChildren<Camera>();
            myCam.transform.SetParent(null);
            myCam.transform.position = camPos;
            myCam.transform.eulerAngles = camEuler;

            myCam.rect = new Rect(0, 1, 1, 1);

            snd = GetComponent<AudioSource>();
        }

        void OnDestroy()
        {
            if (myCam)
            {
                Destroy(myCam.gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            switch(section)
            {
                case 1:
                    Section1();
                    break;
                case 2:
                    Section2();
                    break;
            }

            winnerIsText.transform.localPosition = Vector3.Lerp(winnerIsText.transform.localPosition, new Vector3(0, 230, 0), 8 * Time.deltaTime);

            winnerText.transform.localScale = Vector3.one * (1 + (Mathf.Sin(Time.timeSinceLevelLoad * 2) * 0.1f));
            background.color = Color.Lerp(background.color, new Color(0, 0, 0, 0.645f), 4 * Time.deltaTime);

            sectionTimer -= Time.deltaTime;

            if(sectionTimer<=0)
            {
                NextSection();
            }

            Rect n = myCam.rect;
            n.y = Mathf.Lerp(n.y, 0, 6 * Time.deltaTime);
            myCam.rect = n;
        }

        public void GiveScores(int[] scores)
        {
            allScores = new ScoreIdentity[scores.Length];

            for(int i=0; i<scores.Length; i++)
            {
                allScores[i].playerName = "Player " + (i + 1);
                allScores[i].playerScore = scores[i];
            }

            SortList();

            if(section==0)
            {
                NextSection();
            }
        }

        void SortList()
        {
            Array.Sort(allScores, delegate (ScoreIdentity s1, ScoreIdentity s2) { return s2.playerScore.CompareTo(s1.playerScore); });
        }

        public static BamResultsScript ShowResults(Kojima.GameMode gameModeScript, string gameModeName = "")
        {
            BamResultsScript newResults;
           

            GameObject newResultsObj = GameObject.Instantiate<GameObject>(Kojima.GameController.s_singleton.resultsScreenPrefab);
            newResults = newResultsObj.GetComponent<BamResultsScript>();

            newResults.myGameMode = gameModeScript;

            if(newResults.myGameMode)
            {
                newResults.myGameMode.gameObject.SetActive(false);
            }

            newResults.SetName(gameModeName);

            return newResults;
        }

        public void NextSection()
        {
            spd = 5;
            //Debug.Break();

            switch (section)
            {
                case 0:
                    winnerText.Text = allScores[0].playerName;
                    //winnerText.Tracking = 100;
                    winnerText.Size = 0;
                    sectionTimer = 6.5f;
                    break;
                case 1:
                    for (int i = 0; i < allScores.Length; i++)
                    {
                        leaderboardText.Text += "" + allScores[i].playerName + "     " + allScores[i].playerScore + "\n";
                    }

                    sectionTimer = 8;
                    //leaderboardText.Leading = 355;
                    break;
                case 2:
                    Return();
                    sectionTimer = 99;
                    break;
            }

            if (section < 2)
                section++;
        }

        void FillInText()
        {

        }

        public void SetName(string name)
        {
            gameNameText.Text = name;
        }

        void Section1()
        {
            if(sectionTimer<4)
            {
                if(winnerText.Size == 0 && !snd.isPlaying)
                {
                    snd.PlayOneShot(crowdSound);
                    snd.PlayOneShot(otherSnd);
                    winnerName.Play();
                }

                winnerText.Tracking = Mathf.Lerp(winnerText.Tracking, 0, 9 * Time.deltaTime);
                winnerText.Size = Mathf.Lerp(winnerText.Size, 700, 25 * Time.deltaTime);
            }

            if (spd < 100)
                spd *= 1 + (Time.deltaTime * 5);

            leaderboardText.enabled = false;
        }

        void Section2()
        {
            leaderboardText.enabled = true;
            Vector3 pos1 = Vector3.right * 1920;
            Vector3 pos2 = Vector3.zero;

            section1.localPosition = Vector3.Lerp(section1.transform.localPosition, pos1, spd * Time.deltaTime);
            section2.localPosition = Vector3.Lerp(section2.transform.localPosition, pos2, spd * Time.deltaTime);

            if (spd < 200)
                spd *= 1 + (Time.deltaTime * 1.5f);

            if(section2.localPosition.x > -1000)
            {
                //leaderboardText.Leading = Mathf.Lerp(leaderboardText.Leading, 0, 10 * Time.deltaTime);
            }

            if(sectionTimer<3)
            {
                returningToLobby.Size = Mathf.Lerp(returningToLobby.Size, 400, 10 * Time.deltaTime);
            }
        }

        public void Return()
        {
            //Bam.LobbyManagerScript.singleton.ReturnToLobby();

            if (myGameMode)
            {
                myGameMode.EndGame();
            }
            else
            {
                if (LobbyManagerScript.singleton)
                {
                    LobbyManagerScript.singleton.ReturnToLobby();
                }
            }

            Kojima.GameController.s_singleton.AllCarsCanMove(true);
            Destroy(gameObject);
        }
    }
}