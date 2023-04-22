using UnityEngine;
using UnityEngine.Events;

namespace LanternTrip {
	public class Shooter : MonoBehaviour {
		[Range(.1f, 5f)] public float totalTime;
		public GameObject projectilePrefab;
		public UnityEvent<GameObject> preShoot;

		public Vector3 CalculateInitialVelocityByTargetPosition(Vector3 position) {
			var up = -Physics.gravity.normalized;
			var g = Physics.gravity.magnitude;

			// Delta components
			Vector3 delta = position - transform.position;
			Vector3 dz = delta.ProjectOnto(up);
			float dy = Vector3.Dot(delta, up);

			// Z velocity
			Vector3 vz = dz / totalTime;

			// Y speed
			float compensatingVy = g * totalTime * .5f;
			float linearVy = dy / totalTime;
			float vy = compensatingVy + linearVy;

			// Combine to get final result
			return vz + up * vy;
		}

		public void Shoot(Vector3 position) {
			var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
			preShoot?.Invoke(projectile);
			Rigidbody rb = projectile.GetComponent<Rigidbody>();
			if(!rb)
				rb = projectile.AddComponent<Rigidbody>();
			rb.velocity = CalculateInitialVelocityByTargetPosition(position);
		}
	}
}