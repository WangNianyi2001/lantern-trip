using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace LanternTrip {
	public class Shooter : MonoBehaviour {
		[Range(.1f, 5f)] public float totalTime;
		public GameObject projectilePrefab;
		public UnityEvent<GameObject> preShoot;
		public Vector3 projectileGravity2 = new Vector3(0.0f, -3.0f, 0.0f);

		public Vector3 CalculateInitialVelocityByTargetPosition(Vector3 position) {
			var up = -Physics.gravity.normalized;
			var g = Physics.gravity.magnitude;

			// Delta components
			Vector3 delta = position - transform.position;
			Vector3 dz = delta.ProjectOntoNormal(up);
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
			projectile.SetGravity(projectileGravity2);
		}

		
		public Vector3 CalculateInitialVelocityByShootingRange(float shootingRange)
		{
			Vector3 res = Vector3.zero;
			
			GameplayManager gameplay = GameplayManager.instance;
			Camera cam = gameplay.camera.camera;
			res = cam.transform.forward * (0.5f + 0.5f) * 100.0f;

			return res;
		}
		
		/// <summary>
		/// Calculate Initial Velocity By Shooting Range (remap to (0,1))
		/// </summary>
		/// <param name="shootingRange"></param>
		public void Shoot(float shootingRange)
		{
			var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
			preShoot?.Invoke(projectile);
			Rigidbody rb = projectile.GetComponent<Rigidbody>();
			if(!rb)
				rb = projectile.AddComponent<Rigidbody>();
			rb.velocity = CalculateInitialVelocityByShootingRange(shootingRange);
			projectile.SetGravity(projectileGravity2);
		}
	}
}