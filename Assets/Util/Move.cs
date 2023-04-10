using UnityEngine;

namespace LanternTrip {
	public class Move : MonoBehaviour {
		public PathNode[] path = new PathNode[0];
		public Transform target;
		private Path path_test;
	
    public enum MoveMode
	{
		Alternate = Path.MoveMode.Alternate ,
		Normal    = Path.MoveMode.Normal ,
		Reverse   = Path.MoveMode.Reverse,
		Loop      = Path.MoveMode.Loop,
	};

	public MoveMode SelectMoveMode;
		
		void Start() {
			SelectMoveMode = MoveMode.Normal;

			path_test = new Path();
			path_test.Init(path, target);

		if(SelectMoveMode == MoveMode.Normal)
			{path_test.MoveAlongPath(Path.MoveMode.Normal);
		}

		else if(SelectMoveMode == MoveMode.Alternate)
			{path_test.MoveAlongPath(Path.MoveMode.Alternate);
		}

		else if(SelectMoveMode == MoveMode.Reverse)
			{path_test.MoveAlongPath(Path.MoveMode.Reverse);
		}

		else if(SelectMoveMode == MoveMode.Loop)
			{path_test.MoveAlongPath(Path.MoveMode.Loop);
		}

		void OnDrawGizmos() {
			path_test?.Display();
		}
	}
}
}
