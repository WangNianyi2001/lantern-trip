using UnityEngine;

namespace LanternTrip {
	[CreateAssetMenu(menuName = "LanternTrip/Tinder")]
	public class Tinder : ScriptableObject {
		public string typeName;
		public Color mainColor;
		[Tooltip("How much time could it burn.")]
		[Range(0, 120)] public float timeSpan;
	}
}