using UnityEngine;
using UnityEngine.UI;

namespace LanternTrip {
	public class BonusSlotUI : SlotUI {
		public Text timeLeftText;
		
		public override void SetValue(float value) {
			timeLeftText.text = Mathf.Floor(value).ToString();
			gameObject.SetActive(value > 0);
			base.SetValue(value);
		}

		void Start() {
			SetValue(0);
		}
	}
}