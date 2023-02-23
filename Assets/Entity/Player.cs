using UnityEngine;
using System;

namespace LanternTrip {
	public class Player : Entity {
		[Serializable]
		public struct Movement {
			public enum State {
				Passive,        // Character status is controlled externally.
				Walking,        // Character is walking on ground.
				Freefalling,    // Character is falling and doesn't receive player input.
				Jumping,        // Character has just jumped.
				Landing,        // Character has just landed on ground.
			}
			[NonSerialized] public State state;

			[NonSerialized] public Vector3 inputVelocity;
			[NonSerialized] public Vector3 walkingVelocity;

			[Range(0, 100)] public float accelerationGain;
			[Range(0, 500)] public float maxAcceleration;
			[Range(0, 100)] public float maxWalkingSpeed;
			[Range(0, 90)] public float maxWalkingSlopeAngle;
		}

		#region Inspector members
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
		#endregion

		#region Public interfaces
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
					Vector3 targetVelocity = movement.inputVelocity;
					float speed = targetVelocity.magnitude;
					if(speed > movement.maxWalkingSpeed)
						speed = movement.maxWalkingSpeed;
					targetVelocity = targetVelocity.normalized * speed;
					// Project onto the tangent plane of the current standing point
					Vector3 normal = standingPoint.Value.normal;
					float sine = Vector3.Dot(targetVelocity, normal);
					float slopeAngle = Mathf.Asin(sine) / Mathf.PI * 180;
					if(slopeAngle > movement.maxWalkingSlopeAngle)
						break;
					movement.walkingVelocity = targetVelocity = targetVelocity - sine * normal;
					Vector3 force = targetVelocity - rigidbody.velocity;
					// Ease out
					if(speed < 1)
						force *= Mathf.Max(.2f, Mathf.Pow(speed, .5f));
					force *= movement.accelerationGain;
					if(force.magnitude > movement.maxAcceleration)
						force = force.normalized * movement.maxAcceleration;
					rigidbody.AddForce(force);
					break;
			}
		}

		new void OnDrawGizmos() {
			base.OnDrawGizmos();

			if(Application.isPlaying) {
				// Input velocity
				if(movement.state == Movement.State.Walking) {
					Vector3 position = rigidbody.position;
					Gizmos.color = Color.blue;
					Gizmos.DrawLine(position, position + movement.walkingVelocity);
				}
			}
		}
		#endregion
	}
}