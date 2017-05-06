using UnityEngine;
using System.Collections;
using Bam;

namespace GCSharp
{
    public class BombPass : MonoBehaviour
    {
        [SerializeField]
        private bool m_holdingBomb = false, m_timerStart;

        [SerializeField]
        private MaterialChanger m_matChangerScript;

        [SerializeField]
        private GameObject m_bomb;

        [SerializeField]
        private GameObject m_bombGO;

        private float m_timer;
        private float m_scoreTimer = 0;
        public float m_scoreInterval = 2f;

        [SerializeField]
        private float m_timeInterval;

        private GameObject m_playerHit;
        private GameObject m_BombPoint;
        private GameObject m_playerHitBombPoint;
        private Score m_scoreScript;

        [SerializeField]
        private GameObject m_arrow;

        private GameObject m_deliverPoint;
        public Bird.ArrowCheckpoints m_arrowScript;

        // Use this for initialization
        private void Start()
        {
            m_scoreScript = gameObject.GetComponent<Score>();
            m_holdingBomb = false;
            m_timerStart = false;
            m_matChangerScript = GetComponent<MaterialChanger>();
            m_arrow = gameObject.transform.FindChild("LocalArrowRotation(Clone)").gameObject;
            Destroy(m_arrow);
        }

        // Update is called once per frame
        private void Update()
        {
            if (m_arrow == null)
            {
                m_arrow = gameObject.transform.FindChild("LocalArrowRotation(Clone)").gameObject;
                m_arrowScript = m_arrow.GetComponent<Bird.ArrowCheckpoints>();
                m_arrowScript.activeCheckpoint = 1;
            }
            if (m_bombGO == null)
            {
                m_bombGO = GameObject.Find("LowPolyBomb(Clone)");
            }
            if (m_deliverPoint == null)
            {
                m_deliverPoint = GameObject.Find("LowPolyBombMount (1)");
            }

            if (m_timerStart)
            {
                Timer();
            }
            if (m_holdingBomb)
            {
                m_scoreTimer += Time.deltaTime;
                if (m_scoreTimer >= m_scoreInterval)
                {
                    m_scoreScript.AddScore(10, GetComponent<Kojima.CarScript>().m_nplayerIndex - 1);
                    m_scoreTimer -= m_scoreInterval;
                }

                if (m_deliverPoint != null && m_arrow != null)
                {
                    m_arrowScript.activeCheckpoint = 0;
                }
            }
            else if (!m_holdingBomb && m_bombGO != null && m_arrow != null)
            {
                m_arrowScript.activeCheckpoint = 1;
            }
        }

        private void OnCollisionExit(Collision col)
        {
            if (m_holdingBomb)
            {
                if (col.collider.tag == "Player")
                {
                    m_playerHit = col.gameObject;
                    m_playerHitBombPoint = col.gameObject.GetComponent<BombPass>().GetBombPoint();
                    m_timerStart = true;
                }
            }
        }

        private void Timer()
        {
            if (m_timer > m_timeInterval)
            {
                //need to add a delay to the pass
                if (m_bomb.GetComponent<BombScript>())
                {
                    m_bomb.GetComponent<BombScript>().SetNewBombHolder(m_playerHit, m_playerHitBombPoint);
                }
                m_matChangerScript.UpdateMatToInitMat();
                m_playerHit = null;
                m_playerHitBombPoint = null;
                m_bomb = null;
                m_holdingBomb = false;
                m_timer = 0;
                m_timerStart = false;
            }

            m_timer += Time.deltaTime;
        }

        public bool GetHoldingBomb()
        {
            return m_holdingBomb;
        }

        public void SetHoldingBomb(bool _newValue, GameObject _bomb)
        {
            m_holdingBomb = _newValue;
            m_bomb = _bomb;
        }

        public void SetBombPoint(GameObject _bombPoint)
        {
            m_BombPoint = _bombPoint;
        }

        public GameObject GetBombPoint()
        {
            return m_BombPoint;
        }
    }
}