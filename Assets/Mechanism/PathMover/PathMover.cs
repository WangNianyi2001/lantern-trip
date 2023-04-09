using UnityEngine;
using NaughtyAttributes;

namespace LanternTrip {
	public class PathMover : MonoBehaviour {
		[Expandable] [Instance] public MovingPath path;

		void OnDrawGizmos() {
			if(path) {
				path.DrawGizmos();
			}
		}
	}
}