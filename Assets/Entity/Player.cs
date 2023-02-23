using UnityEngine;
using System;

namespace LanternTrip {
	public class Player : Entity {
		[Serializable]
		public struct Movement {
			public enum State {
				Freefalling, Walking, Climbing
			}
			[NonSerialized] public State state;
			[Range(0, 2)]
			public float maxAcceleration;
			[Range(0, 10)]
			public float walkingSpeed;
		}

		#region Inspector members
		public Movement movement;
		#endregion

		#region Core members
		Vector3 _desiredVelocity;
		#endregion

		#region Public interfaces
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

		new void FixedUpdate() {
			base.FixedUpdate();

			// Update movement state
			if(standingPoint == null)
				movement.state = Movement.State.Freefalling;
			else {
				movement.state = Movement.State.Walking;
			}

			switch(movement.state) {
				case Movement.State.Walking:
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