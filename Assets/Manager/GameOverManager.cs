using UnityEngine;
using UnityEngine.SceneManagement;

namespace LanternTrip {
	public class GameOverManager : MonoBehaviour {
		public GameSettings settings;

		public void OnReplay() {
			SceneManager.LoadScene(settings.mainScene);
		}
	}
}