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

		public bool controlRotation;
		[Serializable]
		public struct Anchor {
			public Vector3 position;
			public Vector3 rotation;
		}
		public List<Anchor> anchors;

		[NonSerialized] public int currentSelectIndex = -1;
		public void DrawGizmos() {
			IList<Anchor> anchors = this.anchors;
			if(coordinateMode == CoordinateMode.Local && coordinateLocalBase != null) {
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
