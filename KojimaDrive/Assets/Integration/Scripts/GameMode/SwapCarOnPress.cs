using UnityEngine;
using System.Collections;
using Rewired;

namespace Kojima
{
    //===================== Kojima Drive - Half-Full 2017 ====================//
    //
    // Author: HARRY DAVIES, SAM BRITNELL
    // Purpose: Handles swapping cars based on a button input
    // Namespace: KOJIMA
    //
    //===============================================================================//


    public class SwapCarOnPress : MonoBehaviour
    {
        Rewired.Player m_rewiredPlayer;

        bool m_bLockSwap = false;

        void Start()
        {
            m_rewiredPlayer = gameObject.GetComponent<CarScript>().GetRewiredPlayer();

            /*if (GetComponent<CarScript>().m_nplayerIndex == 1)
            {
                m_sDPadLeft = KeyCode.Tab;
                m_sDPadRight = KeyCode.LeftShift;
            }
            else if(GetComponent<CarScript>().m_nplayerIndex == 2)
            {
                m_sDPadLeft = KeyCode.RightShift;
                m_sDPadRight = KeyCode.RightControl;
            }
            else if (GetComponent<CarScript>().m_nplayerIndex == 3)
            {
                m_sDPadLeft = KeyCode.Insert;
                m_sDPadRight = KeyCode.Home;
            }
            else if (GetComponent<CarScript>().m_nplayerIndex == 4)
            {
                m_sDPadLeft = KeyCode.KeypadMinus;
                m_sDPadRight = KeyCode.KeypadPlus;
            }*/

            //m_sDPadLeft = m_rewiredPlayer.GetButton("SwitchCarLeft");
            //m_sDPadRight = m_rewiredPlayer.GetButton("SwitchCarRight");
        }

        void Update()
        {
            if (!m_bLockSwap)
            {
                if (CarSwapManager.m_sInstance.GetSwapping())
                {
                    Swap();
                }

                if(gameObject.GetComponent<CarScript>().GetRewiredPlayer().GetButtonDown("Action"))
                {
                    m_bLockSwap = true;
                }
            }
        }

        void Swap()
        {
             //if (gameObject.GetComponent<CarScript>().GetRewiredPlayer().GetButtonDown("SwitchCarLeft") || Input.GetKeyDown(KeyCode.Tab))
             //{
             //    MoveForwardInList();

             //}

             //if (gameObject.GetComponent<CarScript>().GetRewiredPlayer().GetButtonDown("SwitchCarRight"))
             //{
             //    MoveBackInList();
             //}
         
        }

        void MoveForwardInList()
        {
            int index = GetComponent<CarData>().m_index + 1;
            if (Kojima.CarSwapManager.m_sInstance.m_carPrefab.Count <= index)
            {
                index = 1;
            }
            Kojima.CarSwapManager.m_sInstance.ChangeCar(GetComponent<CarScript>().m_nplayerIndex - 1, GetComponent<CarScript>().m_nControllerID, index);
        }

        void MoveBackInList()
        {
            int index = GetComponent<CarData>().m_index - 1;
            if (0 >= index)
            {
                index = Kojima.CarSwapManager.m_sInstance.m_carPrefab.Count;
            }
            Kojima.CarSwapManager.m_sInstance.ChangeCar(GetComponent<CarScript>().m_nplayerIndex - 1, GetComponent<CarScript>().m_nControllerID, index);
        }

        public bool CarLocked()
        {
            return m_bLockSwap;
        }

        public void SetCarLock(bool _state)
        {
            m_bLockSwap = _state;
        }
    }
}
