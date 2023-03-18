using UnityEngine;

namespace LanternTrip {
	[CreateAssetMenu(menuName = "LanternTrip/GameplayAgent")]
	public class GameplayAgent : ScriptableObject {
		public GameplayManager gameplay => GameplayManager.instance;

		public void Log(string text) => Debug.Log(text);
		public void LogWarning(string text) => Debug.LogWarning(text);

		public void LoadTinder(Tinder tinder) => gameplay.LoadTinder(tinder);

		public void AddBonusTime(float time) => gameplay.AddBonusTime(time);

		public bool Burning {
			get => gameplay.burning;
			set => gameplay.burning = value;
		}
	}
}