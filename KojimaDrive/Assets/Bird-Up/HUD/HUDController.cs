//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: HUD Controller Script
// Namespace: Bird
// NOTES ON INHERITANCE HERE:
// HUDController is a child oh HUDElement so that HUDControllers can be owned by other HUDControllers.
//
//===============================================================================//

using UnityEngine;
using System;
using System.Collections.Generic;

namespace Bird {
	public class HUDController : HUDElement {
		[Range(1,4)]
		public int m_nPlayer = 1;

		public LayerMask m_nLayer;

		public class hudElementToggleData_t {
			public hudElementToggleData_t() {
				
			}
			public hudElementToggleData_t(hudElementToggleData_t old) {
				m_nPlayerID = old.m_nPlayerID;
				m_Type = old.m_Type;
				m_nState = old.m_nState;
			}
			public enum elementState_e {
				DISABLE = 0,
				ENABLE = 1,
				TOGGLE = 2
			}
			public int m_nPlayerID;				// 0 = all players
			public Type m_Type;					// Target UI element type
			public elementState_e m_nState;		// 0 = disable, 1 = enable, 2 = toggle
		}

		public class hudElementToggleMultiData_t {
			public hudElementToggleMultiData_t() {

			}
			public hudElementToggleMultiData_t(hudElementToggleMultiData_t old) {
				m_nPlayerID = old.m_nPlayerID;
				m_ArrayTypes = old.m_ArrayTypes;
				m_nState = old.m_nState;
			}
			public int m_nPlayerID;             // 0 = all players
			public Type[] m_ArrayTypes;         // Target UI element types
			public hudElementToggleData_t.elementState_e m_nState;     // 0 = disable, 1 = enable, 2 = toggle
		}

		protected virtual void Start() {
			// Set up parent controller references
			for(int i = 0; i < m_HUDElements.Count; i++) {
				if(m_HUDElements[i] == null) {
					continue;
				}

				m_HUDElements[i].m_ParentController = this;
			}

			Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, Event_HUDElementToggle);
			Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.UI_HUD_HIDE_ALL_ELEMENTS, Event_HUDHideAll);
			Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.UI_HUD_HIDE_ALL_ELEMENTS, Event_HUDHideAll_TargetPlayer);
		}

		protected override void OnDestroy() {
			Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, Event_HUDElementToggle);
			Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.UI_HUD_HIDE_ALL_ELEMENTS, Event_HUDHideAll);
			Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.UI_HUD_HIDE_ALL_ELEMENTS, Event_HUDHideAll_TargetPlayer);
			base.OnDestroy();
		}

		// DOES NOT HIDE HUDController OBJECTS
		void Event_HUDHideAll_TargetPlayer(object data) {
			int nPlayer = (int)data;
			if(m_nPlayer != nPlayer && m_nPlayer != 0) {
				return;
			}

			for (int i = 0; i < m_HUDElements.Count; i++) {
				if (!typeof(HUDController).IsAssignableFrom(m_HUDElements[i].GetType())) {
					m_HUDElements[i].m_bDisplay = false;
				}
			}
		}

		void Event_HUDHideAll() {
			for(int i = 0; i < m_HUDElements.Count; i++) {
				if(!typeof(HUDController).IsAssignableFrom(m_HUDElements[i].GetType())) {
					m_HUDElements[i].m_bDisplay = false;
				}
			}
		}

		void Event_HUDElementToggleActual(hudElementToggleData_t dataobj) {
			if (dataobj.m_nPlayerID != m_nPlayer && dataobj.m_nPlayerID != 0) {
				return;
			}

			// Toggle ourselves in this instance (special case, easier to add this rather than add ourselves to a self-repeating update loop!)
			if (m_ParentController == null && dataobj.m_Type == GetType()) {
				switch (dataobj.m_nState) {
					case hudElementToggleData_t.elementState_e.DISABLE:
						m_bDisplay = false;
						break;
					case hudElementToggleData_t.elementState_e.ENABLE:
						m_bDisplay = true;
						break;
					case hudElementToggleData_t.elementState_e.TOGGLE:
					default:
						m_bDisplay = !m_bDisplay;
						break;
				}

				return;
			}

			ElementToggle(dataobj.m_Type, dataobj.m_nState);
		}

		void Event_HUDElementToggleMulti(hudElementToggleMultiData_t dataobj) {
			if (dataobj.m_nPlayerID != m_nPlayer && dataobj.m_nPlayerID != 0) {
				return;
			}

			for (int i = 0; i < dataobj.m_ArrayTypes.Length; i++) {
				// Toggle ourselves in this instance (special case, easier to add this rather than add ourselves to a self-repeating update loop!)
				if (m_ParentController == null && dataobj.m_ArrayTypes[i] == GetType()) {
					switch (dataobj.m_nState) {
						case hudElementToggleData_t.elementState_e.DISABLE:
							m_bDisplay = false;
							break;
						case hudElementToggleData_t.elementState_e.ENABLE:
							m_bDisplay = true;
							break;
						case hudElementToggleData_t.elementState_e.TOGGLE:
						default:
							m_bDisplay = !m_bDisplay;
							break;
					}

					continue;
				}

				ElementToggle(dataobj.m_ArrayTypes[i], dataobj.m_nState);
			}
		}

		void ElementToggle(Type type, hudElementToggleData_t.elementState_e elementState) {
			for (int i = 0; i < m_HUDElements.Count; i++) {
				if (m_HUDElements[i].GetType() == type) {
					switch (elementState) {
						case hudElementToggleData_t.elementState_e.DISABLE:
							m_HUDElements[i].m_bDisplay = false;
							break;
						case hudElementToggleData_t.elementState_e.ENABLE:
							m_HUDElements[i].m_bDisplay = true;
							break;
						case hudElementToggleData_t.elementState_e.TOGGLE:
						default:
							m_HUDElements[i].m_bDisplay = !m_HUDElements[i].m_bDisplay;
							break;
					}
				}
			}
		}

		void Event_HUDElementToggle(object data) {
			if(data.GetType() == typeof(hudElementToggleData_t)) {
				// Single element
				hudElementToggleData_t dataobj = (hudElementToggleData_t)data;
				Event_HUDElementToggleActual(dataobj);
			}

			if (data.GetType() == typeof(hudElementToggleMultiData_t)) {
				// Single element
				hudElementToggleMultiData_t dataobj = (hudElementToggleMultiData_t)data;
				Event_HUDElementToggleMulti(dataobj);
			}
		}

		public void AddHUDElement(HUDElement hudelement) {
			hudelement.m_ParentController = this;
			m_HUDElements.Add(hudelement);
			hudelement.gameObject.layer = m_nLayer;
		}

		public void RemoveHUDElement(HUDElement hudelement) {
			hudelement.m_ParentController = null;
			m_HUDElements.Remove(hudelement);
		}

