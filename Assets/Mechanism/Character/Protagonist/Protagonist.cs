using UnityEngine;

namespace LanternTrip {
	public partial class Protagonist : Character {
		GameplayManager gameplay => GameplayManager.instance;

		protected override Vector3 CalculateWalkingVelocity() {
			return base.CalculateWalkingVelocity() * gameplay.speedBonusRate;
		}

		protected override void UpdateMovementState() {
			base.UpdateMovementState();
			switch(state) {
				case "Walking":
					if(CanShoot && gameplay.ChargeUpValue > 0) {
						state = "Shooting";
					}
					break;
				case "Shooting":
					if(gameplay.ChargeUpValue == 0)
						state = "Walking";
					break;
			}
		}

		protected override Vector3 CalculateExpectedDirection() {
			if(state == "Shooting") {
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

		public bool CanShoot => state == "Walking" || state == "Shooting";

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