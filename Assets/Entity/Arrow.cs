using UnityEngine;

namespace LanternTrip {
	public class Arrow : MonoBehaviour {
		public Tinder.Type type;

		private void OnCollisionEnter(Collision collision) {
			Enemy enemy = collision.gameObject.GetComponent<Enemy>();
			enemy?.TakeArrowDamage(this);
		}
	}
}
