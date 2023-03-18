using UnityEngine;

namespace LanternTrip {
	public class CharacterAnimationController {
		public readonly Character character;
		public Animator animator => character.animator;

		public bool Moving = false;
		public bool Jumping = false;
		public bool Freefalling = false;

		public CharacterAnimationController(Character character) {
			this.character = character;
		}

		public void Update() {
			if(!animator)
				return;

			animator.transform.localPosition = Vector3.zero;
			animator.transform.localRotation = Quaternion.identity;

			animator.SetBool("Moving", Moving);
			animator.SetBool("Jumping", Jumping);
			animator.SetBool("Freefalling", Freefalling);
		}
	}
}
