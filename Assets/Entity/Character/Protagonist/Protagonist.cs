using UnityEngine;
using NaughtyAttributes;

namespace LanternTrip {
	public class Protagonist : Character {
		[Header("Shooting")]
		[Range(0, 4)] public float verticalSpeed;
		[MinMaxSlider(1, 20)] public Vector2 speedRange;

		protected override void UpdateMovementState() {
			base.UpdateMovementState();
			switch(state) {
				case State.Walking:
					if(CanShoot && GameplayManager.instance.ChargeUpValue > 0) {
						state = State.Shooting;
					}
					break;
				case State.Shooting:
					if(GameplayManager.instance.ChargeUpValue == 0)
						state = State.Walking;
					break;
			}
		}

		protected override Vector3 CalculateExpectedDirection() {
			if(state == State.Shooting) {
				Vector3? target = GameplayManager.instance.shoot.TargetPosition;
				if(!target.HasValue)
					return transform.forward;
				Vector3 offset = target.Value - transform.position;
				return offset.normalized;
			}
			else
				return base.CalculateExpectedDirection();
		}

		public override void Die() {
			base.Die();

			Debug.Log("Died");
		}

		public void Shoot() {
			GameplayManager gameplay = GameplayManager.instance;
			if(gameplay.Burn(1)) {
				gameplay.shoot.MakeShoot();
			}
		}
	}
}