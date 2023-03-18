using UnityEngine;
using UnityEngine.UI;
using System;

namespace LanternTrip {
	public class SlotTrack : MonoBehaviour {
		public Image selector;

		LanternSlot current;

		public LanternSlot Current {
			get => current;
			set {
				if(value == null) {
					current = null;
					selector.gameObject.SetActive(false);
					return;
				}
				current = value;
				RectTransform rt = selector.rectTransform,
					target = value.ui.GetComponent<RectTransform>();
				rt.position = target.position;
				rt.sizeDelta = target.sizeDelta;
				selector.gameObject.SetActive(true);
			}
		}
	}
}