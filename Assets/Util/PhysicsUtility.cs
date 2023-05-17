using UnityEngine;

namespace LanternTrip {
	public static class PhysicsUtility {

		public struct CircularSector {
			public Vector3 center;
			public Vector3 normal;
			public float radius;
			public Vector3 startingDirection;
			public float spanAngle;

			Quaternion worldToLocalRotation => Quaternion.LookRotation(startingDirection, normal);
			Matrix4x4 worldToLocalTransform => Matrix4x4.Rotate(worldToLocalRotation) * Matrix4x4.Translate(-center);
			Matrix4x4 localToWorldTransform => worldToLocalTransform.inverse;

			/// <summary>Converts world rectangular coordinate into local cylindrical coordinate.</summary>
			/// <returns>(azimuth, radius, height)</returns>
			/// <remarks>Azimuth is calculated starting from `startingDirection` by the left-handed direction.</remarks>
			public Vector3 WorldToLocal(Vector3 xyz) {
				Vector3 localXYZ = worldToLocalTransform.MultiplyPoint(xyz);
				Vector3 localARH;
				localARH.z = localXYZ.y;                            // height
				localXYZ.y = 0;
				localARH.y = localXYZ.magnitude;                    // radius
				localARH.x = Mathf.Atan2(localXYZ.x, localXYZ.z);   // azimuth
				return localARH;
			}
			public Vector3 LocalToWorld(Vector3 local) {
				Vector3 localXYZ;
				localXYZ.z = Mathf.Cos(local.x) * local.y;
				localXYZ.x = Mathf.Sin(local.x) * local.y;
				localXYZ.y = local.z;
				return localToWorldTransform.MultiplyPoint(localXYZ);
			}

			public bool SweepCast(Vector3 worldPosition, float height) {
				Vector3 localARH = WorldToLocal(worldPosition);
				if(!MathUtil.InRange(localARH.z, 0, height))
					return false;
				if(localARH.y < 0) {
					localARH.y = -localARH.y;
					localARH.x = -localARH.x;
				}
				if(localARH.y > radius)
					return false;
				localARH.x = MathUtil.Mod(localARH.x, Mathf.PI * 2);
				if(localARH.x > spanAngle)
					return false;
				return true;
			}
		}

		public struct RightTriangle {
			public static float Hypotenuse(float legA, float legB) => Mathf.Sqrt(legA * legA + legB * legB);
			public static float Leg(float hypotenuse, float otherLeg) => Mathf.Sqrt(hypotenuse * hypotenuse - otherLeg * otherLeg);
		}

		/// <summary>Sweep cast along the normal of a circular sector.</summary>
		public static RaycastHit? CircularSectorSweepCast(CircularSector circularSector, float distance) {
			ref CircularSector g = ref circularSector;
			RaycastHit hit;
			float t = -g.radius;
			Vector3 local = Vector3.zero;
			while(t < distance) {
				local.z = t;
				Vector3 world = g.LocalToWorld(local);
				Physics.SphereCast(world, g.radius, g.normal, out hit, distance - t, ~0, QueryTriggerInteraction.Ignore);
				if(!hit.collider)
					// No more candidate hits
					break;
				if(g.SweepCast(hit.point, distance))
					return hit;
				// Update local z
				t = Mathf.Max(t + .1f, g.WorldToLocal(hit.point).z);
			}
			return null;
		}

		public static void DrawCircularSectorSweepGizmos(CircularSector circularSector, float distance) {
			ref CircularSector g = ref circularSector;
			const int azimuthResolution = 10;
			const int heightResolution = 3;
			for(int azimuthI = 0; azimuthI <= azimuthResolution; ++azimuthI) {
				for(int heightI = 0; heightI <= heightResolution; ++heightI) {
					float azimuthA = g.spanAngle * azimuthI / azimuthResolution;
					float azimuthB = g.spanAngle * (azimuthI + 1) / azimuthResolution;
					bool drawAzimuth = azimuthI != azimuthResolution;

					float heightA = distance * heightI / heightResolution;
					float heightB = distance * (heightI + 1) / heightResolution;
					bool drawHeight = heightI != heightResolution;

					Vector3 topLeft = g.LocalToWorld(new Vector3(azimuthA, g.radius, heightA));
					Vector3 topRight = g.LocalToWorld(new Vector3(azimuthB, g.radius, heightA));
					Vector3 bottomLeft = g.LocalToWorld(new Vector3(azimuthA, g.radius, heightB));
					Vector3 bottomRight = g.LocalToWorld(new Vector3(azimuthB, g.radius, heightB));

					if(drawAzimuth)
						Gizmos.DrawLine(topLeft, topRight);
					if(drawHeight)
						Gizmos.DrawLine(topLeft, bottomLeft);
					if(drawAzimuth && drawHeight) {
						Gizmos.DrawLine(topRight, bottomRight);
						Gizmos.DrawLine(bottomLeft, bottomRight);
					}
				}
			}
		}

		public static int FrictionModeToInt(PhysicMaterialCombine mode) {
			int i = (int)mode;
			return (i & 2 >> 1) | (i & 1 << 1);
		}

		public static PhysicMaterialCombine CombineFrictionMode(PhysicMaterialCombine a, PhysicMaterialCombine b) {
			int ia = FrictionModeToInt(a), ib = FrictionModeToInt(b);
			int i = Mathf.Max(ia, ib);
			i = (i & 2 >> 1) | (i & 1 << 1);
			return (PhysicMaterialCombine)i;
		}

		public static float CalculateFrictionCoefficient(this ContactPoint contact, bool dynamic = false) {
			// Average < Min < Multiply < Max
			PhysicMaterial
				a = contact.thisCollider.material,
				b = contact.otherCollider.material;
			PhysicMaterialCombine mode = CombineFrictionMode(a.frictionCombine, b.frictionCombine);
			float fa, fb;
			if(dynamic) {
				fa = a.dynamicFriction;
				fb = b.dynamicFriction;
			}
			else {
				fa = a.staticFriction;
				fb = b.staticFriction;
			}
			switch(mode) {
				case PhysicMaterialCombine.Average:
					return (fa + fb) * .5f;
				case PhysicMaterialCombine.Multiply:
					return fa * fb;
				case PhysicMaterialCombine.Minimum:
					return Mathf.Min(fa, fb);
				case PhysicMaterialCombine.Maximum:
					return Mathf.Max(fa, fb);
			}
			return 1;
		}
	}
}