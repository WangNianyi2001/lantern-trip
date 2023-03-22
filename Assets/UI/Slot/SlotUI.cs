using UnityEngine;
using UnityEngine.Events;

namespace LanternTrip {
	public abstract class SlotUI : MonoBehaviour {
		float value;

		public float GetValue() => value;

		public virtual void SetValue(float value) {
			float oldValue = this.value;
			this.value = value;
			if(oldValue == 0 && value != 0)
				onFill?.Invoke();
			if(oldValue != 0 && value == 0)
				onExhausted?.Invoke();
		}

		public UnityEvent onFill;
		public UnityEvent onExhausted;
	}
}