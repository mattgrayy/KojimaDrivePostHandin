using UnityEngine;
using System.Collections;

namespace Bam
{
    public class MapCameraScript : MonoBehaviour
    {
        public static MapCameraScript singleton;

        public static bool mapAvailable = true;

        [SerializeField]
        bool mapOpen;

        Camera myCam;

        // Use this for initialization
        void Awake()
        {
            myCam = GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            if(mapAvailable)
            {
                for(int i=0; i<Kojima.GameController.s_ncurrentPlayers; i++)
                {
                    Kojima.CarScript curPlayer = Kojima.GameController.s_singleton.m_players[i];

                    if (curPlayer)
                    {
                        if(curPlayer.GetRewiredPlayer().GetButtonDown("ActivateDebugCam"))
                        {
                            if(mapOpen)
                            {
                                CloseMap();
                            }
                            else
                            {
                                OpenMap();
                            }

                            break;
                        }
                    }
                }
            }
            else
            {
                if(mapOpen)
                {
                    CloseMap();
                }
            }

            Movement();
        }

        void Movement()
        {
            Rect newRect = myCam.rect;
            float target = 1;

            if(mapOpen)
            {
                target = 0;
            }

            newRect.y = Mathf.Lerp(newRect.y, target, 15 * Time.deltaTime);

            myCam.rect = newRect;

            myCam.enabled = newRect.y < 0.95f;
        }

        public void ChangeMapAvailability(bool _enabled)
        {
            mapAvailable = _enabled;
        }

        public void OpenMap()
        {
            mapOpen = true;
        }

        public void CloseMap()
        {
            mapOpen = false;
        }
    }
}