using UnityEngine;
using System;

namespace LanternTrip {
	public class Player : Entity {
		[Serializable]
		public struct MovementSettings {
			[Serializable]
			public struct Walking {
				[Range(0, 100)] public float accelerationGain;
				[Range(0, 100)] public float maxAcceleration;
				[Range(0, 100)] public float maxSpeed;
				[Range(0, 90)] public float maxSlopeAngle;
			}
			public Walking walking;

			[Serializable]
			public struct Jumping {
				[Range(0, 5)] public float speed;
			}
			public Jumping jumping;
		}

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
		public MovementSettings movementSettings;
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
						// Reset necessary infomation
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

		Vector3 CalculateWalkingVelocity() {
			Vector3 targetVelocity = movement.inputVelocity;
			float speed = targetVelocity.magnitude;
			speed = Mathf.Min(speed, movementSettings.walking.maxSpeed);
			targetVelocity = targetVelocity.normalized * speed;
			// Project onto the tangent plane of the current standing point
			Vector3 normal = standingPoint.Value.normal;
			float sine = Vector3.Dot(targetVelocity.normalized, normal.normalized);
			float slopeAngle = -Mathf.Asin(sine) / Mathf.PI * 180;
			if(slopeAngle > movementSettings.walking.maxSlopeAngle)
				return Vector3.zero;
			targetVelocity = targetVelocity - sine * speed * normal;
			return targetVelocity;
		}
		Vector3 CalculateWalkingForce(Vector3 targetVelocity) {
			Vector3 deltaVelocity = targetVelocity - rigidbody.velocity;
			float magnitude = deltaVelocity.magnitude;
			magnitude *= movementSettings.walking.accelerationGain;
			magnitude = Mathf.Min(magnitude, movementSettings.walking.maxAcceleration);
			return deltaVelocity.normalized * magnitude;
		}
		#endregion

		#region Public interfaces
		public void Jump() {
			Vector3 impulse = -Physics.gravity.normalized * movementSettings.jumping.speed / rigidbody.mass;
			rigidbody.AddForce(impulse, ForceMode.Impulse);
			movement.state = Movement.State.Jumping;
		}
		#endregion

		#region Life cycle
		new void Start() {
			base.Start();

			// Initialize
			movement.state = Movement.State.Freefalling;
			movement.inputVelocity = Vector3.zero;
		}

		void FixedUpdate() {
			UpdateMovementState();
			switch(movement.state) {
				case Movement.State.Walking:
					movement.walkingVelocity = CalculateWalkingVelocity();
					Vector3 force = CalculateWalkingForce(movement.walkingVelocity);
					rigidbody.AddForce(force);
					break;
			}
		}

		new void OnDrawGizmos() {
			base.OnDrawGizmos();

			if(Application.isPlaying) {
				// Input velocity
				if(movement.state == Movement.State.Walking) {
					Gizmos.color = Color.blue;
					Gizmos.DrawRay(rigidbody.position, movement.walkingVelocity);
				}
			}
		}
		#endregion
	}
}