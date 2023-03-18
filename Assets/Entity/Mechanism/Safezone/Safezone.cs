using UnityEngine;

namespace LanternTrip {
	public class Safezone : MonoBehaviour {
		public Trigger trigger;

		protected void Start() {
			if(trigger == null)
				trigger = GetComponentInChildren<Trigger>();
			if(trigger == null)
				Debug.LogWarning("Trigger for safezone is null");


			trigger.onEnter.AddListener(_ => {
				Debug.Log("Enter safezone");
				GameplayManager.instance.burning = false;
			});
			trigger.onExit.AddListener(_ => {
				Debug.Log("Exit safezone");
				GameplayManager.instance.burning = true;
			});
		}
	}
}