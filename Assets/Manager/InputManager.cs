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
		bool isOrientingCamera = false;
		float orbitDistance = 1;
		CinemachineOrbitalTransposer orbit;
		Vector2 targetOrbitOrientation, actualOrbitOrientation;
		#endregion

		#region Inspector members
		public InputCoordinate coordinate;
		new public Camera camera;
		public CinemachineVirtualCamera orbitalCamera;
		[MinMaxSlider(0, 90)] public Vector2 orbitAzimuthRange;
		[Range(0, 2)] public float orbitGain;
		[Range(0, 5)] public float orbitDamp;
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
			isOrientingCamera = value.Get<float>() > .5f;
		}
		public void OnPlayerOrientCamera(InputValue value) {
			if(!isOrientingCamera || !orbit)
				return;

			Vector2 raw = value.Get<Vector2>() * orbitGain;
			targetOrbitOrientation.y += raw.x;
			targetOrbitOrientation.x += raw.y;
		}
		#endregion

		#region Life cycle
		void Start() {
			// Get component references
			playerInput = GetComponent<PlayerInput>();

			// Initialize main game
			GainPlayerControl();

			if(orbitalCamera) {
				var transposer = orbitalCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
				if(transposer is CinemachineOrbitalTransposer) {
					orbit = transposer as CinemachineOrbitalTransposer;
					orbitDistance = orbit.m_FollowOffset.magnitude;
				}
			}
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
			protagonist.movement.inputVelocity = v;

			// Orbital camera orientation
			if(orbit) {
				targetOrbitOrientation.x = Mathf.Clamp(targetOrbitOrientation.x, orbitAzimuthRange.x, orbitAzimuthRange.y);
				actualOrbitOrientation = Vector2.Lerp(actualOrbitOrientation, targetOrbitOrientation, Mathf.Exp(-orbitDamp));
				orbit.m_FollowOffset = Quaternion.Euler(actualOrbitOrientation) * Vector3.forward * -orbitDistance;
			}
		}
		#endregion
	}
}