using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LanternTrip {
	public abstract class SlotUI : MonoBehaviour {
		float value;

		public Image graphic;

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

		protected void Start() {
			graphic.material = new Material(graphic.material);
		}

		protected void FixedUpdate() {
			graphic.material?.SetFloat("t", value);
		}
	}
}