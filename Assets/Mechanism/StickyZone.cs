using System;
using System.Collections.Generic;
using UnityEngine;

namespace LanternTrip {
	[RequireComponent(typeof(Rigidbody))]
	public class StickyZone : MonoBehaviour {
		Rigidbody rb;
		HashSet<Collider> colliders = new HashSet<Collider>(16);
		Vector3 lastPosition;
		Quaternion lastRotation;

		bool IsInterested(Transform t) => !(transform.IsChildOf(t) || t.IsChildOf(transform));

		void OnTriggerEnter(Collider c) {
			if(IsInterested(c.transform))
				colliders.Add(c);
		}
		void OnTriggerExit(Collider c) {
			if(IsInterested(c.transform))
				colliders.Remove(c);
		}

		private void OnCollisionEnter(Collision c)
		{
			if(IsInterested(c.transform))
				colliders.Add(c.collider);
		}

		private void OnCollisionExit(Collision c)
		{
			if(IsInterested(c.transform))
				colliders.Remove(c.collider);
		}

		void Start() {
			rb = GetComponent<Rigidbody>();
			lastPosition = rb.position;
			lastRotation = rb.rotation;
		}

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
				rb.MovePosition(rb.position + deltaPosition);
			}
			lastPosition = newPosition;
			#endregion
		}
	}
}