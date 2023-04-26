using UnityEngine;
using System;
using System.Collections;
using System.Linq;

namespace LanternTrip {
	public partial class Character : Entity {
		#region Serialized members
		public CharacterMovementSettings movementSettings;
		public Animator animator;
		[SerializeField] protected AudioSource stepAudio;
		public AudioClip[] footstepClips;
		#endregion

		#region Core members
		public CharacterAnimationController animationController;
		[NonSerialized] public string state;
		[NonSerialized] public Vector3 inputVelocity;
		[NonSerialized] public Vector3 walkingVelocity;
		#endregion

		#region Core methods
		protected virtual Vector3 InputVelocity => inputVelocity;

		protected virtual void UpdateMovementState() {
			switch(state) {
				case "Walking":
					// If not standing on any point, freefall
					if(!standingPoint.HasValue) {
						walkingVelocity = Vector3.zero;
						state = "Freefalling";
					}
					break;
				case "Freefalling":
					animationController.Freefalling = true;
					animationController.Jumping = false;

					// If landed, land
					if(standingPoint.HasValue) {
						// 如果不注释，会导致角色在斜坡上滑落时无法判定落地
						// float fallingSpeed = Vector3.Dot(rigidbody.velocity, Physics.gravity);
						// if(fallingSpeed < 0)
						// 	break;
						state = "Landing";
					}
					break;
				case "Jumping":
					animationController.Jumping = true;
					break;
				case "Landing":
					animationController.Freefalling = false;
					state = "Walking";
					break;
				case "Dead":
					animationController.Dead = true;
					break;
			}
			animationController.Moving =
				(state == "Walking")
				&& (walkingVelocity.magnitude > .1f);
			animationController.Freefalling =
				state == "Freefalling";
		}

		protected virtual float SlopeByNormal(Vector3 normal) {
			float cos = Vector3.Dot(-Physics.gravity.normalized, normal.normalized);
			return Mathf.Acos(cos);
		}

		protected virtual Vector3 CalculateWalkingVelocity() {
			if(state == "Shooting")
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
			state = "Freefalling";

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
			if(state != "Walking")
				return;
			state = "Jumping";
			StartCoroutine(JumpCoroutine());
		}

		public void PlayStepSound() {
			if(!stepAudio)
				return;
			if(footstepClips == null || footstepClips.Count() == 0)
				return;
			int i = Mathf.FloorToInt(UnityEngine.Random.value * footstepClips.Count());
			stepAudio.clip = footstepClips[i];
			stepAudio.Play();
		}
		#endregion

		#region Life cycle
		protected new void Start() {
			base.Start();

			animationController = new CharacterAnimationController(this);

			// Initialize
			state = "Walking";
			inputVelocity = Vector3.zero;

			onDie.AddListener(() => state = "Dead");
		}

		protected new void Update() {
			base.Update();
			UpdateMovementState();
			if(state != "Dead") {
				walkingVelocity = CalculateWalkingVelocity();
				Vector3 walkingForce = CalculateWalkingForce(walkingVelocity);
				rigidbody.AddForce(walkingForce);
			}
			if(state == "Walking") {
				if(movementSettings.jumping.autoJump) {
					if(CalculateShouldAutoJump())
						Jump();
				}
			}
			if(state != "Dead") {
				Vector3 zenithTorque = CalculateZenithTorque();
				rigidbody.AddTorque(zenithTorque);
			}

			animationController.Update();
		}
		#endregion
	}
}