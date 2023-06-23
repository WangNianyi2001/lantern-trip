using UnityEngine;
using System.Collections;

namespace LanternTrip {
	public class GameOverManager : MonoBehaviour {
		public GameSettings settings;

		public void OnReplay() {
			SceneLoader.instance.LoadMain();
		}
	}
}