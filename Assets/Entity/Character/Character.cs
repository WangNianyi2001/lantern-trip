using UnityEngine;
using System.Collections;

namespace LanternTrip {
	public partial class Character : Entity {
		public struct Movement {
			public enum State {
				Passive,        // Character status is controlled externally.
				Walking,        // Character is walking on ground.
				Freefalling,    // Character is falling and doesn't receive player input.
				Jumping,        // Character has just jumped.
				Landing,        // Character has just landed on ground.
				Dead,
				Shooting,
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
		public CharacterAnimationController animationController;
		#endregion

		#region Core methods
		protected virtual Vector3 InputVelocity => movement.inputVelocity;

		protected virtual void UpdateMovementState() {
			switch(movement.state) {
				case Movement.State.Walking:
					// If not standing on any point, freefall
					if(!standingPoint.HasValue) {
						movement.walkingVelocity = Vector3.zero;
						movement.state = Movement.State.Freefalling;
					}
					break;
				case Movement.State.Freefalling:
					animationController.Freefalling = true;
					animationController.Jumping = false;

					// If landed, land
					if(standingPoint.HasValue) {
						float fallingSpeed = Vector3.Dot(rigidbody.velocity, Physics.gravity);
						if(fallingSpeed < 0)
							break;
						movement.state = Movement.State.Landing;
					}
					break;
				case Movement.State.Jumping:
					animationController.Jumping = true;
					break;
				case Movement.State.Landing:
					animationController.Freefalling = false;
					movement.state = Movement.State.Walking;
					break;
				case Movement.State.Dead:
					animationController.Dead = true;
					break;
			}
			animationController.Moving =
				(movement.state == Movement.State.Walking)
				&& (movement.walkingVelocity.magnitude > .1f);
			animationController.Freefalling =
				movement.state == Movement.State.Freefalling;
			rigidbody.isKinematic = movement.state == Movement.State.Passive;
		}

		protected virtual float SlopeByNormal(Vector3 normal) {
			float cos = Vector3.Dot(-Physics.gravity.normalized, normal.normalized);
			return Mathf.Acos(cos);
		}

		protected virtual Vector3 CalculateWalkingVelocity() {
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
			Vector3 jumpingImpulse = transform.up * speed / rigidbody.mass;
			rigidbody.AddForce(jumpingImpulse, ForceMode.Impulse);
			movement.state = Movement.State.Freefalling;

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

		public bool Idle => movement.state == Movement.State.Walking && InputVelocity.magnitude < .1f;

		public void Jump() {
			if(movement.state != Movement.State.Walking)
				return;
			movement.state = Movement.State.Jumping;
			StartCoroutine(JumpCoroutine());
		}
		#endregion

		#region Life cycle
		protected new void Start() {
			base.Start();

			animationController = new CharacterAnimationController(this);

			// Initialize
			movement.state = Movement.State.Walking;
			movement.inputVelocity = Vector3.zero;
		}

		protected void FixedUpdate() {
			UpdateMovementState();
			switch(movement.state) {
				case Movement.State.Walking:
					movement.walkingVelocity = CalculateWalkingVelocity();
					Vector3 walkingForce = CalculateWalkingForce(movement.walkingVelocity);
					rigidbody.AddForce(walkingForce);
					if(movementSettings.jumping.autoJump) {
						if(CalculateShouldAutoJump())
							Jump();
					}
					break;
			}
			switch(movement.state) {
				case Movement.State.Passive:
				case Movement.State.Dead:
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