using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Bam
{
    public class SabotagePlayerIDCanvas : MonoBehaviour
    {
        
        public Text m_playerID;

        int id;
        Kojima.CarScript m_myCar;
        Bam.PlayerCameraScript m_myCam;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.position = Kojima.GameController.s_singleton.m_players[id - 1].transform.position;
                //Kojima.GameController.s_singleton.m_players[id-1].GetSocket(CarSockets.Sockets.Top).position;
            FaceCamera();
        }

        public void SetPlayerID(Kojima.CarScript car, Bam.PlayerCameraScript cam)
        {
            m_myCar = car;
            m_myCam = cam;
            m_playerID.text = "P" + car.m_nplayerIndex;
            id = car.m_nplayerIndex;
        }

        void FaceCamera()
        {
            m_playerID.transform.rotation = Quaternion.LookRotation(m_myCam.transform.forward, m_myCam.transform.up);
        }
    }
}
