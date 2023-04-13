using UnityEngine;

namespace LanternTrip {
	public class ToggleElevator : MonoBehaviour {
		public MoveAlongPath pathMover;

		public void Toggle() {
			pathMover.alternate = false;
			if(pathMover.Moving)
				pathMover.direction *= -1;
			else {
				pathMover.reversed = pathMover.Progress > .5f;
				pathMover.StartMoving();
			}
		}
	}
}