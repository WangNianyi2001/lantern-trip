using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

namespace LanternTrip {
	[CreateAssetMenu(menuName = "LanternTrip/GameSettings")]
	public class GameSettings : ScriptableObject {
		public uint lanternSlotCount;
		public List<Bonus> bonuses;
		[Min(0)] public int respawnCinderCost;
		public Tinder respawnGift;
		[Scene] public string mainScene;
		[Scene] public string gameOverScene;
	}
}