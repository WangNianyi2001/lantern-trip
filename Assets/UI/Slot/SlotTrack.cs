using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace LanternTrip {
	public class SlotTrack : MonoBehaviour {
		public Image selector;

		LanternSlot current;

		IEnumerator UpdateCurrent() {
			yield return new WaitForEndOfFrame();
			RectTransform rt = selector.rectTransform,
				target = current.ui.GetComponent<RectTransform>();
			rt.position = target.position;
			rt.sizeDelta = target.sizeDelta;
			selector.gameObject.SetActive(true);
		}

		public LanternSlot Current {
			get => current;
			set {
				if(value == null) {
					current = null;
					selector.gameObject.SetActive(false);
					return;
				}
				current = value;
				StartCoroutine(UpdateCurrent());
			}
		}
	}
}