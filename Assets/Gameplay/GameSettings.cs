using UnityEngine;
using System.Collections.Generic;

namespace LanternTrip {
	[CreateAssetMenu(menuName = "LanternTrip/GameSettings")]
	public class GameSettings : ScriptableObject {
		public uint lanternSlotCount;
		public List<Bonus> bonuses;
	}
}