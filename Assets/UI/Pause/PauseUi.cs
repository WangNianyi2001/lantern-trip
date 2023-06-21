using UnityEngine;

namespace LanternTrip {
	[RequireComponent(typeof(RectTransform))]
	public class PauseUi : MonoBehaviour {
		protected void OnEnable() {
			if(GameplayManager.instance) {
				GameplayManager.instance.Paused = true;
			}
			Cursor.lockState = CursorLockMode.None;
		}

		protected void OnDisable() {
			if(GameplayManager.instance) {
				GameplayManager.instance.Paused = false;
			}
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
}