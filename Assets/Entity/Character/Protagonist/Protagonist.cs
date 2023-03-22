using UnityEngine;

namespace LanternTrip {
	public class Protagonist : Character {
		public void Shoot() {
			Debug.Log($"Shoot at {GameplayManager.instance.ChargeUpValue}");
		}
	}
}