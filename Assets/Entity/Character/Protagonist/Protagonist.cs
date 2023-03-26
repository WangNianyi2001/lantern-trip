using UnityEngine;
using NaughtyAttributes;

namespace LanternTrip {
	public partial class Protagonist : Character {
		[Header("Shooting")]
		[Range(0, 4)] public float verticalSpeed;
		[MinMaxSlider(1, 20)] public Vector2 speedRange;
		[Range(0, 1)] public float shootingAngleRate;

		GameplayManager gameplay => GameplayManager.instance;

		protected override Vector3 CalculateWalkingVelocity() {
			return base.CalculateWalkingVelocity() * gameplay.speedBonusRate;
		}

		protected override void UpdateMovementState() {
			base.UpdateMovementState();
			switch(state) {
				case State.Walking:
					if(CanShoot && gameplay.ChargeUpValue > 0) {
						state = State.Shooting;
					}
					break;
				case State.Shooting:
					if(gameplay.ChargeUpValue == 0)
						state = State.Walking;
					break;
			}
		}

		protected override Vector3 CalculateExpectedDirection() {
			if(state == State.Shooting) {
				Vector3? target = gameplay.shoot.TargetPosition;
				if(!target.HasValue)
					return transform.forward;
				Vector3 offset = target.Value - transform.position;
				offset = offset.ProjectOnto(transform.up);
				return offset.normalized;
			}
			else
				return base.CalculateExpectedDirection();
		}

		public void Shoot() {
			if(gameplay.Burn(1)) {
				gameplay.shoot.MakeShoot();
			}
		}

		protected new void FixedUpdate() {
			base.FixedUpdate();
			//
		}
	}
}