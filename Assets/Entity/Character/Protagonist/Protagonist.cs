using UnityEngine;

namespace LanternTrip {
	public class Protagonist : Character {
		protected override void UpdateMovementState() {
			base.UpdateMovementState();
			switch(movement.state) {
				case Movement.State.Walking:
					if(Idle && GameplayManager.instance.ChargeUpValue > 0) {
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
				Vector3 offset = GameplayManager.instance.shoot.Position.Value - transform.position;
				return offset.normalized;
			}
			else
				return base.CalculateExpectedDirection();
		}
	}
}