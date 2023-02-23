using UnityEngine;
using UnityEngine.Serialization;
using System;

namespace LanternTrip {
	public class Player : Entity {
		[Serializable]
		public struct Movement {
			public enum State {
				Freefalling, Walking, Climbing
			}
			[NonSerialized] public State state;

			[NonSerialized] public Vector3 inputVelocity;
			[NonSerialized] public Vector3 walkingVelocity;

			[Range(0, 2)] public float maxAcceleration;
			[Range(0, 10)] public float maxWalkingSpeed;
		}

		#region Inspector members
		public Movement movement;
		#endregion

		#region Public interfaces
		#endregion

		#region Life cycle
		new void Start() {
			base.Start();

			// Initialize
			movement.inputVelocity = Vector3.zero;
		}

		new void FixedUpdate() {
			base.FixedUpdate();

			// Update movement state
			if(!standingPoint.HasValue) {
				// Not standing on any point, neither walking or climbing
				// Reset necessary infomation
				switch(movement.state) {
					case Movement.State.Walking:
						movement.walkingVelocity = Vector3.zero;
						break;
				}
				movement.state = Movement.State.Freefalling;
			}
			else {
				movement.state = Movement.State.Walking;
			}

			switch(movement.state) {
				case Movement.State.Walking:
					Vector3 targetVelocity = movement.inputVelocity;
					// Trim target velocity if exceeds maximum speed
					if(targetVelocity.magnitude > movement.maxWalkingSpeed)
						targetVelocity = targetVelocity.normalized * movement.maxWalkingSpeed;
					// Project onto the tangent plane of the current standing point
					Vector3 normal = standingPoint.Value.normal;
					targetVelocity = targetVelocity - Vector3.Dot(targetVelocity, normal) * normal;
					Vector3 deltaVelocity = targetVelocity - rigidbody.velocity;
					rigidbody.velocity += movement.maxAcceleration * deltaVelocity;
					break;
			}
		}
		#endregion
	}
}