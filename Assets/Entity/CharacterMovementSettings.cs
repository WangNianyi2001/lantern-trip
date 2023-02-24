using UnityEngine;
using System;

namespace LanternTrip {
	[CreateAssetMenu(menuName = "LanternTrip/CharacterMovementSettings")]
	public class CharacterMovementSettings : ScriptableObject {
		[Serializable]
		public struct Walking {
			[Range(0, 100)] public float accelerationGain;
			[Range(0, 100)] public float maxAcceleration;
			[Range(0, 100)] public float maxSpeed;
			[Range(0, 90)] public float maxSlopeAngle;
		}
		public Walking walking;

		[Serializable]
		public struct Jumping {
			[Range(0, 5)] public float speed;
		}
		public Jumping jumping;
	}
}