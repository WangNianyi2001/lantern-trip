using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace LanternTrip {
	public class Shooter : MonoBehaviour {
		[Range(.1f, 5f)] public float totalTime;
		public GameObject projectilePrefab;
		public UnityEvent<GameObject> preShoot;

		public float arrowSpeed = 30.0f;
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
			// rb.velocity = CalculateInitialVelocityByTargetPosition(position);
			rb.velocity = CalculateInitialVelocityByDistance(position);
			projectile.SetGravity(projectileGravity2);
		}

		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="position">Target Position</param>
		/// <returns></returns>
		public Vector3 CalculateInitialVelocityByDistance(Vector3 position)
		{
			// Delta components
			Vector3 delta = position - transform.position;
			var dir = delta.normalized;

			// Combine to get final result

			var res = dir * arrowSpeed;
			var factor = math.sqrt( math.max(1.0f, math.min(25.0f, delta.magnitude)));
			res *= factor;
			return res;
		}
		
	}
}