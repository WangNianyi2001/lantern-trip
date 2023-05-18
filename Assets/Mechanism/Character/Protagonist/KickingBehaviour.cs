using UnityEngine;

namespace LanternTrip {
	public class KickingBehaviour : StateMachineBehaviour {
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			GameplayManager.instance?.protagonist?.EndKicking();
		}
	}
}