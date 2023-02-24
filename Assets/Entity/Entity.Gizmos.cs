#if UNITY_EDITOR
using UnityEngine;

namespace LanternTrip {
	public partial class Entity : MonoBehaviour {
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
	}
}
#endif