using UnityEngine;

namespace LanternTrip {
	public static class MathUtil {
		public static Vector3 ProjectOnto(this Vector3 v, Vector3 planeNormal) { 
			return v - Vector3.Dot(v, planeNormal.normalized) * planeNormal;
		}
	}
}
