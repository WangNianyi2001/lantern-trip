using UnityEngine;
using System.Collections;

namespace LanternTrip {
	[RequireComponent(typeof(Entity))]
	public class Arrow : MonoBehaviour {
		public Entity entity;
		public Tinder.Type type;

		bool firstCollision = false;

		IEnumerator FirstCollisionCoroutine() {
			yield return new WaitForSeconds(5);
			Destroy(gameObject);
		}

		private void OnCollisionEnter(Collision collision) {
			if(firstCollision)
				return;
			firstCollision = true;

			Enemy enemy = collision.gameObject.GetComponent<Enemy>();
			enemy?.TakeArrowDamage(this);

			StartCoroutine(FirstCollisionCoroutine());
		}
	}
}
