using System;
using UnityEngine;

namespace LanternTrip {
	public partial class Character : Entity {
		#region Properties

		[NonSerialized] public uint ID = 0;
		private static uint id = 0;

		public int HP = 100;

		public Tinder.Type element = Tinder.Type.Red;

		#endregion

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
		public Animator animator;
		#endregion

		#region Core members
		public Movement movement;
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
			Vector3 normal = standingPoint.HasValue ? standingPoint.Value.normal : -Physics.gravity;
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
		Vector3 CalculateZenithTorque() {
			Vector3 up = Physics.gravity;
			Vector3 actualAngularVelocity = rigidbody.angularVelocity;
			Vector3 expected = rigidbody.velocity.ProjectOnto(up);
			if(expected.magnitude < .1f)
				return -actualAngularVelocity;

			Vector3 expectedDirection = expected.normalized;
			Vector3 actualDirection = transform.forward.ProjectOnto(up).normalized;
			float deltaZenith = Mathf.Acos(Vector3.Dot(expectedDirection, actualDirection));
			// Left or right
			float direction = Mathf.Sign(Vector3.Dot(Vector3.Cross(actualDirection, expectedDirection), up));

			Vector3 expectedAngularVelocity = up * deltaZenith * direction;
			return expectedAngularVelocity - actualAngularVelocity;
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
			if(movement.state != Movement.State.Walking)
				return;
			Vector3 impulse = -Physics.gravity.normalized * movementSettings.jumping.speed / rigidbody.mass;
			rigidbody.AddForce(impulse, ForceMode.Impulse);
			movement.state = Movement.State.Jumping;
		}
		#endregion

		#region Life cycle
		protected new void Start() {
			base.Start();

			ID = id++;

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
					break;
			}
			Vector3 zenithTorque = CalculateZenithTorque();
			rigidbody.AddTorque(zenithTorque);

			// Animator
			animator.transform.localPosition = Vector3.zero;
			animator.transform.localRotation = Quaternion.identity;
			animator?.SetBool("Walking", movement.walkingVelocity.magnitude > .1f);
		}
		#endregion
	}
}