//========================= Kojima Drive - Bird-Up 2017 =========================//
//
// Author: Sam Morris (SpAMCAN)
// Purpose: Transition object. Manages PostFX transitions.
// Namespace: Bird
//
//===============================================================================//

using UnityEngine;
using System.Collections;

namespace Bird {
	public abstract class BaseTransition : MonoBehaviour {
		public bool m_bLockInputWhileRunning = false;
		public bool m_bLockPlayerInputWhileRunning = false;
		bool m_bPreviousInputLockState;
		bool[] m_bPreviousPlayerInputLockState;

		protected bool m_bTransitionRunning = false;
		[SerializeField]
		public string m_strTransitionChannel = "GENERIC";
		public string TransitionChannel {
			get {
				return m_strTransitionChannel;
			}

			set {
				// If we're active, we need to update our active transition channel
				if(TransitionActive) {
					TransitionController.s_CurrentlyActiveTransitions[m_strTransitionChannel] = null;
					if (TransitionController.s_CurrentlyActiveTransitions.ContainsKey(value)) {
						if (TransitionController.s_CurrentlyActiveTransitions[value] != null) {
							TransitionController.s_CurrentlyActiveTransitions[value].InterruptTransition();
						}
						TransitionController.s_CurrentlyActiveTransitions[value] = this;
					} else {
						TransitionController.s_CurrentlyActiveTransitions.Add(value, this);
					}
				}

				m_strTransitionChannel = value;
			}
		}

		public bool TransitionActive {
			get {
				return m_bTransitionRunning;
			}
		}

		public bool m_bInterruptTransitionOnDestroy = false;

		// Event stuff
		public bool m_bFireKJDEvents = true;
		public bool m_bCustomKJDCompletionEvent = false;
		public Kojima.Events.Event m_eCustomCompletionEvent;

		// UI Events
		public System.Collections.Generic.List<UI_Transition_Listener> m_UIEventListeners;

		private void OnDestroy() {
			if (TransitionActive && m_bInterruptTransitionOnDestroy) {
				InterruptTransition();
			}

			// Unsubscribe
			Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.UI_TRANS_INTERRUPT, Event_InterruptTransition);
		}

		// Allow someone to interrupt a transition and snap to the final state here!
		public virtual void Event_InterruptTransition(object data) {
			string stridata = (string)data;
			if (stridata == m_strTransitionChannel) {
				InterruptTransition();
			}
		}

		public virtual void InterruptTransition() {
			StopTransition();
		}

		public virtual void StopTransition(bool bInterrupted = false) {
			m_bTransitionRunning = false;
			TransitionController.s_CurrentlyActiveTransitions[m_strTransitionChannel] = null;

			// Unsubscribe
			Kojima.EventManager.m_instance.UnsubscribeToEvent(Kojima.Events.Event.UI_TRANS_INTERRUPT, Event_InterruptTransition);

			if(m_bLockInputWhileRunning) {
				BaseMenuScreen.m_bInputLocked = m_bPreviousInputLockState;
			}

			if(m_bLockPlayerInputWhileRunning) {
				for(int i = 0; i < m_bPreviousPlayerInputLockState.Length; i++) {
					Kojima.GameController.s_singleton.m_players[i].CanMove = m_bPreviousPlayerInputLockState[i];
				}
			}

			if (m_UIEventListeners != null && !bInterrupted) {
				for (int i = 0; i < m_UIEventListeners.Count; i++) {
					if (bInterrupted) {
						m_UIEventListeners[i].TransitionInterrupted();
					} else {
						m_UIEventListeners[i].TransitionCompleted();
					}
				}
			}

			if (m_bFireKJDEvents) {
				if (m_bCustomKJDCompletionEvent) {
					Kojima.EventManager.m_instance.AddEvent(m_eCustomCompletionEvent);
				}

				Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_TRANS_ENDED);
			}
		}

		[Tooltip("Fire interrupt instead of stop when overriding another transition on the same channel")]
		public bool m_bInterruptTransitionDuringOverride = false;

		public virtual void StartTransitionReverse() {
			StartTransition(); // by default, no behaviour here
		}

		public virtual void StartTransition() {
			if (TransitionController.s_CurrentlyActiveTransitions == null) {
				TransitionController.s_CurrentlyActiveTransitions = new System.Collections.Generic.Dictionary<string, BaseTransition>();
			}

			bool bContainsKey = TransitionController.s_CurrentlyActiveTransitions.ContainsKey(m_strTransitionChannel);
			if (bContainsKey && TransitionController.s_CurrentlyActiveTransitions[m_strTransitionChannel] != null) {
				if (m_bInterruptTransitionDuringOverride) {
					TransitionController.s_CurrentlyActiveTransitions[m_strTransitionChannel].InterruptTransition();
				} else {
					TransitionController.s_CurrentlyActiveTransitions[m_strTransitionChannel].StopTransition();
				}
			}

			if (bContainsKey) {
				TransitionController.s_CurrentlyActiveTransitions[m_strTransitionChannel] = this;
			} else {
				TransitionController.s_CurrentlyActiveTransitions.Add(m_strTransitionChannel, this);
			}

			m_bTransitionRunning = true;

			// Fire the event
			Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_TRANS_STARTED);

			// Subscribe to interrupt
			Kojima.EventManager.m_instance.SubscribeToEvent(Kojima.Events.Event.UI_TRANS_INTERRUPT, Event_InterruptTransition);

			if (m_bLockInputWhileRunning) {
				m_bPreviousInputLockState = BaseMenuScreen.m_bInputLocked;
				BaseMenuScreen.m_bInputLocked = true;
			}

			if (m_bLockPlayerInputWhileRunning) {
				m_bPreviousPlayerInputLockState = new bool[Kojima.GameController.s_ncurrentPlayers];
				for (int i = 0; i < m_bPreviousPlayerInputLockState.Length; i++) {
					m_bPreviousPlayerInputLockState[i] = Kojima.GameController.s_singleton.m_players[i].CanMove;
					Kojima.GameController.s_singleton.m_players[i].CanMove = false;
				}
			}

			if (m_UIEventListeners != null) {
				for (int i = 0; i < m_UIEventListeners.Count; i++) {
					if (m_UIEventListeners[i] != null) {
						m_UIEventListeners[i].m_TriggeredTransition = this;
						m_UIEventListeners[i].TransitionStarted();
					}
				}
			}

			if(m_bFireKJDEvents) {
				Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_TRANS_STARTED);
			}
		}

		void Update() {
			if (m_bTransitionRunning) {
				UpdateTransition();

				if (m_UIEventListeners != null) {
					for (int i = 0; i < m_UIEventListeners.Count; i++) {
						m_UIEventListeners[i].TransitionUpdate(this);
					}
				}
			}
		}

		public abstract void UpdateTransition();

		public void SendCompletionMessage() {
			Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_TRANS_ENDED);
		}
	}
}