using UnityEngine;

namespace LanternTrip {
	[RequireComponent(typeof(InputManager))]
	public class GameplayManager : MonoBehaviour {
		#region Core members
		InputManager input;
		#endregion

		#region Life cycle
		void Start() {
			input = GetComponent<InputManager>();
		}
		#endregion
	}
}