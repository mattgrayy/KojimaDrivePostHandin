//===================== Kojima Drive - Half-Full Games 2017 ====================//
//
// Author:  Harry Davies
// Purpose: Hides and ungidesobject when specified Events are called.
// Namespace: HF
//
//===============================================================================//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kojima
{
    public class HideOnEvent : MonoBehaviour
    {
        public List<Events.Event> m_revealEvent;
        public List<Events.Event> m_hideEvent;

        void Start()
        {
            foreach(Events.Event events in m_revealEvent)
            {
                EventManager.m_instance.SubscribeToEvent(events, EvFunc_RevealEvent);
            }

            foreach (Events.Event events in m_hideEvent)
            {
                EventManager.m_instance.SubscribeToEvent(events, EvFunc_HideEvent);
                if (events == Events.Event.GM_FREEROAM)
                {
                    gameObject.SetActive(false);
                }
            }            
        }

        private void OnDestroy()
        {
            foreach (Events.Event events in m_revealEvent)
            {
                EventManager.m_instance.UnsubscribeToEvent(events, EvFunc_RevealEvent);
            }

            foreach (Events.Event events in m_hideEvent)
            {
                EventManager.m_instance.UnsubscribeToEvent(events, EvFunc_HideEvent);
            }
        }

        void EvFunc_RevealEvent()
        {
            gameObject.SetActive(true);
        }

        void EvFunc_HideEvent()
        {
            gameObject.SetActive(false);
        }
    }
}