/*		public bool m_bSendEvent = false;
		private void Update() {
			if(m_bSendEvent) {
				Bird.HUDController.hudElementToggleData_t data = new Bird.HUDController.hudElementToggleData_t();
				data.m_nPlayerID = 1;
				data.m_nState = hudElementToggleData_t.elementState_e.TOGGLE;
				data.m_Type = typeof(HUD_RacePosition);
				Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, data);
				m_bSendEvent = false;
			}
		}	*/

#if TEST_HUDCONTROLLER
		public bool m_bTest_Area = false;
		public string m_strAreaName = "TEST AREA NAME";

		public bool m_bTest_GameStart = false;
		public string m_strEventName = "EVENT TEST";
		public string m_strEventByline = "EVENT BYLINE TEST";

		public bool m_bTest_EXP = false;
		public int m_nEXPToAdd = 10;
		private void Update() {
			if(m_bTest_Area) {
				HUD_Area.hudAreaData_t newdata = new HUD_Area.hudAreaData_t();
				newdata.m_nTargetPlayerID = m_nPlayer;
				newdata.m_strAreaName = m_strAreaName;
				Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_SHOW_AREANAME, newdata);
				m_bTest_Area = false;
			}

			if(m_bTest_EXP) {
				HF.PlayerExp.AddEXP(m_nPlayer - 1, m_nEXPToAdd, true, true, "DO A POINT!");
				m_bTest_EXP = false;
			}

			if (m_bTest_GameStart) {
				HUD_EventStart.hudEventStartData_t newdata = new HUD_EventStart.hudEventStartData_t();
				newdata.m_strEventName = m_strEventName;
				newdata.m_strEventDesc = m_strEventByline;
				Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_SHOW_EVENTSTART, newdata);
				m_bTest_GameStart = false;
			}
		}
#endif

		[SerializeField]
		public List<HUDElement> m_HUDElements;
		protected virtual void LateUpdate() {
			if (m_ParentController == null) {
				UpdateHUDElement();
			}
		}

		public override void UpdateHUDElement() {
			for (int i = 0; i < m_HUDElements.Count; i++) {
				if(m_HUDElements[i] == null) {
					continue;
				}

				if (m_HUDElements[i].m_bDisplay) {
					m_HUDElements[i].UpdateHUDElement();
				}

				m_HUDElements[i].gameObject.SetActive(m_bDisplay && m_HUDElements[i].m_bDisplay);
			}
		}

		public T GetHUDElement<T>() where T : HUDElement {
			for(int i = 0; i < m_HUDElements.Count; i++) {
				if(m_HUDElements[i].GetType() == typeof(T)) {
					return (T)m_HUDElements[i];
				}
			}

			return null;
		}

		public T[] GetHUDElements<T>() where T: HUDElement {
			int nCount = 0;
			for (int i = 0; i < m_HUDElements.Count; i++) {
				if (m_HUDElements[i].GetType() == typeof(T)) {
					nCount++;
				}
			}

			if(nCount == 0) {
				return null;
			}

			T[] array = new T[nCount];
			int j = 0;
			for (int i = 0; i < m_HUDElements.Count; i++) {
				if (m_HUDElements[i].GetType() == typeof(T)) {
					array[j] = (T)m_HUDElements[i];
					j++;
				}
			}


			return array;
		}
	}
}
