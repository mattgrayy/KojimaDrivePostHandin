using UnityEngine;
using System.Collections;
using Kojima;

namespace GCSharp
{
    public class BombScript : MonoBehaviour
    {
        //bomb System
        private Bomb_System m_BombSystem;

        private GameObject m_bombSystemObject;

        public GameObject m_smokeParticlePref, m_currentSmokePart;
        public float m_smokeTime;
        private float m_smokeTimer;

        private ExplosionPhysics m_ExpPhy;
        private ParticleSpawner m_ParticleSpawner;
        private ParticleKiller m_ParticleKiller;
        private GameObject m_ParticlespawnerObject;

        private GameObject m_player;

        private CarScript m_carScript;

        // Use this for initialization
        private bool m_initCol, m_atDeliverPoint = false;

        public float m_val;

        [SerializeField]
        private float m_lowerVal, m_forwardVal;

        private float m_Timer;
        public float m_TimeLimit;

        public Vector3 tempPos, m_initalPos;

        private void Start()
        {
            m_player = null;
            m_initalPos = gameObject.transform.position;
            gameObject.transform.rotation = Quaternion.Euler(-90f, -90f, 0f);
            m_bombSystemObject = GameObject.FindGameObjectWithTag("BombSystem");
            m_BombSystem = m_bombSystemObject.GetComponent<Bomb_System>();
            /* m_ExpPhysicObject = GameObject.FindGameObjectWithTag("ExplosionsPhysics");
             m_ExpPhysic = m_ExpPhysicObject.GetComponent<ExplosionPhysics>();*/

            Vector3 t_pos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.75f, gameObject.transform.position.z);
            m_currentSmokePart = (GameObject)Instantiate(m_smokeParticlePref, t_pos, Quaternion.identity);
            m_currentSmokePart.transform.parent = gameObject.transform;
            m_currentSmokePart.GetComponent<ParticleSystem>().startSize = 0.1f;
            m_currentSmokePart.GetComponent<ParticleSystem>().startColor = Color.yellow;
            ParticleSystem.ShapeModule shapeMod = m_currentSmokePart.GetComponent<ParticleSystem>().shape;
            shapeMod.radius = 1f;

            m_ParticleSpawner = GetComponent<ParticleSpawner>();
            m_ExpPhy = GetComponent<ExplosionPhysics>();

            m_Timer = 0;

            m_smokeTimer = 0;

            m_initCol = false;
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                m_BombSystem.SetGameOver();
                Destroy(gameObject);
            }
            if (m_player != null)
            {
                if (m_player.GetComponent<Kojima.CarScript>().CurrentlyInWater)
                {
                    if (m_player.GetComponent<BombPass>())
                    {
                        m_player.GetComponent<BombPass>().SetHoldingBomb(false, null);
                    }
                    m_player.GetComponent<MaterialChanger>().UpdateMatToInitMat();
                    gameObject.transform.parent = null;
                    m_player = null;
                    m_initCol = false;
                    ResetBombPos();
                }
            }
            if (m_atDeliverPoint)
            {
                m_Timer += Time.deltaTime;
                if (m_Timer > m_TimeLimit)
                {
                    print("Fire in the hole!");
                    //triggers the explosion physics
                    m_ExpPhy.trigger();

                    //particles for explosion
                    m_ParticleSpawner.SpawnParticle();
                    //spawns in ner bomb
                    m_BombSystem.SetGameOver();
                    //destroys the bomb
                    Destroy(gameObject);
                    //m_atDeliverPoint = false;
                }
            }
        }

        public bool IsHeld()
        {
            if (m_player)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((other.tag == "Player") && (m_initCol == false))
            {
                m_initCol = true;
                tempPos = other.gameObject.transform.position;

                gameObject.transform.position = other.gameObject.GetComponent<BombPass>().GetBombPoint().transform.position;
                gameObject.transform.parent = other.gameObject.transform;
                m_player = other.gameObject;
                if (m_player.GetComponent<MaterialChanger>())
                {
                    m_player.GetComponent<MaterialChanger>().UpdateMatToBHMat();
                }
                if (m_player.GetComponent<BombPass>())
                {
                    m_player.GetComponent<BombPass>().SetHoldingBomb(true, gameObject);
                }
            }

            if (other.tag == "Deliver")
            {
                print("Hit deliver point");

                if (!m_atDeliverPoint)
                {
                    //gameObject.GetComponent<SphereCollider>().isTrigger = false;
                    print("taking bomb from player");
                    m_player.GetComponent<Score>().AddScore(300, m_player.GetComponent<Kojima.CarScript>().m_nplayerIndex - 1);
                    if (m_player.GetComponent<BombPass>())
                    {
                        m_player.GetComponent<BombPass>().SetHoldingBomb(false, null);
                    }
                    m_player.GetComponent<MaterialChanger>().UpdateMatToInitMat();
                    gameObject.transform.parent = null;
                    gameObject.transform.position = other.gameObject.transform.position;
                    gameObject.transform.parent = other.gameObject.transform;

                    m_atDeliverPoint = true;
                }
            }
        }

        public void SetNewBombHolder(GameObject _newHolder, GameObject _newPosGO)
        {
            gameObject.transform.position = _newPosGO.transform.position;
            gameObject.transform.parent = _newPosGO.transform;

            //tempPos = _newHolder.transform.position;
            //gameObject.transform.position = new Vector3(tempPos.x, tempPos.y + m_val, tempPos.z);
            //gameObject.transform.parent = _newHolder.transform;
            m_player = _newHolder;
            if (m_player.GetComponent<MaterialChanger>())
            {
                m_player.GetComponent<MaterialChanger>().UpdateMatToBHMat();
            }
            if (m_player.GetComponent<BombPass>())
            {
                m_player.GetComponent<BombPass>().SetHoldingBomb(true, gameObject);
            }
        }

        private void ResetBombPos()
        {
            gameObject.transform.position = m_initalPos;
        }
    }
}