using UnityEngine;

namespace LanternTrip {
	public class GameOverManager : MonoBehaviour {
		public GameSettings settings;

		public void OnReplay() {
			SceneLoader.instance.LoadAsync(settings.mainScene);
		}
	}
}