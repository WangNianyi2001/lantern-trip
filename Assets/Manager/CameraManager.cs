using UnityEngine;
using Cinemachine;
using System;

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
		public CinemachineVirtualCamera vCam;
		[SerializeField] CameraMode mode;
		[Range(0, Mathf.PI / 2)] public float followingZenith;
		[Range(0, 50)] public float followingDistance;
		[Range(0, 50)] public float orbitalDistance;
		
		CinemachineOrbitalTransposer orbitTransposer;

		public Vector3 FollowOffset {
			get => orbitTransposer.m_FollowOffset;
			set => orbitTransposer.m_FollowOffset = value;
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
			get => FollowOffset.magnitude;
			set => FollowOffset = FollowOffset.normalized * value;
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

		void Start() {
			orbitTransposer = vCam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineOrbitalTransposer;
			Mode = Mode;
		}
	}
}