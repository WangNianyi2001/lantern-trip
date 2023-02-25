#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace LanternTrip {
	public partial class Character : Entity {
		protected new void OnDrawGizmos() {
			base.OnDrawGizmos();

			if(Application.isPlaying) {
				Collider collider = rigidbody.GetComponent<Collider>();

				// Movement state
				if(movement.state != Movement.State.Freefalling) {
					Handles.color = Color.white;
					Vector3 position = (collider.bounds.max + collider.bounds.min) / 2;
					position.y = collider.bounds.max.y;
					Handles.Label(position, typeof(Movement.State).GetEnumName(movement.state));
				}

				// Input velocity
				if(movement.state == Movement.State.Walking) {
					Gizmos.color = Color.blue;
					Gizmos.DrawRay(rigidbody.position, movement.walkingVelocity);
				}

				// Staircase detection
				if(movement.state == Movement.State.Walking) {
					Gizmos.color = Color.white;
					PhysicsUtility.DrawCircularSectorSweepGizmos(staircaseSector, scanHeight);
				}
			}
		}
	}
}
#endif