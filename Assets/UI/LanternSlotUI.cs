using UnityEngine;
using UnityEngine.UI;

namespace LanternTrip {
	public class LanternSlotUI : SlotUI {
		public Image icon;
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
                //icon.color = value?.mainColor ?? Color.gray;
                switch (value?.mainColor)
                {
                    case TinderType.Red:
						icon.color = Color.red;
						break;
                    case TinderType.Green:
						icon.color = Color.green;
						break;
                    case TinderType.Blue:
						icon.color = Color.blue;
                        break;
                    default:
                        break;
                }
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

		void Start() {
			Tinder = null;
		}
	}
}