using UnityEngine;
using System;
using System.Collections.Generic;
using NaughtyAttributes;

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
	}
}
