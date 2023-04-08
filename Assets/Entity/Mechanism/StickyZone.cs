using UnityEngine;
using System.Collections.Generic;

namespace LanternTrip {
	[RequireComponent(typeof(Rigidbody))]
	public class StickyZone : MonoBehaviour {
		Rigidbody rb;
		HashSet<Collider> colliders = new HashSet<Collider>(16);
		Vector3 lastPosition;

		bool IsInterested(Transform t) => !(transform.IsChildOf(t) || t.IsChildOf(transform));

		void OnTriggerEnter(Collider c) {
			if(IsInterested(c.transform))
				colliders.Add(c);
		}
		void OnTriggerExit(Collider c) {
			if(IsInterested(c.transform))
				colliders.Remove(c);
		}

		void Start() => rb = GetComponent<Rigidbody>();

		void FixedUpdate() {
			Vector3 position = rb.position;
			Vector3 delta = position - lastPosition;
			lastPosition = position;
			foreach(var c in colliders) {
				var rb = c.GetComponent<Rigidbody>();
				if(rb == null)
				continue;
				rb.MovePosition(rb.position + delta);
			}
		}
	}
}