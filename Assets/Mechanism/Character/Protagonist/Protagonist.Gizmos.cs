using UnityEngine;

namespace LanternTrip {
	public partial class Protagonist {
		public new void OnDrawGizmos() {
			base.OnDrawGizmos();

			// Shooting
			if(state == "Shooting") {
				Gizmos.color = Color.cyan;
				Gizmos.DrawRay(new Ray(transform.position, CalculateExpectedDirection()));

				Vector3? target = GameplayManager.instance.shoot.TargetPosition;
				if(target.HasValue) {
					Gizmos.DrawSphere(target.Value, .1f);
				}
			}
		}
	}
}
