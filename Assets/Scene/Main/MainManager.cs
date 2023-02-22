using UnityEngine;
using UnityEngine.InputSystem;

namespace LanternTrip {
	[RequireComponent(typeof(PlayerInput))]
	public class MainManager : MonoBehaviour {
		public static MainManager instance;

		#region Inspector members
		public Player player;
		#endregion

		#region Core members
		PlayerInput playerInput;
		#endregion

		#region Public interfaces
		public void GainPlayerControl() {
			playerInput.SwitchCurrentActionMap("Player");
			playerInput.ActivateInput();
		}
		#endregion

		#region Input handlers
		void OnPlayerMove(InputValue value) {
			Vector2 rawVelocity = value.Get<Vector2>();
			Vector3 convertedVelocity = new Vector3 {
				x = rawVelocity.x,
				y = 0,
				z = rawVelocity.y
			};
			player.desiredVelocity = convertedVelocity;
		}
		#endregion

		#region Life cycle
		void Awake() {
			instance = this;
		}

		void Start() {
			// Get component references
			playerInput = GetComponent<PlayerInput>();

			// Initialize main game
			GainPlayerControl();
		}
		#endregion
	}
}