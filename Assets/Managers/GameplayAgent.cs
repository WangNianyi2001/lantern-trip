using UnityEngine;

namespace LanternTrip {
	[CreateAssetMenu(menuName = "LanternTrip/GameplayAgent")]
	public class GameplayAgent : ScriptableObject {
		public GameplayManager manager => GameplayManager.instance;

		public void LoadTinder(Tinder tinder) => manager.LoadTinder(tinder);
	}
}