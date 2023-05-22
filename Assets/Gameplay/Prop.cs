using UnityEngine;
using UnityEngine.Events;

namespace LanternTrip {
	[CreateAssetMenu(menuName = "LanternTrip/Prop")]
	public class Prop : ScriptableObject {
		public new string name;
		public Sprite texture;
		public UnityEvent onUse;
	}
}