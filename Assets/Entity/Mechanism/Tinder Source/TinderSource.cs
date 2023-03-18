using UnityEngine;
using UnityEngine.Events;
using DSUsable = PixelCrushers.DialogueSystem.Wrappers.Usable;

namespace LanternTrip {
	[RequireComponent(typeof(Entity))]
	public class TinderSource : MonoBehaviour {
		/// <summary>Temporary solution for getting current tinder.</summary>
		/// <remarks>Might be changed in the future.</remarks>
		public static TinderSource current = null;

		public Trigger trigger;
		public Tinder type;
		public UnityEvent onApproach;
		public UnityEvent onLeave;
		public UnityEvent onDeliver;

		private DSUsable usable;

		public void OnApproach() {
			if(!isActiveAndEnabled)
				return;

			onApproach.Invoke();
			current = this;
		}
		public void OnLeave() {
			if(!isActiveAndEnabled)
				return;

			onLeave.Invoke();
			current = null;
		}
		public void Deliver() {
			if(!isActiveAndEnabled)
				return;
			if(current != this)
				return;

			GameplayManager.instance.LoadTinder(type);
			onDeliver.Invoke();

			current = null;
			gameObject.SetActive(false);
			// Alternatively:
			// Destroy(gameObject);
		}

		protected void Start() {
			if(trigger == null)
				trigger = GetComponentInChildren<Trigger>();
			if(trigger == null)
				Debug.LogWarning("Trigger for tinder source is null");

			trigger.TagMask = "Player";

			trigger.onEnter.AddListener(_ => OnApproach());
			trigger.onExit.AddListener(_ => OnLeave());

			usable = trigger.gameObject.AddComponent<DSUsable>();
			usable.overrideName = "Tinder Source";
			usable.overrideUseMessage = "Load";
		}
	}
}