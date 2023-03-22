using UnityEngine;

namespace LanternTrip {
	public class CharacterAnimationController {
		public readonly Character character;
		public Animator animator => character.animator;

		public bool Moving = false;
		public bool Jumping = false;
		public bool Freefalling = false;
		public bool Dead = false;
		public bool HoldingBow = false;
		public float ChargingUpValue = 0;

		public CharacterAnimationController(Character character) {
			this.character = character;
		}

		public void Update() {
			if(!animator)
				return;

			animator.SetBool("Moving", Moving);
			animator.SetBool("Jumping", Jumping);
			animator.SetBool("Freefalling", Freefalling);
			animator.SetBool("Dead", Dead);
			animator.SetBool("Holding Bow", HoldingBow);
			animator.SetFloat("Charging Up", ChargingUpValue);
		}
	}
}
