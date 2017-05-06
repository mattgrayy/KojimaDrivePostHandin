using UnityEngine;
using System.Collections;

public class HUDTest_FakeGamemode : MonoBehaviour {

	// Fake gamemode code to demonstrate setting up the hud for your game type
	void Start () {
		// Hide all HUDElements
		Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_HIDE_ALL_ELEMENTS);

		// Unhide the elements we want (this event accepts both hudElementToggleData_t and hudElementToggleMultiData_t - multi accepts an array of types)
		Bird.HUDController.hudElementToggleMultiData_t dataobject = new Bird.HUDController.hudElementToggleMultiData_t();
		dataobject.m_nPlayerID = 1; // Target player ID 0 = all players (otherwise, it's 1 - 4)
		dataobject.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.ENABLE;
		dataobject.m_ArrayTypes = new System.Type[] { typeof(Bird.HUD_EXP), typeof(Bird.HUD_Timer), typeof(Bird.HUD_ScorePopupMgr), typeof(Bird.HUD_RacePosition), typeof(Bird.HUD_NavArrow) };

		// This will enable the exp display, timer, score popup and race position
		Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, dataobject);
	}

	float timer;
	public float fTimerTarget = 10.0f;
	private void Update() {
		if (timer == fTimerTarget) {
			return;
		}

		timer += Time.deltaTime;
		if (timer >= fTimerTarget) {
			timer = fTimerTarget;
			Bird.HUDController.hudElementToggleMultiData_t dataobject2 = new Bird.HUDController.hudElementToggleMultiData_t();
			dataobject2.m_nPlayerID = 3; // Target player ID 0 = all players (otherwise, it's 1 - 4)
			dataobject2.m_nState = Bird.HUDController.hudElementToggleData_t.elementState_e.ENABLE;
			dataobject2.m_ArrayTypes = new System.Type[] { typeof(Bird.HUD_EXP), typeof(Bird.HUD_Timer), typeof(Bird.HUD_ScorePopupMgr), typeof(Bird.HUD_RacePosition), typeof(Bird.HUD_NavArrow) };

			// This will enable the exp display, timer, score popup and race position
			Kojima.EventManager.m_instance.AddEvent(Kojima.Events.Event.UI_HUD_TOGGLE_ELEMENT, dataobject2);
		}
	}

}
