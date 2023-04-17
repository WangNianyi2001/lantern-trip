using UnityEngine;
using NaughtyAttributes;

namespace LanternTrip {
	[CreateAssetMenu(menuName = "LanternTrip/ShootingSettings")]
	public class ShootingSettings : ScriptableObject {
		public Vector3 outPosition;
		[MinMaxSlider(0, 20)] public Vector2 range;
		[Min(0)] public float maxTime;
		[Min(0)] public float maxSlope;
	}
}