using UnityEngine;
using UnityEngine.UI;

namespace LanternTrip {
	public class PropUi : MonoBehaviour {
		Prop prop;

		public Image texture;

		public Prop Prop {
			get => prop;
			set {
				prop = value;
				if(prop) {
					texture.sprite = prop.texture;
				}
				else {
					texture.sprite = null;
				}
			}
		}
	}
}