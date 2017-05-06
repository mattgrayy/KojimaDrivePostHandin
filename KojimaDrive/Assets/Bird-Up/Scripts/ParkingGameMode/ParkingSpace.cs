//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//namespace Kojima
//{
//    public class ParkingSpace : MonoBehaviour
//    {

//        [SerializeField]
//        protected List<CarScript> m_CarList;                //cars currently withing the space
//        [SerializeField]
//        protected List<int> m_WheelsInside;             //how many wheels each car has inside the zone
//                                                        //[SerializeField]
//                                                        //static protected float[] m_Scores = new float[4];					//the score of each car
//        [SerializeField]
//        protected CarScript m_CurrentParkedCar;             //which car is currently in the space?
//        [SerializeField]
//        protected float m_LevelTimer;                   //how long has the current level been going?
//        protected bool Won = false;                     //TEMPORARY FOR TESTING
//        protected SpriteRenderer m_Renderer;            //for changing the colour of the space
//        protected ParticleSystem m_Particles;           //for changing the colour of particles
//        protected ParticleSystem m_ParticlesTall;       //for rotating larger particle system
//        public float m_RoundTime;                       //how long do cars have to stay in the space
//        public float m_ScoreLimit;                      //what is the maximum number of points before a player wins?
//                                                        // Use this for initialization
//        void Start()
//        {
//            m_Renderer = GetComponentInChildren<SpriteRenderer>();
//            m_Particles = transform.FindChild("Low Particles").gameObject.GetComponent<ParticleSystem>();
//            m_ParticlesTall = transform.FindChild("Tall Particles").gameObject.GetComponent<ParticleSystem>();
//        }

//        // Update is called once per frame
//        void LateUpdate()
//        {
//            /*if (!Won)*/
//            {
//                if (m_CarList.Count > 0)
//                {
//                    UpdateScores();
//                    if (FindMostParked(m_CarList) != null)
//                    {
//                        Color newColour = FindMostParked(m_CarList).GroundMat.color;
//                        m_Renderer.color = newColour;
//                        m_Particles.startColor = newColour;
//                    }
//                    else
//                    {
//                        m_Renderer.color = Color.white;
//                        m_Particles.startColor = Color.white;
//                    }
//                    //m_LevelTimer += Time.deltaTime;
//                    if (GameManager.GM.RoundTimer <= 0.0f && !m_bBonusPointsGiven)
//                    {
//                        CarScript winner = FindMostParked(m_CarList);
//                        if (winner != null)
//                        {
//                            CarScript.s_ParkingBonusScore[winner.controlingPlayer - 1]++;
//                            winner.ScorePopup("PARKED BONUS!", CarScript.s_ParkingBonusScoreMult, true, false);
//                            Soundbank.PlayOneShot("SFX", "POINT_ADD_ALT");
//                        }
//                        m_bBonusPointsGiven = true;
//                    }
//                }
//                m_ParticlesTall.transform.Rotate(new Vector3(0.0f, 0.0f, 60.0f * Time.deltaTime));
//            }

//            //UpdateScores();
//        }

//        public float[] m_fScoresThisRound = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };
//        public float[] m_fScoresThisRoundTicker = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };
//        bool m_bBonusPointsGiven = true;

//        public void RoundReset()
//        {
//            for (int i = 0; i < 4; i++)
//            {
//                m_fScoresThisRound[i] = 0.0f;
//                m_fScoresThisRoundTicker[i] = 0;
//            }

//            m_bBonusPointsGiven = false;
//        }

//        //void UpdateScores()
//        //{
//        //    if (GameManager.GameState == GameManager.gameStates_e.GAMESTATE_GAMEPLAY)
//        //    {
//        //        for (int i = 0; i < m_CarList.Count; i++)
//        //        {
//        //            CarScript.s_ParkingSpaceScores[m_CarList[i].controlingPlayer - 1] += m_WheelsInside[i] * Time.deltaTime;
//        //            m_fScoresThisRound[m_CarList[i].controlingPlayer - 1] += m_WheelsInside[i] * Time.deltaTime;
//        //            m_fScoresThisRoundTicker[m_CarList[i].controlingPlayer - 1] += m_WheelsInside[i] * Time.deltaTime;

