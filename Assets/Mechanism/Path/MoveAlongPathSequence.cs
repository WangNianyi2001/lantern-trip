using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;

namespace LanternTrip {
	public class MoveAlongPathSequence : MonoBehaviour {
		#region Internal fields
		float progress = 0;
		Coroutine movingCoroutine = null;
		int targetIndex;
		#endregion

		#region Serialized fields
		[Min(0)] public float speed = 1;
		public float direction = 1;
		public bool startOnAwake = false;
		[Expandable] [Instance] public List<MovingPath> paths = new List<MovingPath>();
		#endregion

		#region Internal functions
		IEnumerator MovingCoroutine() {
			StopMoving();
			direction = Mathf.Sign(targetIndex - Progress);
			while(true) {
				yield return new WaitForFixedUpdate();
				float delta = speed * Time.fixedDeltaTime * direction;
				float expectedProgress = Progress + delta;
				if(direction * (expectedProgress - targetIndex) >= 0) {
					Progress = targetIndex;
					break;
				}
				Progress = expectedProgress;
			}
			StopMoving();
		}
		#endregion

		#region Public interfaces
		public bool Moving => movingCoroutine != null;
		public float MaxProgress => paths.Count;
		public float Progress {
			get => progress;
			set {
				progress = Mathf.Clamp(value, 0, MaxProgress);
				int i = (int)Mathf.Min(paths.Count - 1, Mathf.Floor(progress));
				float frac = progress - i;
				paths[i].Apply(transform, frac);
			}
		}

		public int MaxIndex => (int)MaxProgress;
		public int CurrentIndex {
			get {
				float i = Progress;
				if(direction > 0)
					i = Mathf.Floor(i);
				else
					i = Mathf.Ceil(i);
				i = Mathf.Clamp(i, 0, MaxProgress);
				return (int)i;
			}
		}
		public int NextIndex {
			get {
				if(direction > 0) {
					if(CurrentIndex >= MaxProgress)
						return paths.Count - 1;
					return CurrentIndex + 1;
				}
				else {
					if(CurrentIndex <= 0)
						return 1;
					return CurrentIndex - 1;
				}
			}
		}

		public void MoveTo(int i) {
			if(Moving)
				StopMoving();
			targetIndex = (int)Mathf.Clamp(i, 0, MaxProgress);
			movingCoroutine = StartCoroutine(MovingCoroutine());
			
		}

		[ContextMenu("Move To Next")]
		public void MoveToNext() {
			MoveTo(NextIndex);
		}

		[ContextMenu("Stop Moving")]
		public void StopMoving() {
			if(Moving) {
				StopCoroutine(movingCoroutine);
				movingCoroutine = null;
			}
			Debug.Log("Stop Moving");
		}
		#endregion

		#region Life cycle

		private void Start()
		{
			if (startOnAwake)
			{
				MoveToNext();
			}
		}

		void OnDisable() {
			StopMoving();
		}

		void OnDrawGizmos() {
			foreach(var path in paths)
				path?.DrawGizmos();
		}
		#endregion
	}
}