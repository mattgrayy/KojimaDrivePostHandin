using UnityEngine;
using System.Collections;
namespace Bird {
	public class UI_BTN_ResetSavedEXP : MenuButton_Listener {


		public override void OnButtonPress(BaseMenuScreen parentMenu) {
			HF.ExperienceManager.ResetGlobalEXP(); // Goodbye, progress
		}

		public override void OnButtonSelect(BaseMenuScreen parentMenu) {
			
		}

		public override void OnButtonDeselect(BaseMenuScreen parentMenu) {
			
		}
	}
}