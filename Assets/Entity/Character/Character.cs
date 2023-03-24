using UnityEngine;
using System.Collections;
using System;

namespace LanternTrip {
	public partial class Character : Entity {
		public enum State {
			Passive,        // Character status is controlled externally.
			Walking,        // Character is walking on ground.
			Freefalling,    // Character is falling and doesn't receive player input.
			Jumping,        // Character has just jumped.
			Landing,        // Character has just landed on ground.
			Dead,
			Shooting,
		}

		#region Inspector members
		public CharacterMovementSettings movementSettings;
		public Animator animator;
		#endregion

		#region Core members
		public CharacterAnimationController animationController;
		[NonSerialized] public State state;
		[NonSerialized] public Vector3 inputVelocity;
		[NonSerialized] public Vector3 walkingVelocity;
		#endregion

		#region Core methods
		protected virtual Vector3 InputVelocity => inputVelocity;

		protected virtual void UpdateMovementState() {
			switch(state) {
				case State.Walking:
					// If not standing on any point, freefall
					if(!standingPoint.HasValue) {
						walkingVelocity = Vector3.zero;
						state = State.Freefalling;
					}
					break;
				case State.Freefalling:
					animationController.Freefalling = true;
					animationController.Jumping = false;

					// If landed, land
					if(standingPoint.HasValue) {
						float fallingSpeed = Vector3.Dot(rigidbody.velocity, Physics.gravity);
						if(fallingSpeed < 0)
							break;
						state = State.Landing;
					}
					break;
				case State.Jumping:
					animationController.Jumping = true;
					break;
				case State.Landing:
					animationController.Freefalling = false;
					state = State.Walking;
					break;
				case State.Dead:
					animationController.Dead = true;
					break;
			}
			animationController.Moving =
				(state == State.Walking)
				&& (walkingVelocity.magnitude > .1f);
			animationController.Freefalling =
				state == State.Freefalling;
			rigidbody.isKinematic = state == State.Passive;
		}

		protected virtual float SlopeByNormal(Vector3 normal) {
			float cos = Vector3.Dot(-Physics.gravity.normalized, normal.normalized);
			return Mathf.Acos(cos);
		}

		protected virtual Vector3 CalculateWalkingVelocity() {
			if(state == State.Shooting)
				return Vector3.zero;
			Vector3 targetVelocity = InputVelocity;
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
		protected virtual Vector3 CalculateWalkingForce(Vector3 targetVelocity) {
			Vector3 deltaVelocity = targetVelocity - rigidbody.velocity;
			deltaVelocity = deltaVelocity.ProjectOnto(Physics.gravity);
			float magnitude = deltaVelocity.magnitude;
			magnitude *= movementSettings.walking.accelerationGain;
			magnitude = Mathf.Min(magnitude, movementSettings.walking.maxAcceleration);
			return deltaVelocity.normalized * magnitude;
		}
		protected virtual Vector3 CalculateExpectedDirection() {
			Vector3 velocity = rigidbody.velocity.ProjectOnto(Physics.gravity);
			if(velocity.magnitude < .1f)
				return transform.forward;
			return velocity.normalized;
		}
		protected virtual Vector3 CalculateZenithTorque() {
			Vector3 up = Physics.gravity.normalized;

			Vector3 expectedDirection = CalculateExpectedDirection();
			Vector3 actualDirection = transform.forward.ProjectOnto(up).normalized;
			float cosine = Vector3.Dot(expectedDirection, actualDirection);
			cosine = Mathf.Clamp(cosine, -1f, 1f);	// Prevent overflow
			float deltaZenith = Mathf.Acos(cosine);
			// Left or right
			float direction = Mathf.Sign(Vector3.Dot(Vector3.Cross(actualDirection, expectedDirection), up));

			Vector3 expectedAngularVelocity = up * deltaZenith * direction;
			float attenuation = (expectedAngularVelocity - rigidbody.angularVelocity).magnitude;
			attenuation = Mathf.Pow(attenuation, .5f);
			return attenuation * movementSettings.walking.torqueGain * expectedAngularVelocity;
		}

		public PhysicsUtility.CircularSector autoJumpSector => new PhysicsUtility.CircularSector {
			center = transform.position + transform.up * movementSettings.jumping.autoJumpHeight,
			normal = Physics.gravity.normalized,
			radius = movementSettings.jumping.autoJumpRadius,
			spanAngle = Mathf.PI,
			startingDirection = transform.right,
		};
		protected virtual bool CalculateShouldAutoJump() {
			float castDistance = movementSettings.jumping.autoJumpHeight - movementSettings.jumping.autoJumpBottomSlitHeight;
			RaycastHit? hit = PhysicsUtility.CircularSectorSweepCast(autoJumpSector, castDistance);
			return hit.HasValue;
		}

		IEnumerator JumpCoroutine() {
			yield return new WaitForSeconds(movementSettings.jumping.preWaitingTime);
			float gravity = Physics.gravity.magnitude;
			float targetHeight = movementSettings.jumping.height;
			// v^2 = 2gh
			float speed = Mathf.Sqrt(2 * gravity * targetHeight);
			Vector3 jumpingImpulse = transform.up * speed * rigidbody.mass;
			rigidbody.AddForce(jumpingImpulse, ForceMode.Impulse);
			state = State.Freefalling;

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

		public bool CanMove {
			get {
				switch(state) {
					default: return true;
					case State.Passive:
					case State.Dead:
						return false;
				}
			}
		}

		public bool CanShoot {
			get {
				bool idle = state == State.Walking && InputVelocity.magnitude < .1f;
				bool charging = state == State.Shooting;
				return idle || charging;
			}
		}

		public void Jump() {
			if(state != State.Walking)
				return;
			state = State.Jumping;
			StartCoroutine(JumpCoroutine());
		}

		public virtual void Die() {
			state = State.Dead;
		}
		#endregion

		#region Life cycle
		protected new void Start() {
			base.Start();

			animationController = new CharacterAnimationController(this);

			// Initialize
			state = State.Walking;
			inputVelocity = Vector3.zero;
		}

		protected void FixedUpdate() {
			UpdateMovementState();
			if(CanMove) {
				walkingVelocity = CalculateWalkingVelocity();
				Vector3 walkingForce = CalculateWalkingForce(walkingVelocity);
				rigidbody.AddForce(walkingForce);
			}
			if(state == State.Walking) {
				if(movementSettings.jumping.autoJump) {
					if(CalculateShouldAutoJump())
						Jump();
				}
			}
			switch(state) {
				case State.Passive:
				case State.Dead:
					break;
				default:
					Vector3 zenithTorque = CalculateZenithTorque();
					rigidbody.AddTorque(zenithTorque);
					break;
			}

			animationController.Update();
		}
		#endregion
	}
}