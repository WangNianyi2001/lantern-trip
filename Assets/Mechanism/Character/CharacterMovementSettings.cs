using UnityEngine;
using System;
using NaughtyAttributes;

namespace LanternTrip {
	[CreateAssetMenu(menuName = "LanternTrip/CharacterMovementSettings")]
	public class CharacterMovementSettings : ScriptableObject {
		[Serializable]
		public struct Walking {
			[Range(0, 100)] public float accelerationGain;
			[Range(0, 100)] public float maxAcceleration;
			[Range(0, 20)] public float torqueGain;
			[Range(0, 50)] public float speed;
			[Range(0, 90)] public float maxSlopeAngle;
			[Range(0, 1)] public float maxAiringDuration;
		}
		public Walking walking;

		[Serializable]
		public struct Jumping {
			[Range(0, 4)] public float height;
			[Range(0, 1)] public float preWaitingTime;
			public bool autoJump;
			[ShowIf("autoJump")] public float autoJumpRadius;
			[ShowIf("autoJump")] public float autoJumpHeight;
			[ShowIf("autoJump")] public float autoJumpBottomSlitHeight;
		}
		public Jumping jumping;
	}
}