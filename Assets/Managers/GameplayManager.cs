using UnityEngine;

namespace LanternTrip {
	[RequireComponent(typeof(InputManager))]
	public class GameplayManager : ManagerBase {
		public static GameplayManager instance;

		#region Inspector members
		new public Protagonist protagonist;
		#endregion

		#region Core members
		InputManager input;
		#endregion

		#region Life cycle
		void Awake() {
			instance = this;
		}

		void Start() {
			input = GetComponent<InputManager>();
		}
		#endregion
	}
}