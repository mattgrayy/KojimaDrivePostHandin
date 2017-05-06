using UnityEngine;
using System.Collections;

namespace Kojima
{
    public class ExampleCaller : MonoBehaviour
    {
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.A))
            {
                EventManager.m_instance.AddEvent(Events.Event.UI_UICHANGED);
            }
        }
    }
}
