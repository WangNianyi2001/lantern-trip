using UnityEngine;
using UnityEngine.InputSystem;

namespace LanternTrip {
	[RequireComponent(typeof(PlayerInput))]
	public class InputManager : ManagerBase {
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
			if(protagonist == null)
				return;
			Vector2 raw = value.Get<Vector2>();
			protagonist.movement.inputVelocity = new Vector3(
				raw.x,
				0,
				raw.y
			);
		}

		void OnPlayerJump(InputValue _) {
			protagonist?.Jump();
		}
		#endregion

		#region Life cycle
		void Start() {
			// Get component references
			playerInput = GetComponent<PlayerInput>();

			// Initialize main game
			GainPlayerControl();
		}
		#endregion
	}
}