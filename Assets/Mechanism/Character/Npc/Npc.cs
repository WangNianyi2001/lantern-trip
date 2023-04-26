using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;

namespace LanternTrip {
	public class Npc : Character {
		#region Serialized fields
		[Expandable] public NpcProfile profile;
		public float followDistance = 2;
		[ConversationPopup] public List<string> conversations;
		public bool repeatLastConversation;
		public int conversationIndex = 0;
		#endregion

		#region Internal fields
		protected Transform followTarget = null;
		#endregion

		#region Internal functions
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
		#endregion

		#region Public interfaces
		public void SetFollowTarget(Transform target) {
			followTarget = target;
		}

		public void TriggerNextConversation() {
			Debug.Log($"Conversation {conversationIndex}/{conversations.Count}");
			if(conversationIndex < 0 || conversationIndex >= conversations.Count)
				return;
			GameplayManager.instance.StartConversation(conversations[conversationIndex]);
			++conversationIndex;
			if(repeatLastConversation) {
				if(conversationIndex >= conversations.Count)
					conversationIndex = conversations.Count - 1;
			}
		}
		#endregion

		#region Life cycle
		protected new void Start() {
			base.Start();

			Hp = profile.hp;
			Undead = profile.undead;
		}
		#endregion
	}
}
