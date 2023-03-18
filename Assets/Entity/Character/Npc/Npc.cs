using UnityEngine;
using NaughtyAttributes;

namespace LanternTrip {
	public class Npc : Character {
		[Expandable] public NpcProfile profile;
		public float followDistance = 2;

		private int hp;
		protected Transform followTarget = null;

		protected override Vector3 InputVelocity {
			get {
				if(followTarget == null)
					return base.InputVelocity;
				Vector3 delta = followTarget.position - transform.position;
				float length = delta.magnitude;
				length = Mathf.Max(0, length - followDistance);
				return delta.normalized * length;
			}
		}

		#region Public interfaces
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

		public void SetFollowTarget(Transform target) {
			followTarget = target;
		}
		#endregion

		protected new void Start() {
			base.Start();

			hp = profile.hp;
		}
	}
}
