using UnityEngine;
using System.Collections.Generic;

namespace LanternTrip {
	[RequireComponent(typeof(Trigger))]
	public class StickyZone : MonoBehaviour {
		Trigger trigger;
		List<Entity> entities = new List<Entity>();
		Vector3 lastVelocity;

		void Start() {
			trigger = GetComponent<Trigger>();
			trigger.onEnter.AddListener(c => {
				var entity = c.GetComponentInParent<Entity>();
				entities.Add(entity);
			});
			trigger.onExit.AddListener(c => {
				var entity = c.GetComponentInParent<Entity>();
				entities.Remove(entity);
			});
		}

		void FixedUpdate() {
			Vector3 velocity = trigger.entity.Rigidbody.velocity;
			Vector3 delta = velocity - lastVelocity;
			lastVelocity = velocity;
			foreach(var entity in entities)
				entity.Rigidbody.AddForce(delta, ForceMode.VelocityChange);
		}
	}
}