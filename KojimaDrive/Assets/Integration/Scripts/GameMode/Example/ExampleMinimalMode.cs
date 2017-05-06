using UnityEngine;
using System.Collections;

namespace Kojima
{
    public class ExampleMinimalMode : GameMode
    {
        new
        void Start()
        {
            base.Start();
        }

        new
       void Update()
        {
            base.Update();

            //Game Mode Loop
            if (m_active)
            {
                Debug.Log("Example Active");

                //Game Modes are required to have an exit point
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    EndGame();
                }
            }
        }
    }
}
