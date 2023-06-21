using UnityEngine;
using UnityEngine.UI;

namespace LanternTrip {
	[RequireComponent(typeof(RectTransform))]
	public class DashUi : MonoBehaviour {
		public Image icon, background;

		public float CdProgress {
			set {
				icon.fillAmount = value;
				background.fillAmount = 1 - value;
			}
		}
	}
}