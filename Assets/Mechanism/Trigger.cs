using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using UniRx;
using System;

namespace LanternTrip {
	public class Trigger : MonoBehaviour {
		public Entity entity;
		public PixelCrushers.TriggerEvent agent;
		public bool oneTime = false;
		[SerializeField][Tag] string tagMask;
		[SerializeField] UnityEvent<Collider> onEnter;
		[SerializeField] UnityEvent<Collider> onExit;

		[SerializeField] float delay = 0.0f;

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
				if(string.IsNullOrEmpty(value)) {
					value = "Player";
				}
				tagMask = value;
				agent.tagMask.m_tags = new string[] { tagMask };
			}
		}

		public void OnEnter(Collider collider) {
			Observable.Timer(TimeSpan.FromSeconds(delay)).Subscribe(_ =>
			{
                onEnter?.Invoke(collider);
            });
            
        }
		public void OnExit(Collider collider) => onExit?.Invoke(collider);

		private void MessageHandlerHandler(GameObject obj, string message) {
			if(!isActiveAndEnabled)
				return;
			Collider collider = obj?.GetComponent<Collider>();
			if(!collider)
				return;
			SendMessage(message, collider, SendMessageOptions.DontRequireReceiver);
		}

		protected void Start() {
			TagMask = tagMask;

			agent.onTriggerEnter.AddListener(obj => MessageHandlerHandler(obj, "OnEnter"));
			agent.onTriggerExit.AddListener(obj => MessageHandlerHandler(obj, "OnExit"));

			agent.onTriggerEnter.AddListener(_ => {
				if(oneTime)
					enabled = false;
			});
		}
	}
}
