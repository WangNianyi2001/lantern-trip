using UnityEngine;
using UnityEngine.UI;

namespace LanternTrip {
	public class LanternSlotUI : MonoBehaviour {
		public Image icon;
		public RectMask2D mask;

		Tinder _tinder;

		float maskValue {
			set {
				value = Mathf.Clamp01(value);
				RectTransform rt = mask.rectTransform;
				float height = (rt.parent as RectTransform).sizeDelta.y;
				Vector2 max = rt.anchorMax;
				max.y = rt.anchorMin.y + height * value;
			}
		}

		public Tinder tinder {
			set {
				_tinder = value;
				icon.color = _tinder?.mainColor ?? Color.gray;
			}
		}

		public float timeLeft {
			set {
				if(_tinder == null) {
					maskValue = 1;
					return;
				}
				maskValue = value / _tinder.timeSpan;
			}
		}

		void Start() {
			tinder = null;
			timeLeft = 0;
		}
	}
}