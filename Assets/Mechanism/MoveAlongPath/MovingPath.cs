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

		public bool controlRotation;
		[Serializable]
		public struct Anchor {
			public Vector3 position;
			public Vector3 rotation;
		}
		public List<Anchor> anchors;

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

		public KeyValuePair<int, float> ProgressByDistance(float d) {
			if(d < 0)
				return new KeyValuePair<int, float>(0, 0);
			int i;
			for(i = 0; i < anchors.Count - 1; ++i) {
				float sd = LengthByIndex(i);
				if(sd >= d)
					return new KeyValuePair<int, float>(i, d / sd);
				d -= sd;
			}
			return new KeyValuePair<int, float>(anchors.Count - 2, 1);
		}

		public Vector3 Position(KeyValuePair<int, float> progress) {
			Vector3 a = anchors[progress.Key].position, b = anchors[progress.Key + 1].position;
			Vector3 res = Vector3.Lerp(a, b, progress.Value);
			if(IsLocal)
				res = coordinateLocalBase.localToWorldMatrix.MultiplyPoint(res);
			return res;
		}
		public Quaternion Rotation(KeyValuePair<int, float> progress) {
			Quaternion a = Quaternion.Euler(anchors[progress.Key].rotation), b = Quaternion.Euler(anchors[progress.Key + 1].rotation);
			Quaternion res = Quaternion.Lerp(a, b, progress.Value);
			if(IsLocal)
				res = coordinateLocalBase.rotation * res;
			return res;
		}

		[NonSerialized] public int currentSelectIndex = -1;
		public void DrawGizmos() {
			IList<Anchor> anchors = this.anchors;
			if(IsLocal) {
				anchors = anchors.Select(a => {
					a.position = coordinateLocalBase.localToWorldMatrix.MultiplyPoint(a.position);
					Quaternion q = Quaternion.Euler(a.rotation);
					q = coordinateLocalBase.rotation * q;
					a.rotation = q.eulerAngles;
					return a;
				}).ToList();
			}
			for(int i = 0; i < anchors.Count(); ++i) {
				var anchor = anchors[i];
				var selected = i == currentSelectIndex;

				var color = selected ? Color.red : Color.yellow;
				var radius = selected ? .1f : .05f;
				Gizmos.color = color;
				Gizmos.DrawSphere(anchor.position, radius);

				if(controlRotation) {
					var q = Quaternion.Euler(anchor.rotation);
					var p = anchor.position;
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
					var prev = anchors[i - 1];
					Gizmos.DrawLine(anchor.position, prev.position);
				}
			}
		}
	}
}
