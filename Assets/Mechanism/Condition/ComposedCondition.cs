using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

namespace LanternTrip {
	public enum ConditionComposingType {
		And, Or,
	}

	public class ComposedCondition : MonoBehaviour {
		public ConditionComposingType composingType;
		[SerializeField] List<MonoCondition> monoConditions;
		public UnityEvent onValueChange;
		public UnityEvent onFulfill;

		bool value;
		public bool Value => value;

		public void UpdateValue() {
			var values = monoConditions.Select(x => x.Value);
			bool result = false;
			switch(composingType) {
				case ConditionComposingType.And:
					result = true;
					foreach(var v in values)
						result &= v;
					break;
				case ConditionComposingType.Or:
					result = false;
					foreach(var v in values)
						result |= v;
					break;
			}
			bool changed = value != result;
			value = result;
			if(changed) {
				onValueChange?.Invoke();
				if(value)
					onFulfill?.Invoke();
			}
		}

		void OnEnable() {
			foreach(var mono in monoConditions)
				mono.OnValueChanged += UpdateValue;
		}

		void OnDisable() {
			foreach(var mono in monoConditions)
				mono.OnValueChanged -= UpdateValue;
		}
	}
}