//        //            if ((m_fScoresThisRoundTicker[m_CarList[i].controlingPlayer - 1]) > 1)
//        //            {
//        //                m_CarList[i].ScorePopup("IN PARKING SPACE!", CarScript.s_ParkingSpaceScoreMult);
//        //                m_fScoresThisRoundTicker[m_CarList[i].controlingPlayer - 1] = 0;
//        //            }
//        //        }
//        //    }
//        //    /*List<motor> Contendors = new List<motor>();
//        //    if (m_CarList != null)
//        //    {
//        //        for (int i = 0; i < m_CarList.Count; i++)
//        //        {
//        //            if (m_fScoresThisRound[Contendors[i].controlingPlayer - 1] >= m_ScoreLimit)
//        //            {
//        //                Contendors.Add(m_CarList[i]);
//        //            }
//        //        }
//        //    }
//        //    if (Contendors.Count != 0)
//        //    {
//        //        if (Contendors.Count == 1)
//        //        {
//        //            Win(Contendors[0]);
//        //        }
//        //        else
//        //        {
//        //            Win(FindMostParked(Contendors));
//        //        }
//        //    }*/
//        //}

//        void Win(CarScript _Winner)
//        {
//            Won = true;
//        }

//        CarScript SetClosestCar(List<CarScript> _Contendors)
//        {
//            float closestDistance = 9999.0f;
//            int closestCar = 0;
//            for (int i = 0; i < _Contendors.Count; i++)
//            {
//                if (Vector3.Distance(m_CarList[i].transform.position, transform.position) < closestDistance)
//                {
//                    closestDistance = Vector3.Distance(m_CarList[i].transform.position, transform.position);
//                    closestCar = i;
//                }
//            }
//            return _Contendors[closestCar];
//        }

//        CarScript FindMostParked(List<CarScript> _Contendors)
//        {
//            int CurrentHighest = 0;
//            List<CarScript> Contendors = new List<CarScript>();
//            for (int i = 0; i < _Contendors.Count; i++)
//            {
//                int CarIndex = m_CarList.IndexOf(_Contendors[i]);
//                if (Contendors.Count == 0)
//                {
//                    Contendors.Add(_Contendors[i]);
//                    CurrentHighest = m_WheelsInside[CarIndex];
//                }
//                else
//                {

//                    if (m_WheelsInside[CarIndex] > CurrentHighest)
//                    {
//                        Contendors.Clear();
//                        CurrentHighest = m_WheelsInside[CarIndex];
//                    }
//                    else if (m_WheelsInside[CarIndex] == CurrentHighest)
//                    {
//                        Contendors.Add(m_CarList[i]);
//                    }
//                }
//            }
//            if (CurrentHighest == 0)
//            {
//                return null;
//            }
//            else if (Contendors.Count > 1)
//            {
//                return SetClosestCar(Contendors);
//            }
//            else
//            {
//                return Contendors[0];
//            }
//        }

//        CarScript FindMostParked()
//        {
//            int CurrentHighest = 0;
//            List<CarScript> Contendors = new List<CarScript>();
//            for (int i = 0; i < m_CarList.Count; i++)
//            {
//                if (Contendors.Count == 0)
//                {
//                    Contendors.Add(m_CarList[i]);
//                    CurrentHighest = m_WheelsInside[i];
//                }
//                else
//                {

//                    if (m_WheelsInside[i] > CurrentHighest)
//                    {
//                        Contendors.Clear();
//                        CurrentHighest = m_WheelsInside[i];
//                    }
//                    else if (m_WheelsInside[i] == CurrentHighest)
//                    {
//                        Contendors.Add(m_CarList[i]);
//                    }
//                }
//            }
//            if (Contendors.Count > 1)
//            {
//                return SetClosestCar(Contendors);
//            }
//            else
//            {
//                return Contendors[0];
//            }
//        }

//        void OnTriggerEnter(Collider col)
//        {
//            if (col.GetComponent<wheel>())
//            {
//                if (!m_CarList.Contains(col.GetComponentInParent<CarScript>()))
//                {
//                    m_CarList.Add(col.GetComponentInParent<CarScript>());
//                    m_WheelsInside.Add(1);
//                    //m_Scores.Add(0);
//                }
//                else
//                {
//                    m_WheelsInside[m_CarList.IndexOf(col.GetComponentInParent<CarScript>())]++;
//                }
//            }
//        }

//        void OnTriggerExit(Collider col)
//        {
//            if ((m_CarList.Contains(col.GetComponentInParent<CarScript>())) && (col.GetComponent<CarScript>()))
//            {
//                m_WheelsInside[m_CarList.IndexOf(col.GetComponentInParent<CarScript>())]--;
//            }
//        }
//    }
//}