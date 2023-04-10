using UnityEngine;

namespace LanternTrip {
	public class ShootRelease : StateMachineBehaviour {
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			GameplayManager.instance.protagonist.Shoot();
		}
	}
}