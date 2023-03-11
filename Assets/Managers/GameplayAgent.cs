using UnityEngine;

namespace LanternTrip {
	[CreateAssetMenu(menuName = "LanternTrip/GameplayAgent")]
	public class GameplayAgent : ScriptableObject {
		public GameplayManager manager => GameplayManager.instance;

		public void Log(string text) => Debug.Log(text);
		public void LogWarning(string text) => Debug.LogWarning(text);

		public void LoadTinder(Tinder tinder) => manager.LoadTinder(tinder);

		public void AddBonusTime(float time) => manager.AddBonusTime(time);
	}
}