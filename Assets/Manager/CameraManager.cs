using UnityEngine;
using Cinemachine;
using System;
using NaughtyAttributes;

namespace LanternTrip {
	public enum CameraMode {
		Orbital,
		Following,
	}

	public enum FollowingCameraMode {
		PositiveX, NegativeX,
		PositiveY, NegativeY,
	}

	public class CameraManager : ManagerBase {
		#region Serialized fields
		public new Camera camera;
		public CinemachineVirtualCamera vCam;
		[SerializeField] CameraMode mode;
		[Range(0, Mathf.PI / 2)] public float followingZenith;
		[Range(0, 50)] public float followingDistance;
		[Range(0, 50)] public float orbitalDistance;
		public bool useRayCast = true;
		public LayerMask rayCastLayer;
		[MinMaxSlider(-90, 90)] public Vector2 zenithRange;
		[Range(0, 1)] public float damp;
		#endregion

		#region Internal fields
		CinemachineOrbitalTransposer orbitTransposer;
		CinemachineComposer composer;
		float distance;
		Vector3 followOffset;
		#endregion

		#region Public interfaces
		public Vector3 FollowOffset {
			get => followOffset - AimOffset;
			set => followOffset = value + AimOffset;
		}
		public Vector3 AimOffset => composer.m_TrackedObjectOffset;
		public Ray FollowRay {
			get {
				var offset = AimOffset;
				var from = orbitTransposer.FollowTarget.position + offset;
				var direction = Quaternion.Euler(0, Azimuth * 180 / Mathf.PI, 0) * FollowOffset;
				return new Ray(from, direction);
			}
		}

		public float Zenith {
			get => FollowOffset.Zenith();
			set => FollowOffset = new Vector3(0, Mathf.Sin(value), -Mathf.Cos(value)) * Distance;
		}
		public float Azimuth {
			get => orbitTransposer.m_XAxis.Value * Mathf.PI / 180;
			set {
				var xAxis = orbitTransposer.m_XAxis;
				xAxis.Value = value * 180 / Mathf.PI;
				orbitTransposer.m_XAxis = xAxis;
			}
		}
		public float Distance {
			get => distance;
			set => distance = value;
		}

		public void SetFollowing(FollowingCameraMode mode) {
			Mode = CameraMode.Following;
			float azimuth = 0;
			switch(mode) {
				case FollowingCameraMode.PositiveY:
					azimuth = 0;
					break;
				case FollowingCameraMode.PositiveX:
					azimuth = 90;
					break;
				case FollowingCameraMode.NegativeY:
					azimuth = 180;
					break;
				case FollowingCameraMode.NegativeX:
					azimuth = 270;
					break;
			}
			Azimuth = azimuth;
		}

		public CameraMode Mode {
			get => mode;
			set {
				switch(mode = value) {
					case CameraMode.Orbital:
						Distance = orbitalDistance;
						break;
					case CameraMode.Following:
						Distance = followingDistance;
						Zenith = followingZenith;
						break;
				}
			}
		}
		#endregion

		#region Life cycle
		void Start() {
			orbitTransposer = vCam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineOrbitalTransposer;
			composer = vCam.GetCinemachineComponent(CinemachineCore.Stage.Aim) as CinemachineComposer;
			followOffset = orbitTransposer.m_FollowOffset;
			Mode = Mode;
			distance = FollowOffset.magnitude;
		}

		void Update() {
			// Lock zenith
			float z = Zenith * 180 / Mathf.PI;
			z = Mathf.Clamp(z, zenithRange.x, zenithRange.y);
			z = z * Mathf.PI / 180;
			Zenith = z;

			// Ray cast camera position
			var castedDistance = distance;
			if(useRayCast) {
				RaycastHit hit;
				Ray ray = FollowRay;
				Physics.Raycast(ray, out hit, distance, rayCastLayer);
				if(hit.collider != null)
					castedDistance = (hit.point - ray.origin).magnitude * .9f;
			}
			FollowOffset = FollowOffset.normalized * castedDistance;
		}

		void FixedUpdate() {
			orbitTransposer.m_FollowOffset = Vector3.Lerp(
				orbitTransposer.m_FollowOffset,
				followOffset,
				1 - Mathf.Exp(-Time.fixedDeltaTime / damp)
			);
		}

		private void OnDrawGizmos() {
			if(!Application.isPlaying)
				return;
			Gizmos.DrawRay(FollowRay);
		}
		#endregion
	}
}