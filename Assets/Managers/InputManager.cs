using UnityEngine;
using UnityEngine.InputSystem;

namespace LanternTrip {
	[RequireComponent(typeof(PlayerInput))]
	public class InputManager : ManagerBase {
		#region Core members
		PlayerInput playerInput;
		Vector2 mousePosition = new Vector2();
		#endregion

		#region Public interfaces
		public void GainPlayerControl() {
			playerInput.SwitchCurrentActionMap("Player");
			playerInput.ActivateInput();
		}

		public Vector2 MousePosition => mousePosition;
		#endregion

		#region Input handlers
		public void OnPlayerMove(InputValue value) {
			if(protagonist == null)
				return;
			Vector2 raw = value.Get<Vector2>();
			protagonist.movement.inputVelocity = new Vector3(
				raw.x,
				0,
				raw.y
			);
		}

		public void OnPlayerJump(InputValue _) {
			protagonist?.Jump();
		}

		public void OnPlayerLoadTinder(InputValue _) {
			gameplay.LoadTinderFromCurrentSource();
		}

		public void OnPlayerScrollSlot(InputValue value) {
			float raw = value.Get<float>();
			if(raw == 0)
				return;
			int delta = (int)Mathf.Sign(raw);
			gameplay.ScrollSlot(delta);
		}

		public void OnPlayerBow(InputValue _) {
			gameplay.HoldingBow ^= true;
		}

		public void OnPlayerAim(InputValue value) {
			mousePosition = value.Get<Vector2>();
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