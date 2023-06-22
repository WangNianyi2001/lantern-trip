using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using NaughtyAttributes;
using System;

namespace LanternTrip {
	[RequireComponent(typeof(PlayerInput))]
	public class InputManager : ManagerBase {
		public enum InputCoordinate {
			World, Camera, Protagonist,
		};

		#region Core members
		PlayerInput playerInput;
		Vector2 mousePosition = new Vector2();
		[NonSerialized] public Vector3 rawInputMovement;
		#endregion

		#region Inspector members
		public InputCoordinate coordinate;
		new public Camera camera;
		public CinemachineVirtualCamera orbitalCamera;
		[MinMaxSlider(0, 90)] public Vector2 orbitAzimuthRange;
		#endregion

		#region Public interfaces
		public void GainPlayerControl() {
			playerInput.SwitchCurrentActionMap("Player");
			playerInput.ActivateInput();
		}

		public void DiscardPlayerControl()
		{
			playerInput.DeactivateInput();
		}

		public Vector2 MousePosition => mousePosition;
		#endregion

		#region Input handlers
		public void OnTogglePause() {
			gameplay.Paused ^= true;
		}

		public void OnPlayerMove(InputValue value) {
			if(protagonist == null)
				return;
			Vector2 raw = value.Get<Vector2>();
			Vector3 raw3 = new Vector3(raw.x, 0, raw.y);
			rawInputMovement = raw3;
		}

		public void OnPlayerJump(InputValue _) {
			protagonist?.Jump();
		}

		public void OnPlayerInteract(InputValue _) {
			var selector = gameplay.protagonist.selector;
			selector.UseCurrentSelection();
		}

		public void OnPlayerScrollSlot(InputValue value) {
			float raw = value.Get<float>();
			if(raw == 0)
				return;
			int delta = (int)Mathf.Sign(raw);
			gameplay.ScrollSlot(delta);
		}

		public void OnPlayerBow(InputValue _) {
			gameplay.protagonist.HoldingBow ^= true;
		}

		public void OnPlayerAim(InputValue value) {
			mousePosition = value.Get<Vector2>();
		}

		public void OnPlayerChargeUp(InputValue value) {
			float raw = value.Get<float>();
			gameplay.protagonist.ChargeUpSpeed = raw;
		}

		public void OnPlayerToggleCameraMode(InputValue _) {
			switch(gameplay.camera.Mode) {
				case CameraMode.Orbital:
					gameplay.camera.SetFollowing(FollowingCameraMode.PositiveY);
					break;
				case CameraMode.Following:
					gameplay.camera.Mode = CameraMode.Orbital;
					break;
			}
		}
		public void OnPlayerOrientCamera(InputValue value) {
			if(gameplay.camera.Mode != CameraMode.Orbital)
				return;
			Vector2 raw = value.Get<Vector2>();
			gameplay.camera.Azimuth += raw.x * Mathf.PI / 180;
			gameplay.camera.Zenith += raw.y * Mathf.PI / 180;
		}

		public void OnPlayerDash(InputValue _) {
			protagonist?.Dash();
		}

		public void OnPlayerKick(InputValue _) {
			protagonist?.Kick();
		}

		public void OnPlayerSwitchProp(InputValue _) {
			++gameplay.PropIndex;
		}
		public void OnPlayerUseProp(InputValue _) {
			gameplay.UseCurrentProp();
		}

#if DEBUG
		public void OnPlayerCheat(InputValue _) {
			gameplay.Cheating ^= true;
		}
#endif
		#endregion

		#region Life cycle
		protected void OnEnable() {
			// Get component references
			playerInput = GetComponent<PlayerInput>();
			playerInput.actions.FindActionMap("Pause").Enable();

			// Initialize main game
			GainPlayerControl();
			Cursor.lockState = CursorLockMode.Locked;
		}

		protected void OnDisable() {
			Cursor.lockState = CursorLockMode.None;
		}

		protected void FixedUpdate() {
			// Movement
			Vector3 v = rawInputMovement;
			Quaternion q = Quaternion.identity;
			switch(coordinate) {
				case InputCoordinate.World:
					break;
				case InputCoordinate.Protagonist:
					q = protagonist.transform.rotation;
					break;
				case InputCoordinate.Camera:
					q = camera.transform.rotation;
					break;
			}
			Vector3 euler = q.eulerAngles;
			euler.x = 0;
			q = Quaternion.Euler(euler);
			v = q * v;
			protagonist.inputVelocity = v;
		}
		#endregion
	}
}