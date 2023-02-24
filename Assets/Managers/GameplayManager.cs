using UnityEngine;

namespace LanternTrip {
	[RequireComponent(typeof(InputManager))]
	public class GameplayManager : ManagerBase {
		public static GameplayManager instance;

		public struct TinderSlot {
			public Tinder type;
			public float fuelLast;
		}

		#region Inspector members
		new public Protagonist protagonist;
		#endregion

		#region Core members
		InputManager input;
		const uint tinderSlotCount = 3u;
		TinderSlot[] tinderSlots;
		#endregion

		#region Life cycle
		void Awake() {
			instance = this;
		}

		void Start() {
			input = GetComponent<InputManager>();
			tinderSlots = new TinderSlot[tinderSlotCount];
			for(int i = 0; i < tinderSlotCount; ++i) {
				tinderSlots[i] = new TinderSlot {
					type = null,
					fuelLast = 0,
				};
			}
		}
		#endregion
	}
}