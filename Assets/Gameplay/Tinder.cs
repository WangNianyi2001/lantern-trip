using UnityEngine;

namespace LanternTrip {
	[CreateAssetMenu(menuName = "LanternTrip/Tinder")]
	public class Tinder : ScriptableObject {
		/// <remarks>Use Type.Invalid to indicate bonus for all different types</remarks>
		public enum Type {
			Invalid = 0,
			Red = 1,
			Green = 2,
			Blue = 4
		}
		public Type type;
		public Color mainColor;
		[Tooltip("How much time could it burn.")]
		[Range(0, 120)] public float timeSpan;
	}
}
