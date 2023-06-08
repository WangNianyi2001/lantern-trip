using System;
using System.Collections.Generic;
using System.Linq;
using UniRx.Triggers;
using UniRx;
using UnityEngine;

namespace LanternTrip {
	[RequireComponent(typeof(Rigidbody))]
	public class StickyZone : MonoBehaviour {
		Rigidbody rb;
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
			ContactPoint tmp = new ContactPoint();
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
			rb = GetComponent<Rigidbody>();
			contactingPoints = new Dictionary<Collider, ContactPoint>();
			lastPosition = rb.position;
			lastRotation = rb.rotation;

			SubscribePhysicsOfParent();
		}



		// void FixedUpdate() {
		// 	#region Rotation
		// 	Quaternion newRotation = rb.rotation;
		// 	Quaternion deltaRotation = newRotation * Quaternion.Inverse(lastRotation);
		// 	foreach(var c in colliders) {
		// 		var rb = c.GetComponent<Rigidbody>();
		// 		if(rb == null)
		// 			continue;
		// 		var offset = rb.position - lastPosition;
		// 		rb.MovePosition(this.rb.position + deltaRotation * offset);
		// 		rb.MoveRotation(rb.rotation * deltaRotation);
		// 	}
		// 	lastRotation = newRotation;
		// 	#endregion
		// 	#region Position
		// 	Vector3 newPosition = rb.position;
		// 	Vector3 deltaPosition = newPosition - lastPosition;
		// 	foreach(var c in colliders) {
		// 		var rb = c.GetComponent<Rigidbody>();
		// 		if(rb == null)
		// 			continue;
		// 		rb.MovePosition(rb.position + deltaPosition);
		// 	}
		// 	lastPosition = newPosition;
		// 	#endregion
		// }
		
		
		void FixedUpdate() {
			#region Rotation
			Quaternion newRotation = rb.rotation;
			Quaternion deltaRotation = newRotation * Quaternion.Inverse(lastRotation);
			foreach(var c in colliders) {
				var rb = c.GetComponent<Rigidbody>();
				if(rb == null)
					continue;
				var offset = rb.position - lastPosition;
				rb.MovePosition(this.rb.position + deltaRotation * offset);
				rb.MoveRotation(rb.rotation * deltaRotation);
			
			}
			lastRotation = newRotation;
			#endregion
			#region Position
			Vector3 newPosition = rb.position;
			Vector3 deltaPosition = newPosition - lastPosition;
			foreach(var c in colliders) {
				var rb = c.GetComponent<Rigidbody>();
				if(rb == null)
					continue;
				// rb.MovePosition(rb.position + deltaPosition);
				rb.gameObject.transform.Translate(deltaPosition, Space.World);
			}
			lastPosition = newPosition;
			#endregion
		}
	}
}