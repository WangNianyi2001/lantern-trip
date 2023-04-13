#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace LanternTrip {
	public partial class Character : Entity {
		protected new void OnDrawGizmos() {
			base.OnDrawGizmos();

			if(Application.isPlaying) {
				// Movement state
				Handles.color = Color.white;
				Collider collider = rigidbody.GetComponent<Collider>();
				Vector3 position = (collider.bounds.max + collider.bounds.min) / 2;
				position.y = collider.bounds.max.y;
				Handles.Label(position, state);

				// Standing point
				if(standingPoint.HasValue) {
					var point = standingPoint.Value;

					Gizmos.color = Color.red;
					Gizmos.DrawSphere(point.point, .1f);

					// Normal
					Gizmos.color = Color.green;
					Gizmos.DrawRay(point.point, point.normal);
				}

				// Input velocity
				if(state == "Walking") {
					Gizmos.color = Color.blue;
					Gizmos.DrawRay(rigidbody.position, walkingVelocity);
				}

				// Zenith torque
				Gizmos.color = Color.green;
				Gizmos.DrawRay(rigidbody.position, -CalculateZenithTorque());

				// Auto jumping sector
				if(movementSettings.jumping.autoJump) {
					Gizmos.color = Color.white;
					float distance = movementSettings.jumping.autoJumpHeight - movementSettings.jumping.autoJumpBottomSlitHeight;
					PhysicsUtility.DrawCircularSectorSweepGizmos(autoJumpSector, distance);
				}
			}
		}
	}
}
#endif