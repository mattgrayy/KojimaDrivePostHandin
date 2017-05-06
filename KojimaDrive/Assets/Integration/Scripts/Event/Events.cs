//===================== Kojima Drive - Half-Full Games 2017 ====================//
//
// Author:  Harry Davies
// Purpose: Used to create event call lists, by adding a new entry, adds a new
//          event call list.
// Namespace: HF
//
//===============================================================================//

using UnityEngine;
using System.Collections;

//================================== Event Key ==================================//
//
//                              ST_ = State Events
//                              UI_ = UI Event
//                              GM_ = Game Mode Events
//                              DS_ = Drive and Seek
//                              EX_ = Example Event
//                              TW_ = Twist Event
//
//
//===============================================================================//

namespace Kojima {
	public class Events : MonoBehaviour {
		public enum Event {
			ST_STATECHANGED,
			UI_UICHANGED,
			GM_FREEROAM,
			GM_DRIVEANDSEEK,
			GM_EXAMPLE,
			GM_VOLCANOMADNESS,
			GM_VOLCANOMADNESS2,
			GM_SABOTAGE,
            GM_CARAVAN,
            GM_CARAVANSCORE,
            GM_RACE,
			GM_PASSTHEBOMB,
			GM_LIGHTBLADE,
			GM_PARKING,
			GM_MONKEYTARGET,
			TW_START,
			TW_STOP,
			DS_SETUP,
			DS_RUNNING,
			DS_CHASING,
			DS_RESET,
			DS_FINISH,
			DS_DYNAMIC,
			DS_HIDERREADY,
			EX_SETUP,
			EX_PLAY,
			EX_RESET,
			EX_FINISH,
			EX_BUFFER,

			CAR_SWAPPED,			// When a car is swapped

			// UI Events
			// Transitions
			UI_TRANS_STARTED,       // Triggered when a transition begins
			UI_TRANS_INTERRUPT,     // Call this to interrupt any ongoing transitions
			UI_TRANS_ENDED,         // Triggered when a transition ends
			UI_TRANS_SCREENCOVERED,	// Called when the screen is covered by a transition

			// HUD
			UI_HUD_SHOW_EXP,          // Displays the Drive Points meter (DATA = Bird.HUD_EXP.hudEXPData_t)
			UI_HUD_EXP_MODE,          // Sets the drive points meter mode (DATA = Bird.HUD_EXP.hudEXPModeData_t)
			UI_HUD_SHOW_AREANAME,     // Display the Area Name (DATA = Bird.HUD_Area.hudAreaData_t)
			UI_HUD_SHOW_EVENTSTART,   // Display event title (DATA = Bird.HUD_EventStart.hudEventStartData_t)
			UI_HUD_SHOW_EXP_POPUP,    // Display the crazy taxi style points popup (DATA = Bird.HUD_ScorePopupMgr.hudScorePopupData_t)

			UI_HUD_ARROW_ADD_CHECKPOINT,    // Add a checkpoint		(DATA = hudCheckpointData_t)
			UI_HUD_ARROW_REACHED_CHECKPOINT,    // Player has reached a checkpoint (DATA = Bird.hudArrowReachedData_t)
			UI_HUD_ARROW_REMOVE_CHECKPOINT, // Remove a checkpoint	(DATA = hudCheckpointData_t)

			UI_HUD_TOGGLE_ELEMENT,      // Toggle a HUD element of specified type (DATA = Bird.HUDController.hudElementToggleData_t/Bird.HUDController.hudElementToggleMultiData_t)
			UI_HUD_HIDE_ALL_ELEMENTS,   // Used when setting up a new HUD to hide all current elements (OPTIONAL DATA = int playerID)

			UI_TOGGLE_PAUSE,              // Pause/unpause the game. (DATA = Bird.UI_PauseMenu.pauseUIToggleData_t)

			UI_TRANS_DEFAULT_IN,
			UI_TRANS_DEFAULT_OUT,

			// Add events above this line!
			Count
		}
	}
}