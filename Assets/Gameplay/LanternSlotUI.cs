using UnityEngine;
using UnityEngine.UI;

namespace LanternTrip {
	public class LanternSlotUI : MonoBehaviour {
		public Image icon;
		public RectMask2D mask;
		public Text timeLeftText;

		Tinder _tinder;

		float maskValue {
			set {
				value = Mathf.Clamp01(value);
				RectTransform rt = mask.rectTransform;
				Vector2 max = rt.anchorMax;
				max.y = value;
				rt.anchorMax = max;
			}
		}

		public Tinder tinder {
			set {
				_tinder = value;
				icon.color = value?.mainColor ?? Color.gray;
				if(value == null)
					timeLeft = 0;
			}
		}

		public float timeLeft {
			set {
				if(_tinder == null) {
					maskValue = 1;
					timeLeftText.text = string.Empty;
					return;
				}
				maskValue = value / _tinder.timeSpan;
				timeLeftText.text = Mathf.Floor(value).ToString();
			}
		}

		void Start() {
			tinder = null;
		}
	}
}