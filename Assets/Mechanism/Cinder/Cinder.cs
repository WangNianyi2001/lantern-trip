using UnityEngine;

namespace LanternTrip {
	public class Cinder : MonoBehaviour {
		[Min(0)] public int amount;

		public void Deliver() {
			GameplayManager.instance.Cinder += amount;
			gameObject.SetActive(false);
		}
	}
}