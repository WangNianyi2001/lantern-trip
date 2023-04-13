using UnityEngine;
using UnityEngine.Events;

namespace LanternTrip
{
	[RequireComponent(typeof(Entity))]
	public class TinderSource : MonoBehaviour {
		/// <summary>Temporary solution for getting current tinder.</summary>
		/// <remarks>Might be changed in the future.</remarks>
		public static TinderSource current = null;

		Trigger trigger;
		public Tinder type;
		public UnityEvent onApproach;
		public UnityEvent onLeave;
		public UnityEvent onDeliver;

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
	}
}