using UnityEngine;
using NaughtyAttributes;

namespace LanternTrip {
	public class Npc : Character {
		[Expandable] public NpcProfile profile;

		private int hp;

		public int Hp {
			get => hp;
			set {
				hp = value;
				if(hp <= 0) {
					hp = 0;
					Die();
				}
			}
		}

		public void TakeDamage(int amount) => hp -= amount;

		public virtual void Die() {
			Debug.Log($"NPC {profile.name} died");
		}

		protected new void Start() {
			base.Start();

			hp = profile.hp;
		}
	}
}
