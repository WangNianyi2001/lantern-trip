using UnityEngine;

namespace LanternTrip {
	[RequireComponent(typeof(Entity))]
	public class Arrow : MonoBehaviour {
		public Entity entity;
		public Tinder.Type type;

		private void OnCollisionEnter(Collision collision) {
			Enemy enemy = collision.gameObject.GetComponent<Enemy>();
			enemy?.TakeArrowDamage(this);

			Destroy(gameObject);
		}
	}
}
