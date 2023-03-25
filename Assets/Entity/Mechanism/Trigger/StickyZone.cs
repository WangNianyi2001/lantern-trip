using UnityEngine;
using System.Collections.Generic;

namespace LanternTrip {
	[RequireComponent(typeof(Trigger))]
	public class StickyZone : MonoBehaviour {
		Trigger trigger;
		List<Entity> entities = new List<Entity>();
		Vector3 lastPosition;

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
			Vector3 position = trigger.entity.Rigidbody.position;
			Vector3 delta = position - lastPosition;
			lastPosition = position;
			foreach(var entity in entities)
				entity.Rigidbody.MovePosition(entity.Rigidbody.position + delta);
		}
	}
}