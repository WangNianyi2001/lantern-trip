using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace LanternTrip {
	public class CharacterAnimationController {
		public readonly Character character;
		public Animator animator => character.animator;

		public bool Moving = false;
		public bool Jumping = false;
		public bool Freefalling = false;

		string[] booleans = new string[] { "Moving", "Jumping", "Freefalling" };

		public CharacterAnimationController(Character character) {
			this.character = character;
		}

		public void Update() {
			animator.transform.localPosition = Vector3.zero;
			animator.transform.localRotation = Quaternion.identity;

			foreach(string name in booleans) {
				var field = typeof(CharacterAnimationController).GetField(name);
				animator.SetBool(name, (bool)field.GetValue(this));
			}

			animator.SetBool("Moving", Moving);
			animator.SetBool("Jumping", Jumping);
			animator.SetBool("Freefalling", Freefalling);

			Debug.Log(string.Join(", ", booleans.Where(name => {
				var field = typeof(CharacterAnimationController).GetField(name);
				return (bool)field.GetValue(this);
			})));
		}
	}
}
