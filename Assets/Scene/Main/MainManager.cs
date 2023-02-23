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
		Vector3 inputMovementVelocity;
		#endregion

		#region Public interfaces
		public void GainPlayerControl() {
			playerInput.SwitchCurrentActionMap("Player");
			playerInput.ActivateInput();
		}
		#endregion

		#region Input handlers
		void OnPlayerMove(InputValue value) {
			Vector2 raw = value.Get<Vector2>();
			player.movement.inputVelocity = new Vector3(
				raw.x,
				0,
				raw.y
			);
		}
		void OnPlayerJump(InputValue _) {
			//
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