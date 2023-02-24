using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LanternTrip {
	[RequireComponent(typeof(Rigidbody))]
	public class Entity : MonoBehaviour {
		#region Core members
		protected new Rigidbody rigidbody;
		Dictionary<Collider, ContactPoint> contactingPoints;
		#endregion

		#region Private method
		void UpdateCollision(Collision collision) {
			ContactPoint lowest = collision.contacts.Aggregate((a, b) => a.point.y < b.point.y ? a : b);
			contactingPoints[collision.collider] = lowest;
		}
		#endregion

		#region Public interface
		public ContactPoint? standingPoint {
			get {
				if(contactingPoints.Count == 0)
					return null;
				return contactingPoints.Values.Aggregate((a, b) => a.point.y < b.point.y ? a : b);
			}
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

		protected void OnDrawGizmos() {
			if(Application.isPlaying) {
				// Standing point
				if(standingPoint.HasValue) {
					Gizmos.color = Color.red;
					Gizmos.DrawSphere(standingPoint.Value.point, .1f);

					// Normal
					Gizmos.color = Color.green;
					Gizmos.DrawRay(standingPoint.Value.point, standingPoint.Value.normal);
				}

				// Actual velocity
				Gizmos.color = Color.red;
				Gizmos.DrawRay(rigidbody.position, rigidbody.velocity);
			}
		}
		#endregion
	}
}