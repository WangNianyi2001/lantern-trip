using UnityEngine;
using System;

namespace LanternTrip {
	public class Player : Entity {
		public enum MovementState {
			Freefalling, Walking, Climbing
		}

		[Serializable]
		public struct MovementSettings {
			[Range(0, 2)]
			public float maxAcceleration;
			[Range(0, 10)]
			public float walkingSpeed;
		}

		#region Inspector members
		public MovementSettings movement;
		#endregion

		#region Core members
		Vector3 _desiredVelocity;
		#endregion

		#region Public interfaces
		public MovementState movementState {
			get {
				// TODO
				return MovementState.Walking;
			}
		}

		public Vector3 desiredVelocity {
			set {
				_desiredVelocity = value;
			}
		}
		#endregion

		#region Life cycle
		new void Start() {
			base.Start();

			// Initialize player status
			desiredVelocity = Vector3.zero;
		}

		void FixedUpdate() {
			switch(movementState) {
				case MovementState.Walking:
					Vector3 targetVelocity = _desiredVelocity;
					if(targetVelocity.magnitude > movement.walkingSpeed)
						targetVelocity = targetVelocity.normalized * movement.walkingSpeed;
					Vector3 deltaVelocity = targetVelocity - rigidbody.velocity;
					rigidbody.velocity += movement.maxAcceleration * deltaVelocity;
					break;
			}
		}
		#endregion
	}
}