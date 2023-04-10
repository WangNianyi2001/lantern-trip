using UnityEngine;
using System.Collections;

namespace LanternTrip {
	[RequireComponent(typeof(Entity))]
	public class Arrow : MonoBehaviour {
		public Entity entity;
		public ParticleSystem particle;
		Tinder tinder;

		bool firstCollision = false;

		IEnumerator FirstCollisionCoroutine() {
			yield return new WaitForSeconds(5);
			Destroy(gameObject);
		}

		public Tinder Tinder {
			get => tinder;
			set {
				tinder = value;
#pragma warning disable
				particle.startColor = tinder?.mainColor ?? Color.white;
#pragma warning restore
			}
		}

		private void OnCollisionEnter(Collision collision) {
			if(firstCollision)
				return;
			firstCollision = true;

			particle.Stop();

			Entity entity = collision.gameObject.GetComponent<Entity>();
			entity?.Shot(this);

			StartCoroutine(FirstCollisionCoroutine());
		}
	}
}
