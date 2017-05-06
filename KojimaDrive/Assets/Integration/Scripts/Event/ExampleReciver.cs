using UnityEngine;
using System.Collections;

namespace Kojima
{
    public class ExampleReciver : MonoBehaviour
    {
        void Start()
        {
            EventManager.m_instance.SubscribeToEvent(Events.Event.UI_UICHANGED, Ev_OnUIChanged);
        }

        void OnDestroy()
        {
            EventManager.m_instance.UnsubscribeToEvent(Events.Event.UI_UICHANGED, Ev_OnUIChanged);
        }

        void Ev_OnUIChanged()
        {
            Debug.Log("UIChanged");
        }
    }
}
