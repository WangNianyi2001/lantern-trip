using UnityEngine;
using Cinemachine;

namespace LanternTrip {
	public enum GameCameraMode {
		Orbital,
		Following,
	}

	public class CameraManager : ManagerBase {
		public CinemachineVirtualCamera vCam;
		[SerializeField] GameCameraMode mode;
		
		CinemachineOrbitalTransposer orbitTransposer;

		public Vector3 FollowOffset {
			get => orbitTransposer.m_FollowOffset;
			set => orbitTransposer.m_FollowOffset = value;
		}

		public float Zenith {
			get => FollowOffset.Zenith();
			set {
				if(mode != GameCameraMode.Orbital)
					return;
				FollowOffset = new Vector3(0, Mathf.Sin(value), -Mathf.Cos(value)) * Distance;
			}
		}
		public float Azimuth {
			get => orbitTransposer.m_XAxis.Value * Mathf.PI / 180;
			set {
				if(mode != GameCameraMode.Orbital)
					return;
				var xAxis = orbitTransposer.m_XAxis;
				xAxis.Value = value * 180 / Mathf.PI;
				orbitTransposer.m_XAxis = xAxis;
			}
		}
		public float Distance {
			get => FollowOffset.magnitude;
			set {
				if(mode != GameCameraMode.Orbital)
					return;
				FollowOffset = FollowOffset.normalized * value;
			}
		}

		public GameCameraMode Mode {
			get => mode;
			set {
				mode = value;
			}
		}

		void Start() {
			orbitTransposer = vCam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineOrbitalTransposer;
		}
	}
}