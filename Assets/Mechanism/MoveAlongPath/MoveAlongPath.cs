using UnityEngine;
using System.Collections;
using NaughtyAttributes;

namespace LanternTrip {
	public class MoveAlongPath : MonoBehaviour {
		[Min(0)] public float speed = 1;
		public bool reversed;
		public bool alternate;
		public bool repeat;
		public bool forever;
		public bool beginOnEnable;
		[Expandable] [Instance] public MovingPath path;

		Coroutine movingCoroutine = null;
		float progress = 0;
		float direction = 1;
		float paused = 1;
		int bounceLeft = 0;

		void UpdateProgress(float progress) {
			transform.position = path.Position(progress);
			if(path.useRotation)
				transform.rotation = path.Rotation(progress);
		}

		public float Progress {
			get => progress;
			set {
				progress = Mathf.Clamp(value, 0, path.MaxProgress);
				UpdateProgress(progress);
			}
		}

		bool Next() {
			if(movingCoroutine == null)
				return false;

			float delta = Time.fixedDeltaTime * speed;
			delta *= direction;
			delta *= paused;
			float expectedProgress = Progress + delta;
			Progress = expectedProgress;
			if(Progress != expectedProgress) {  // µΩÕ∑¡À
				if(!forever) {
					if(--bounceLeft <= 0)
						return false;
				}
				direction *= -1;
			}

			return true;
		}

		IEnumerator MovingCoroutine() {
			yield return new WaitForFixedUpdate();
			while(true) {
				if(!Next()) {
					StopMoving();
					yield break;
				}
				yield return new WaitForFixedUpdate();
			}
		}

		[ContextMenu("Start Moving")]
		public void StartMoving() {
			if(!reversed) {
				direction = 1;
				Progress = 0;
			}
			else {
				direction = -1;
				Progress = path.MaxProgress;
			}
			if(!forever)
				bounceLeft = alternate ? 2 : 1;
			movingCoroutine = StartCoroutine(MovingCoroutine());
		}

		[ContextMenu("Pause Moving")]
		public void PauseMoving() => paused = 0;

		[ContextMenu("Continue Moving")]
		public void ContinueMoving() => paused = 1;

		[ContextMenu("Stop Moving")]
		public void StopMoving() {
			if(movingCoroutine != null) {
				StopCoroutine(movingCoroutine);
				movingCoroutine = null;
			}
		}

		void OnEnable() {
			if(beginOnEnable)
				StartMoving();
		}

		void OnDisable() {
			StopMoving();
		}

		void OnDrawGizmos() {
			if(path) {
				path.DrawGizmos();
			}
		}
	}
}