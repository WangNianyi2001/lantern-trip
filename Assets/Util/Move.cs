using UnityEngine;

namespace LanternTrip {
	public class Move : MonoBehaviour {
		public PathNode[] path = new PathNode[0];
		public Transform target;
		private Path path_test;
		
		void Start() {
			path_test = new Path();
			path_test.Init(path, target);

			path_test.MoveAlongPath(Path.MoveMode.Alternate);
		}
		void OnDrawGizmos() {
			path_test?.Display();
		}
	}
}

