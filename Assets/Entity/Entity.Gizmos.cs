#if UNITY_EDITOR
using UnityEngine;

namespace LanternTrip {
	public partial class Entity : MonoBehaviour {
		protected void OnDrawGizmos() {
			if(Application.isPlaying) {
				// Contacting point
				foreach(ContactPoint point in contactingPoints.Values) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere(point.point, .05f);

					// Normal
					Gizmos.color = Color.green;
					Gizmos.DrawRay(point.point, point.normal);
				}

				// Velocity
				if(rigidbody) {
					Gizmos.color = Color.red;
					Gizmos.DrawRay(rigidbody.position, rigidbody.velocity);
				}
			}
		}
	}
}
#endif