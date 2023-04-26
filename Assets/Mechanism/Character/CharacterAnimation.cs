using UnityEngine;

namespace LanternTrip {
	public class CharacterAnimation : MonoBehaviour {
		protected Character character;

		void Start() {
			character = GetComponentInParent<Character>();
		}

		public void OnFootstep() {
			character.PlayStepSound();
		}
	}
}