using UnityEngine;

namespace LanternTrip {
	public static class MathUtil {
		public static Vector3 ProjectOntoNormal(this Vector3 v, Vector3 normal) {
			normal = normal.normalized;
			return v - Vector3.Dot(v, normal) * normal;
		}

		public static Vector3 ProjectOntoVector(this Vector3 v, Vector3 target) {
			return Vector3.Dot(v, target.normalized) * v;
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

		public static bool InRange(float x, float min, float max) => x >= min && x <= max;

		public static float Lerp(this Vector2 range, float t) {
			return Mathf.Lerp(range.x, range.y, t);
		}
	}
}
