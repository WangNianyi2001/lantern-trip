using UnityEngine;
using UnityEngine.UI;

namespace LanternTrip {
	public class LanternSlotUI : SlotUI {
		public RectMask2D mask;
		public Text timeLeftText;

		Tinder tinder;

		float maskProgress {
			set {
				value = Mathf.Clamp01(value);
				RectTransform rt = mask.rectTransform;
				Vector2 max = rt.anchorMax;
				max.y = value;
				rt.anchorMax = max;
			}
		}

		public Tinder Tinder {
			get => tinder;
			set {
				tinder = value;
				if(value == null)
					SetValue(0);
			}
		}

		public override void SetValue(float value) {
			if(tinder == null) {
				maskProgress = 1;
				timeLeftText.text = string.Empty;
			}
			else {
				maskProgress = value / tinder.timeSpan;
				timeLeftText.text = Mathf.Floor(value).ToString();
			}
			base.SetValue(value);
		}

		protected new void Start() {
			base.Start();

			Tinder = null;
		}

		protected new void FixedUpdate() {
			base.FixedUpdate();
			graphic.material?.SetColor("mainColor", tinder?.mainColor ?? Color.gray);
		}
	}
}