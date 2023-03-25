using UnityEngine;

namespace LanternTrip {
	public static class MathUtil {
		public static Vector3 ProjectOnto(this Vector3 v, Vector3 planeNormal) {
			planeNormal = planeNormal.normalized;
			return v - Vector3.Dot(v, planeNormal) * planeNormal;
		}

		public static float Mod(float x, float divisor) {
			return x - Mathf.Floor(x / divisor) * divisor;
		}
	}
}
