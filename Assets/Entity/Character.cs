using UnityEngine;

namespace LanternTrip {
	public partial class Character : Entity {
		public struct Movement {
			public enum State {
				Passive,        // Character status is controlled externally.
				Walking,        // Character is walking on ground.
				Freefalling,    // Character is falling and doesn't receive player input.
				Jumping,        // Character has just jumped.
				Landing,        // Character has just landed on ground.
			}
			public State state;

			public Vector3 inputVelocity;
			public Vector3 walkingVelocity;
		}

		#region Inspector members
		public CharacterMovementSettings movementSettings;
		#endregion

		#region Core members
		public Movement movement;
		float scanOffset = .5f;
		float scanHeight = .3f;
		float radius = .6f;
		PhysicsUtility.CircularSector staircaseSector => new PhysicsUtility.CircularSector {
			center = rigidbody.position + rigidbody.transform.up * scanOffset,
			normal = -rigidbody.transform.up,
			radius = radius,
			startingDirection = rigidbody.transform.right,
			spanAngle = Mathf.PI
		};
		#endregion

		#region Core methods
		void UpdateMovementState() {
			switch(movement.state) {
				case Movement.State.Walking:
					// If not standing on any point, freefall
					if(!standingPoint.HasValue) {
						movement.walkingVelocity = Vector3.zero;
						movement.state = Movement.State.Freefalling;
					}
					break;
				case Movement.State.Freefalling:
					// If landed, land
					if(standingPoint.HasValue) {
						float fallingSpeed = Vector3.Dot(rigidbody.velocity, Physics.gravity);
						if(fallingSpeed < 0)
							break;
						movement.state = Movement.State.Landing;
					}
					break;
				case Movement.State.Jumping:
					// TODO: Animation etc.
					movement.state = Movement.State.Freefalling;
					break;
				case Movement.State.Landing:
					// TODO: Animation etc.
					movement.state = Movement.State.Walking;
					break;
			}
		}

		float SlopeByNormal(Vector3 normal) {
			float cos = Vector3.Dot(-Physics.gravity.normalized, normal.normalized);
			return Mathf.Acos(cos);
		}

		Vector3 CalculateWalkingVelocity() {
			Vector3 targetVelocity = movement.inputVelocity;
			float speed = targetVelocity.magnitude;
			speed *= movementSettings.walking.speed;
			targetVelocity = targetVelocity.normalized * speed;
			// Project onto the tangent plane of the current standing point
			Vector3 normal = standingPoint.Value.normal;
			float slopeAngle = SlopeByNormal(normal) / Mathf.PI * 180;
			if(slopeAngle > movementSettings.walking.maxSlopeAngle)
				return Vector3.zero;
			targetVelocity = targetVelocity - Vector3.Dot(targetVelocity, normal.normalized) * normal;
			return targetVelocity;
		}
		Vector3 CalculateWalkingForce(Vector3 targetVelocity) {
			Vector3 deltaVelocity = targetVelocity - rigidbody.velocity;
			float magnitude = deltaVelocity.magnitude;
			magnitude *= movementSettings.walking.accelerationGain;
			magnitude = Mathf.Min(magnitude, movementSettings.walking.maxAcceleration);
			return deltaVelocity.normalized * magnitude;
		}
		Vector3 CalculateStaircaseImpulse() {
			RaycastHit? hit = PhysicsUtility.CircularSectorSweepCast(staircaseSector, scanHeight);
			if(!hit.HasValue)
				return Vector3.zero;
			Debug.Log($"Staircase: {hit.Value.collider}");
			return transform.up * 2;
		}
		Quaternion CalculateRotation() {
			Vector3 forward = rigidbody.velocity;
			Vector3 upward = -Physics.gravity.normalized;
			forward -= Vector3.Dot(forward, upward) * upward;
			if(forward.magnitude < .5f)
				return rigidbody.rotation;
			return Quaternion.LookRotation(forward, upward);
		}
		#endregion

		#region Public interfaces
		public ContactPoint? standingPoint {
			get {
				ContactPoint? result = null;
				foreach(ContactPoint point in contactingPoints.Values) {
					float slopeAngle = SlopeByNormal(point.normal) / Mathf.PI * 180;
					if(slopeAngle > movementSettings.walking.maxSlopeAngle)
						continue;
					if(!result.HasValue || point.point.y < result.Value.point.y)
						result = point;
				}
				return result;
			}
		}

		public void Jump() {
			Vector3 impulse = -Physics.gravity.normalized * movementSettings.jumping.speed / rigidbody.mass;
			rigidbody.AddForce(impulse, ForceMode.Impulse);
			movement.state = Movement.State.Jumping;
		}
		#endregion

		#region Life cycle
		protected new void Start() {
			base.Start();

			// Initialize
			movement.state = Movement.State.Freefalling;
			movement.inputVelocity = Vector3.zero;
		}

		protected void FixedUpdate() {
			UpdateMovementState();
			switch(movement.state) {
				case Movement.State.Walking:
					movement.walkingVelocity = CalculateWalkingVelocity();
					Vector3 force = CalculateWalkingForce(movement.walkingVelocity);
					rigidbody.AddForce(force);
					rigidbody.AddForce(CalculateStaircaseImpulse(), ForceMode.Impulse);
					rigidbody.rotation = CalculateRotation();
					break;
			}
		}
		#endregion
	}
}