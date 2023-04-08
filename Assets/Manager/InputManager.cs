using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using NaughtyAttributes;

namespace LanternTrip {
	[RequireComponent(typeof(PlayerInput))]
	public class InputManager : ManagerBase {
		public enum InputCoordinate {
			World, Camera, Protagonist,
		};

		#region Core members
		PlayerInput playerInput;
		Vector2 mousePosition = new Vector2();
		Vector3 rawInputMovement;
		bool orientingCamera = false;
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

		public Vector2 MousePosition => mousePosition;
		#endregion

		#region Input handlers
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

		public void OnPlayerChargeUp(InputValue value) {
			float raw = value.Get<float>();
			gameplay.ChargeUpSpeed = raw;
		}

		public void OnPlayerToggleOrientCamera(InputValue value) {
			orientingCamera = value.Get<float>() > .5f;
		}
		public void OnPlayerToggleCameraMode(InputValue _) {
			var mode = gameplay.camera.Mode;
			var enumValues = typeof(GameCameraMode).GetEnumValues();
			int n = enumValues.GetLength(0);
			int i = ((int)mode + 1) % n;
			gameplay.camera.Mode = (GameCameraMode)enumValues.GetValue(i);
		}
		public void OnPlayerOrientCamera(InputValue value) {
			if(!orientingCamera)
				return;
			Vector2 raw = value.Get<Vector2>();
			gameplay.camera.Azimuth += raw.x * Mathf.PI / 180;
			gameplay.camera.Zenith += raw.y * Mathf.PI / 180;
		}
		#endregion

		#region Life cycle
		void Start() {
			// Get component references
			playerInput = GetComponent<PlayerInput>();

			// Initialize main game
			GainPlayerControl();
		}

		void FixedUpdate() {
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