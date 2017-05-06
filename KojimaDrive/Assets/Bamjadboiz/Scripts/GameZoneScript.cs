using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Bam
{
    public class GameZoneScript : MonoBehaviour
    {
        public ParticleSystem m_activeParticle;
        public LobbyLocationScript m_locationCanvas;
        public string m_strSceneName, m_lobbySceneName; //name of the scene to be loaded
        public bool m_isGameMode;

        int m_nTotalCars; //total cars in this zone
        float m_fRotSpeed = 5.0f;
        float m_fPlayerMultiplier; //game "loads" faster when this is larger
        bool m_isActive = false;
        int m_layerCounter;
        LobbyLocationScript[] m_myHUDs;
        float m_fLoadTimer;
        
        

        // Use this for initialization
        void Start()
        {
            //m_activeParticle.gravityModifier = -0.006f;
            m_layerCounter = 8;
            m_myHUDs = new LobbyLocationScript[Kojima.GameController.s_ncurrentPlayers];
            SpawnLocationHUD();
        }

        // Update is called once per frame
        void Update()
        {
            if(m_nTotalCars > 0)
            {
                if(Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.DownArrow))
                {
                    m_fLoadTimer = 1;
                }

                //m_activeParticle.gravityModifier = -0.1f;
                transform.Rotate(0, Time.deltaTime * m_fRotSpeed, 0);
                m_isActive = true;

                if (m_fLoadTimer != 1)
                {
                    HandleGameStartLogic();
                }
            }
            else
            {
                //m_activeParticle.gravityModifier = -0.006f;
                m_isActive = false;
            }

            LerpTimer(m_fPlayerMultiplier, m_isActive);
        }

        void OnEnable()
        {
            m_nTotalCars = 0;
            m_fLoadTimer = 0.0f;
        }

        void HandleGameStartLogic()
        {
            //handles what to do with x amount of cars in a zone(s)
            switch(m_nTotalCars)
            {
                case 0:
                    m_fPlayerMultiplier = 0.0f;
                    break;
                case 1:
                    m_fPlayerMultiplier = 0.03f;
                    break;
                case 2:
                    m_fPlayerMultiplier = 0.05f;
                    break;
                case 3:
                    m_fPlayerMultiplier = 0.08f;
                    break;
                case 4:
                    m_fPlayerMultiplier = 0.1f;
                    break;
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            if(collider.gameObject.GetComponent<Kojima.CarScript>() && !collider.isTrigger)
            {
                m_nTotalCars++;

                if(m_nTotalCars > 0)
                {
                    for (int i = 0; i < m_myHUDs.Length; i++)
                    {
                        if (collider.gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex - 1 == i)
                        {
                            m_myHUDs[i].SetActive(true);
                        }
                    }
                    Kojima.CarScript myCar = collider.gameObject.GetComponent<Kojima.CarScript>();

                    if (myCar.GetCam)
                    {
                        myCar.GetCam.SwitchViewStyle(PlayerCameraScript.viewStyles_e.inGameZone);
                    }
                }
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if(collider.gameObject.GetComponent<Kojima.CarScript>() && !collider.isTrigger)
            {
                m_nTotalCars--;

                    for (int i = 0; i < m_myHUDs.Length; i++)
                    {
                        if (collider.gameObject.GetComponent<Kojima.CarScript>().m_nplayerIndex - 1 == i)
                        {
                            m_myHUDs[i].SetActive(false);
                        }
                    }
                    Kojima.CarScript myCar = collider.gameObject.GetComponent<Kojima.CarScript>();
                    myCar.GetCam.SwitchViewStyle(PlayerCameraScript.viewStyles_e.thirdPerson);            
            }
        }

        void SpawnLocationHUD()
        {
            for(int i = 0; i < Kojima.GameController.s_ncurrentPlayers; i++)
            {
                if (i <= 3)
                {
                    GameObject newHUD = Instantiate<GameObject>(m_locationCanvas.gameObject);
                    newHUD.transform.SetParent(gameObject.transform.parent);
                    newHUD.transform.localPosition = m_locationCanvas.transform.localPosition;
                    newHUD.GetComponent<LobbyLocationScript>().SetCamRefNum(i);
                    newHUD.layer = m_layerCounter;
                    m_layerCounter++;
                    m_myHUDs[i] = newHUD.GetComponent<LobbyLocationScript>();
                }
            }

            m_locationCanvas.gameObject.SetActive(false);
        }

        public void LerpTimer(float timeModifier, bool active)
        {
            if (active)
            {
                if (m_fLoadTimer < 1)
                {
                    m_fLoadTimer += Time.deltaTime * timeModifier;
                }
            }
            else
            {
                if (m_fLoadTimer > 0)
                {
                    m_fLoadTimer -= Time.deltaTime;
                }
            }

            if (m_fLoadTimer >= 1)
            {
                if (m_isGameMode)
                {
                    LobbyManagerScript.singleton.SetCurGame(m_strSceneName);
                    LobbyManagerScript.singleton.LoadGame(m_strSceneName);
                }
                else
                {
                    LobbyManagerScript.singleton.ChangeIsland(m_strSceneName, m_lobbySceneName);
                }
            }

            for(int i = 0; i < m_myHUDs.Length; i++)
            {
                m_myHUDs[i].UpdateTimer(m_fLoadTimer);
            }
        }
    }
}
