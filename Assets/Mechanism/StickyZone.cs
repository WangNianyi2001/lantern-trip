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
		private Matrix4x4 lastM;
	
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
			lastM = transform.worldToLocalMatrix;

			SubscribePhysicsOfParent();
		}



		void FixedUpdate()
		{
			ForwardKinematic();
		}

		// void ForwardKinematic()
		// {
		// 	Vector3 deltaPosition = transform.position - lastPosition;
		// 	lastPosition = transform.position;
		// 	Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(lastRotation);
		// 	lastRotation = transform.rotation;
		// 	foreach(var c in colliders) {
		// 		var rb = c.GetComponent<Rigidbody>();
		// 		if(rb == null)
		// 			continue;
		// 		rb.transform.position += deltaRotation * deltaPosition;
		// 		rb.transform.rotation *= deltaRotation;
		// 	}
		// }

		void ForwardKinematic()
		{
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
				Quaternion R1 =  rb.transform.rotation; 
				
				// position: P1 = P0 + Q0 * L0
				rb.transform.position = P0 + Q0 * L0;
				// rotation: Q1 = Q0 * R1   i.e.   R_1 = Rsz^-1 * Rrb = Rsz'^-1 * Rrb'
				rb.transform.rotation = Q0 * R1;
			}
			
			lastPosition = transform.position;
			lastRotation = transform.rotation;
		}
		
		
		
		// // Nianyi
		// void ForwardKinematic()
		// {
		// 	foreach (Collider target in colliders)
		// 	{
		// 		Matrix4x4 targetTransform = target.transform.worldToLocalMatrix;
		// 		// 假设目标物体静止，在新时刻预期的目标物体的 wTL 矩阵
		// 		Matrix4x4 expectedTransform = targetTransform * transform.worldToLocalMatrix.inverse * lastM;
		// 		// ``，预期的目标物体的世界坐标
		// 		Vector3 expectedWorldPosition = expectedTransform.inverse.GetPosition();
		// 		Vector3 actualWorldPosition = target.transform.position;
		// 		Vector3 deltaWorldPosition = expectedWorldPosition - actualWorldPosition;
		// 		target.transform.position -= deltaWorldPosition;
		// 		// ``，预期的目标物体的世界旋转
		// 		Quaternion expectedWorldRotation = expectedTransform.inverse.rotation;
		// 		Quaternion actualWorldRotation = target.transform.rotation;
		// 		Quaternion deltaWorldRotation = expectedWorldRotation * Quaternion.Inverse(actualWorldRotation);
		// 		target.transform.rotation *= Quaternion.Inverse(deltaWorldRotation);
		// 	}
		// 	lastM = transform.worldToLocalMatrix;
		// }
	}
}
