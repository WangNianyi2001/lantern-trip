using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;

namespace LanternTrip {
	public class MoveAlongPath : MonoBehaviour {
		//public enum MoveMode { ByDistance, BySegment }
		//public MoveMode moveMode;
		[Min(0)] public float speed = 1;
		public bool beginOnEnable;
		[Expandable] [Instance] public MovingPath path;

		Coroutine movingCoroutine = null;
		float distance = 0;

		void UpdateProgress(KeyValuePair<int, float> progress) {
			transform.position = path.Position(progress);
			transform.rotation = path.Rotation(progress);
		}

		public float Distance {
			get => distance;
			set {
				var p = path.ProgressByDistance(value);
				UpdateProgress(p);
				distance = value;
			}
		}

		IEnumerator MovingCoroutine() {
			yield return new WaitForFixedUpdate();
			while(true) {
				if(movingCoroutine == null) {
					StopMoving();
					yield break;
				}
				var nextD = Distance + speed * Time.fixedDeltaTime;
				if(nextD >= path.Length)
					break;
				Distance = nextD;
				yield return new WaitForFixedUpdate();
			}
			Distance = path.Length;
			StopMoving();
		}

		[ContextMenu("Start Moving")]
		public void StartMoving() {
			movingCoroutine = StartCoroutine(MovingCoroutine());
		}

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