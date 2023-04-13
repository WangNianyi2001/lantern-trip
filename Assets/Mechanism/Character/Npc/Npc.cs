using UnityEngine;
using NaughtyAttributes;

namespace LanternTrip {
	public class Npc : Character {
		[Expandable] public NpcProfile profile;
		public float followDistance = 2;

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
		public void SetFollowTarget(Transform target) {
			followTarget = target;
		}
		#endregion

		protected new void Start() {
			base.Start();

			Hp = profile.hp;
			Undead = profile.undead;
		}
	}
}
