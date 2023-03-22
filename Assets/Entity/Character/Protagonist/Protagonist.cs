using UnityEngine;
using NaughtyAttributes;

namespace LanternTrip {
	public class Protagonist : Character {
		[Header("Shooting")]
		[Range(0, 4)] public float verticalSpeed;
		[MinMaxSlider(1, 20)] public Vector2 speedRange;

		protected override void UpdateMovementState() {
			base.UpdateMovementState();
			switch(movement.state) {
				case Movement.State.Walking:
					if(CanShoot && GameplayManager.instance.ChargeUpValue > 0) {
						movement.state = Movement.State.Shooting;
					}
					break;
				case Movement.State.Shooting:
					if(GameplayManager.instance.ChargeUpValue == 0)
						movement.state = Movement.State.Walking;
					break;
			}
		}

		protected override Vector3 CalculateExpectedDirection() {
			if(movement.state == Movement.State.Shooting) {
				Vector3? target = GameplayManager.instance.shoot.Position;
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
	}
}