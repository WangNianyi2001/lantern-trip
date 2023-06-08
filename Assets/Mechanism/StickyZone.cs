using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx.Triggers;
using UniRx;
using UnityEngine;

namespace LanternTrip {
	[RequireComponent(typeof(Rigidbody))]
	public class StickyZone : MonoBehaviour {
		HashSet<Collider> colliders = new HashSet<Collider>(16);
		Vector3 lastPosition;
		Quaternion lastRotation;
		
		protected Dictionary<Collider, ContactPoint> contactingPoints = new Dictionary<Collider, ContactPoint>();

		bool IsInterested(Transform t) => !(transform.IsChildOf(t) || t.IsChildOf(transform));
		
		
		#region Private Function
		void UpdateCollision(Collision collision) {
			// reverse the normal of contact point
			// foreach (var contactPoint in collision.contacts)
			// {
			// 	contactPoint.otherCollider;
			// }

			ContactPoint lowest = collision.contacts.Aggregate((a, b) => a.point.y < b.point.y ? a : b);
			contactingPoints[collision.collider] = lowest;
		}

		void SubscribePhysicsOfParent()
		{
			transform.parent.OnCollisionEnterAsObservable().Subscribe(collision =>
			{
				UpdateCollision(collision);
			});
			transform.parent.OnCollisionStayAsObservable().Subscribe(collision =>
			{
				UpdateCollision(collision);
			});
			
			// remove collision only if the collider is in trigger
			// transform.parent.OnCollisionExitAsObservable().Subscribe(collision =>
			// {
			// 	contactingPoints.Remove(collision.collider);
			// });
		}
		
		#endregion

		#region Physics
		void OnTriggerEnter(Collider c) {
			if(IsInterested(c.transform))
				colliders.Add(c);
			

		}

		private void OnTriggerStay(Collider c)
		{
			// add a enduring contact point to the entity
			var entity = c.GetComponent<Entity>();
			if (entity != null)
			{
				entity.AddContactPoints(contactingPoints);

			}
		}

		void OnTriggerExit(Collider c) {
			if(IsInterested(c.transform))
				colliders.Remove(c);
			var entity = c.GetComponent<Entity>();
			if (entity != null)
			{
				entity.RemoveContactPoints(contactingPoints);

			}
		}
		#endregion
		

		void Start() {
			contactingPoints = new Dictionary<Collider, ContactPoint>();
			lastPosition = transform.position;
			lastRotation = transform.rotation;

			SubscribePhysicsOfParent();
		}



		void FixedUpdate() {
			Vector3 deltaPosition = transform.position - lastPosition;
			lastPosition = transform.position;
			Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(lastRotation);
			lastRotation = transform.rotation;
			foreach(var c in colliders) {
				var rb = c.GetComponent<Rigidbody>();
				if(rb == null)
					continue;
				rb.transform.position += deltaRotation * deltaPosition;
				rb.transform.rotation *= deltaRotation;
			}
		}
	}
}