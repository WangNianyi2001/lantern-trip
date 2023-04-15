using UnityEngine;
using System;
using System.Collections.Generic;
using NaughtyAttributes;
using System.Linq;

namespace LanternTrip {
	public class MovingPath : ScriptableObject {
		public enum CoordinateMode { Local, World }
		public CoordinateMode coordinateMode;
		[ShowIf("coordinateMode", CoordinateMode.Local)] [AllowNesting] public Transform coordinateLocalBase;

		bool IsLocal => coordinateMode == CoordinateMode.Local && coordinateLocalBase != null;

		public bool useRotation;
		[Serializable]
		public struct Anchor {
			public Vector3 position;
			public Quaternion rotation;
		}
		public List<Anchor> anchors;

		public float MaxProgress => anchors.Count - 1;

		public float LengthByIndex(int i) {
			if(i < 0 || i >= anchors.Count - 1)
				throw new IndexOutOfRangeException();
			return Vector3.Distance(anchors[i].position, anchors[i + 1].position);
		}
		public float Length {
			get {
				float res = 0;
				for(int i = 0; i < anchors.Count - 1; ++i)
					res += LengthByIndex(i);
				return res;
			}
		}

		public float ProgressByDistance(float d) {
			if(d < 0)
				return 0;
			int i;
			for(i = 0; i < anchors.Count - 1; ++i) {
				float sd = LengthByIndex(i);
				if(sd >= d)
					return i + d / sd;
				d -= sd;
			}
			return anchors.Count - 1;
		}

		Vector3 _Position(int i) => IsLocal ? coordinateLocalBase.localToWorldMatrix.MultiplyPoint(anchors[i].position) : anchors[i].position;
		Quaternion _Rotation(int i) => IsLocal ? coordinateLocalBase.rotation * anchors[i].rotation : anchors[i].rotation;
		public Vector3 Position(float progress) {
			if(progress <= 0)
				return _Position(0);
			if(progress >= anchors.Count - 1)
				return _Position(anchors.Count - 1);

			int i = (int)Mathf.Floor(progress);
			float f = progress - i;

			Vector3 a = _Position(i), b = _Position(i + 1);
			return Vector3.Lerp(a, b, f);
		}
		public Quaternion Rotation(float progress) {
			if(progress <= 0)
				return _Rotation(0);
			if(progress >= anchors.Count - 1)
				return _Rotation(anchors.Count - 1);

			int i = (int)Mathf.Floor(progress);
			float f = progress - i;
			Quaternion a = _Rotation(i), b = _Rotation(i + 1);
			return Quaternion.Lerp(a, b, f);
		}

		public void Apply(Transform t, float progress) {
			t.position = Position(progress);
			if(useRotation)
				t.rotation = Rotation(progress);
		}

		[NonSerialized] public int currentSelectIndex = -1;
		public void DrawGizmos() {
			if(anchors == null)
				return;
			for(int i = 0; i < anchors.Count(); ++i) {
				var selected = i == currentSelectIndex;
				var q = _Rotation(i);
				var p = _Position(i);

				var color = selected ? Color.red : Color.yellow;
				var radius = selected ? .1f : .05f;
				Gizmos.color = color;
				Gizmos.DrawSphere(p, radius);

				if(useRotation) {
					float l = .2f * (selected ? 1.2f : 1);
					Gizmos.color = Color.red;
					Gizmos.DrawLine(p, p + q * Vector3.right * l);
					Gizmos.color = Color.green;
					Gizmos.DrawLine(p, p + q * Vector3.up * l);
					Gizmos.color = Color.blue;
					Gizmos.DrawLine(p, p + q * Vector3.forward * l);
				}

				if(i > 0) {
					Gizmos.color = Color.white;
					var prev = _Position(i - 1);
					Gizmos.DrawLine(p, prev);
				}
			}
		}
	}
}
