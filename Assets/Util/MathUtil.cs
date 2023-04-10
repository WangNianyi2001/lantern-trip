using UnityEngine;

namespace LanternTrip {
	public static class MathUtil {
		public static Vector3 ProjectOnto(this Vector3 v, Vector3 planeNormal) {
			planeNormal = planeNormal.normalized;
			return v - Vector3.Dot(v, planeNormal) * planeNormal;
		}

		public static Vector3 MagnitudeTo(this Vector3 v, float magnitude) {
			return v.normalized * magnitude;
		}

		public static float GroundedMagnitude(this Vector3 v) {
			Vector3 grounded = v;
			grounded.y = 0;
			return grounded.magnitude;
		}

		public static float Zenith(this Vector3 v) {
			return Mathf.Atan2(v.y, v.GroundedMagnitude());
		}

		public static float Azimuth(this Vector3 v) {
			return Mathf.Atan2(v.x, v.z);
		}

		public static float Mod(float x, float divisor) {
			return x - Mathf.Floor(x / divisor) * divisor;
		}
	}
}
