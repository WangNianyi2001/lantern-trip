using UnityEngine;
using System;
using System.Collections.Generic;

namespace LanternTrip {
	public class MovingPath : ScriptableObject {
		public enum CoordinateMode { Local, World }
		public CoordinateMode coordinateMode;
		public Transform coordinateLocalBase;

		public bool controlRotation;
		[Serializable]
		public struct Anchor {
			public Vector3 position;
			public Vector3 eulerAngles;
		}
		public List<Anchor> anchors;
	}
}
