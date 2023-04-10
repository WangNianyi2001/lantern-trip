using UnityEngine;
using System.Collections;
using NaughtyAttributes;
using System;

namespace LanternTrip {
	public class MoveAlongPath : MonoBehaviour {
		[Min(0)] public float speed = 1;
		public bool reversed;
		public bool alternate;
		public bool forever;
		public bool beginOnEnable;
		[Expandable] [Instance] public MovingPath path;

		Coroutine movingCoroutine = null;
		float progress = 0;
		[NonSerialized] public float direction = 1;
		[NonSerialized] public int bounceLeft = 0;

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
		public bool Moving => movingCoroutine != null;

		bool Next() {
			if(!Moving)
				return false;

			float delta = Time.fixedDeltaTime * speed;
			delta *= direction;
			float expectedProgress = Progress + delta;
			Progress = expectedProgress;
			if(Progress != expectedProgress) {  // ��ͷ��
				if(!forever) {
					if(--bounceLeft <= 0)
						return false;
				}
				if(alternate) {
					direction *= -1;
				} else {
					Progress = direction > 0 ? 0 : path.MaxProgress;
				}
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
			if(path == null) {
				Debug.LogWarning("Path is null");
				return;
			}
			if(path.anchors.Count <= 1) {
				Debug.LogWarning("Path to move along must has at least 2 anchors");
				return;
			}
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
			ContinueMoving();
		}

		[ContextMenu("Continue Moving")]
		public void ContinueMoving() {
			movingCoroutine = StartCoroutine(MovingCoroutine());
		}

		[ContextMenu("Stop Moving")]
		public void StopMoving() {
			if(Moving) {
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
			path?.DrawGizmos();
		}
	}
}