using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

namespace LanternTrip {
	public class Trigger : MonoBehaviour {
		public Entity entity;
		public Collider volume;
		public bool oneTime = false;
		[SerializeField][Tag] string tagMask;
		public UnityEvent<Collider> onEnter;
		public UnityEvent<Collider> onExit;

		private PixelCrushers.TriggerEvent agent;

		public new bool enabled {
			get => base.enabled;
			set {
				base.enabled = value;
				agent.enabled = value;

				if(entity) {
					foreach(var contact in entity.ContactingPoints)
						onExit.Invoke(contact.otherCollider);
				}
			}
		}
		public string TagMask {
			get => tagMask;
			set {
				tagMask = value;
				agent.tagMask.m_tags = new string[] { tagMask };
			}
		}

		protected void Start() {
			if(volume == null)
				volume = GetComponent<Collider>();
			if(volume == null)
				Debug.LogWarning("Volume for trigger is null");

			agent = volume.gameObject.AddComponent<PixelCrushers.TriggerEvent>();
			TagMask = tagMask;

			agent.onTriggerEnter.AddListener(obj => onEnter.Invoke(obj.GetComponent<Collider>()));
			agent.onTriggerExit.AddListener(obj => onExit.Invoke(obj.GetComponent<Collider>()));

			agent.onTriggerEnter.AddListener(_ => {
				if(oneTime)
					enabled = false;
			});
		}
	}
}
