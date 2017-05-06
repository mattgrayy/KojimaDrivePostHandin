//===================== Kojima Drive - Half-Full Games 2017 ====================//
//
// Author:  Harry Davies
// Purpose: Acts as a middle man for handling event calls. Functions can be 
//          subscribed to event call list to be triggered when the event is 
//          called.
// Namespace: HF
//
//===============================================================================//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// I've added data-passing functionality to the event system, allowing us to hand generic objects to the delegate funcs.
// This was required for HUD stuff to figure out which player to target with the event. -sam 19/04/2017
// CHANGES:
// - Event buffers now use an eventQueue_t struct that contains the event ID and possibly a data object
// - Added EventTrigger_Data delegate

namespace Kojima
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager m_instance;
        public delegate void EventTrigger();
		public delegate void EventTrigger_Data(object data);

        [Serializable]
        public struct EventList
        {
            public EventTrigger m_event;
            public Events.Event m_eventType;

			public EventTrigger_Data m_dEventData;
		}

		[Serializable]
		public class eventQueue_t { // Used a class here so that we can pass by ref easier (structs always pass by value in C#)
			public Events.Event m_eEventType;
			public object m_Data;
		}

		//Holds a list of tracked events
		private static List<EventList> m_eventList = new List<EventList>();
        //Holds any events pushed to the EventManager this frame
        private static List<eventQueue_t> m_eventBuffer = new List<eventQueue_t>();
        //Acts as an intermediate when resolving event calls
        private static List<eventQueue_t> m_tempBuffer = new List<eventQueue_t>();

        void Awake()
        {
            if (m_instance)
            {
                Destroy(this.gameObject);
				return; // Don't continue this function, it's pointless.
            }
            else
            {
                m_instance = this;
            }

			//Adds a call list per Event in Events enum
			if (m_eventList.Count == 0) {
				for (int i = 0; i <= (int)Events.Event.Count - 1; i++) {
					EventList tempList = new EventList();
					tempList.m_eventType = (Events.Event)i;
					tempList.m_event = null;
					tempList.m_dEventData = null;

					m_eventList.Add(tempList);
				}
			}
        }

        void Update()
        {
            //Moves all events from tempBuffer to be resolved
            if (m_tempBuffer != null)
            {
                foreach (eventQueue_t events in m_tempBuffer)
                {
                    m_eventBuffer.Add(events);
                }

                m_tempBuffer.Clear();
            }
        }

        void LateUpdate()
        {
            BufferHandler();
        }

        /// <summary>Triggers all events pushed to EventBuffer this frame </summary>
        void BufferHandler()
        {
            //Triggers events pushed to EventBuffer
            foreach (eventQueue_t iterEvent in m_eventBuffer)
            {
				TriggerEvent(iterEvent);
            }

            //Clears the buffer of triggered evennts
            if (m_eventBuffer != null)
            {
                m_eventBuffer.Clear();
            }
        }

        /// <summary>Calls all functions subscribe to this event </summary>
        void TriggerEvent(int _index)
        {
            if (m_eventList[_index].m_event != null)
            {
                m_eventList[_index].m_event();
            }
        }

		/// <summary>Calls all functions subscribe to this event, using the EventQueue struct </summary>
		void TriggerEvent(eventQueue_t queue) {
			if (m_eventList[(int)queue.m_eEventType].m_event != null) {
				m_eventList[(int)queue.m_eEventType].m_event();
			}

			if (m_eventList[(int)queue.m_eEventType].m_dEventData != null && queue.m_Data != null) {
				m_eventList[(int)queue.m_eEventType].m_dEventData(queue.m_Data);
			}
		}

		/// <summary>Adds event call to EventBuffer for processing in LateUpdate </summary>
		public void AddEvent(Events.Event _event, object data = null)
        {
			eventQueue_t queue = new eventQueue_t();
			queue.m_eEventType = _event;
			queue.m_Data = data;
            m_tempBuffer.Add(queue);
        }

		/// <summary>Adds function to call list when this event is triggered </summary>
		public void SubscribeToEvent(Events.Event _event, EventTrigger _trigger)
        {
            EventList tempList = m_eventList[(int)_event];
            tempList.m_event += _trigger;
            m_eventList[(int)_event] = tempList;
        }

        /// <summary>Removes function from call list of this event </summary>
        public void UnsubscribeToEvent(Events.Event _event, EventTrigger _trigger)
        {
            EventList tempList = m_eventList[(int)_event];
            tempList.m_event -= _trigger;
            m_eventList[(int)_event] = tempList;
        }

		/// <summary>Adds function that requires a data object to call list when this event is triggered</summary>
		public void SubscribeToEvent(Events.Event _event, EventTrigger_Data _trigger) {
			EventList tempList = m_eventList[(int)_event];
			tempList.m_dEventData += _trigger;
			m_eventList[(int)_event] = tempList; // If we used classes instead of structs here, we wouldn't need to make two copies
		}

		/// <summary>Removes function that requires a data object from call list of this event </summary>
		public void UnsubscribeToEvent(Events.Event _event, EventTrigger_Data _trigger) {
			EventList tempList = m_eventList[(int)_event];
			tempList.m_dEventData -= _trigger;
			m_eventList[(int)_event] = tempList;
		}
	}
}
