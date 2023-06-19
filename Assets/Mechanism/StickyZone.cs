using UnityEngine;
using UniRx.Triggers;
using UniRx;
using System.Collections.Generic;
using System.Linq;

namespace LanternTrip {
	[RequireComponent(typeof(Rigidbody))]
	public class StickyZone : MonoBehaviour {
		Vector3 lastPosition;
		Quaternion lastRotation;
		
		HashSet<Collider> colliders = new HashSet<Collider>(16);
		Dictionary<Collider, ContactPoint> contactingPoints = new Dictionary<Collider, ContactPoint>();

		#region Internal functions
		bool IsNotInterested(Transform t) => !(transform.IsChildOf(t) || t.IsChildOf(transform));

		void UpdateCollision(Collision collision) {
			ContactPoint lowest = collision.contacts.Aggregate((a, b) => a.point.y < b.point.y ? a : b);
			contactingPoints[collision.collider] = lowest;
		}

		void SubscribePhysicsOfParent() {
			transform.parent.OnCollisionEnterAsObservable().Subscribe(collision => {
				UpdateCollision(collision);
			});
			transform.parent.OnCollisionStayAsObservable().Subscribe(collision => {
				UpdateCollision(collision);
			});
		}
		#endregion

		#region Life cycle
		protected void OnTriggerEnter(Collider c) {
			if(IsNotInterested(c.transform) && c.GetComponent<Entity>() != null)
				colliders.Add(c);
		}

		protected void OnTriggerExit(Collider c) {
			colliders.Remove(c);
			var entity = c.GetComponent<Entity>();
			if(entity != null)
				entity.RemoveContactPoints(contactingPoints);
		}

		protected void Start() {
			contactingPoints = new Dictionary<Collider, ContactPoint>();
			lastPosition = transform.position;
			lastRotation = transform.rotation;

			SubscribePhysicsOfParent();
		}

		protected void FixedUpdate() {
			Vector3 P0 = transform.position;
			Quaternion Q0 = transform.rotation * Quaternion.Inverse(lastRotation); // delta rotation 

			foreach(var c in colliders) {
				var rb = c.GetComponent<Rigidbody>();
				if(rb == null)
					continue;

				// parent: StickyZone (this)
				// child: rb

				Vector3 L0 = rb.transform.position - lastPosition; // offset: StickyZone to rb

				// local rotation of rb respect to StickyZone
				Quaternion R1 = rb.transform.rotation;

				// position: P1 = P0 + Q0 * L0
				rb.transform.position = P0 + Q0 * L0;
				// rotation: Q1 = Q0 * R1   i.e.   R_1 = Rsz^-1 * Rrb = Rsz'^-1 * Rrb'
				rb.transform.rotation = Q0 * R1;
			}

			lastPosition = transform.position;
			lastRotation = transform.rotation;
		}
		#endregion
	}
}
