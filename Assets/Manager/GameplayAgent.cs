using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;

namespace LanternTrip {
	[CreateAssetMenu(menuName = "LanternTrip/GameplayAgent")]
	public class GameplayAgent : ScriptableObject {
		public GameplayManager gameplay => GameplayManager.instance;

		public void Log(string text) => Debug.Log(text);
		public void LogWarning(string text) => Debug.LogWarning(text);

		public void LoadTinder(Tinder tinder) => gameplay.LoadTinder(tinder);

		public void GrantBonusTime(float time) => gameplay.GrantBonusTime(time);
		public void GrantExtraGreenTime(float time) {
			foreach(var slot in gameplay.lanternSlots.Where(slot => slot.tinder.type == Tinder.Type.Green))
				slot.timeLeft += time;
		}

		public float SpeedBonusRate {
			get => gameplay.speedBonusRate;
			set => gameplay.speedBonusRate *= value;
		}
		public bool ColdDebuffEnabled {
			get => gameplay.coldDebuffEnabled;
			set => gameplay.coldDebuffEnabled = value;
		}

		public bool Burning {
			get => gameplay.burning;
			set => gameplay.burning = value;
		}

		public void EnterSafezone() => gameplay.EnterSafezone();
		public void ExitSafezone() => gameplay.ExitSafezone();
		public void EnterColdzone() => gameplay.EnterColdzone();
		public void ExitColdzone() => gameplay.ExitColdzone();

		public void SetOrbitalCamera() => gameplay.camera.Mode = CameraMode.Orbital;
		public void SetFollowingCamera(FollowingCameraMode mode) => gameplay.camera.SetFollowing(mode);

		public void SetCheckpoint(Checkpoint point) => gameplay.LastCheckpoint = point;
		public void RestartLevel() => gameplay.RestartLevel();

		public void Start() => gameplay.StartGame();
		public void Pause() => gameplay.Paused = true;
		public void Resume() => gameplay.Paused = false;
		public void Quit() {
#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}