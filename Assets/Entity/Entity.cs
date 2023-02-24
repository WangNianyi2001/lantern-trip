using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LanternTrip {
	[RequireComponent(typeof(Rigidbody))]
	public partial class Entity : MonoBehaviour {
		#region Core members
		protected new Rigidbody rigidbody;
		protected Dictionary<Collider, ContactPoint> contactingPoints;
		#endregion

		#region Private method
		void UpdateCollision(Collision collision) {
			ContactPoint lowest = collision.contacts.Aggregate((a, b) => a.point.y < b.point.y ? a : b);
			contactingPoints[collision.collider] = lowest;
		}
		#endregion

		#region Life cycle
		protected void Start() {
			// Get component references
			rigidbody = GetComponent<Rigidbody>();
			rigidbody.isKinematic = false;
			rigidbody.useGravity = true;

			// Initialize
			contactingPoints = new Dictionary<Collider, ContactPoint>();
		}

		protected void OnCollisionEnter(Collision collision) {
			UpdateCollision(collision);
		}

		protected void OnCollisionStay(Collision collision) {
			UpdateCollision(collision);
		}

		protected void OnCollisionExit(Collision collision) {
			contactingPoints.Remove(collision.collider);
		}
		#endregion
	}
}