using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

namespace LanternTrip {
	[CreateAssetMenu(menuName = "LanternTrip/GameSettings")]
	public class GameSettings : ScriptableObject {
		[Min(1)] public float fps;
		public uint lanternSlotCount;
		public List<Bonus> bonuses;
		[Min(0)] public int respawnCinderCost;
		public Tinder[] respawnGifts;
		[Scene] public string[] mainScene;
		[Scene] public string gameOverScene;
		public Material arrowTriggerMaterial;
		public float protagonistInitialHp;
	}
}