using UnityEngine;
using UnityEngine.UI;

namespace LanternTrip {
	public class LanternSlotUI : SlotUI {
		public Image icon;
		public RectMask2D mask;
		public Text timeLeftText;

		Tinder tinder;
		float value;

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
				icon.color = value?.mainColor ?? Color.gray;
				if(value == null)
					Value = 0;
			}
		}

		public override float Value {
			get => value;
			set {
				if(tinder == null) {
					this.value = 0;
					maskProgress = 1;
					timeLeftText.text = string.Empty;
				}
				else {
					this.value = value;
					maskProgress = value / tinder.timeSpan;
					timeLeftText.text = Mathf.Floor(value).ToString();
				}
			}
		}

		void Start() {
			Tinder = null;
		}
	}
}