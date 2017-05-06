using UnityEngine;
using System.Collections;

namespace Bird {
	public class MenuButton_UnPauseGame : MenuButton_ChangeScreen {
		public override void OnButtonPress(BaseMenuScreen parentMenu) {
			Time.timeScale = 1.0f;
			base.OnButtonPress(parentMenu);
		}
	}
}