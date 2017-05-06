using UnityEngine;
using System.Collections;

namespace Bird {
	public class DefaultTransitions : MonoBehaviour {

		public GameObject m_DefaultIn;
		public GameObject m_DefaultOut;

		public static BaseTransition s_DefaultIn;
		public static BaseTransition s_DefaultOut;

		[HideInInspector]
		public static bool m_bSetup = false;

		static public DefaultTransitions m_Instance;

		private void Awake() {
			if (!m_bSetup) {
				Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.UI_TRANS_DEFAULT_IN, Event_DefaultTransitionIn);
				Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.UI_TRANS_DEFAULT_OUT, Event_DefaultTransitionOut);

				if (s_DefaultIn == null) {
					s_DefaultIn = Instantiate(m_DefaultIn).GetComponent<BaseTransition>();
					ObjectDB.DontDestroyOnLoad_Managed(s_DefaultIn);
				}

				if (s_DefaultOut == null) {
					s_DefaultOut = Instantiate(m_DefaultOut).GetComponent<BaseTransition>();
					ObjectDB.DontDestroyOnLoad_Managed(s_DefaultOut);
				}
				m_bSetup = true;
				m_Instance = this;
			}
		}

		private void OnDestroy() {
			/*if (m_bSetup) {
				Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.UI_TRANS_DEFAULT_IN, Event_DefaultTransitionIn);
				Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.UI_TRANS_DEFAULT_OUT, Event_DefaultTransitionOut);
				m_bSetup = false;
			}*/
		}

		public static void Event_DefaultTransitionIn() {
			if (s_DefaultIn != null) {
				s_DefaultIn.StartTransition();
			}
		}

		public static void Event_DefaultTransitionOut() {
			if (s_DefaultOut != null) {
				s_DefaultOut.StartTransition();
			}
		}
	}
}