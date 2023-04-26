using UnityEngine;
using UnityEngine.Events;
using System;

namespace LanternTrip {
	public class MonoCondition : MonoBehaviour {
		[SerializeField] bool value;

		public Action OnValueChanged;
		public UnityEvent onValueChanged;

		public bool Value {
			get => value;
			set {
				if(value == this.value)
					return;
				this.value = value;
				onValueChanged?.Invoke();
				OnValueChanged();
			}
		}
	}
}