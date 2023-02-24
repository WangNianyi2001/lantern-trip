using UnityEngine;
using System;

namespace LanternTrip {
	public class LanternSlot : ScriptableObject {
		public Tinder tinder = null;
		public float timeLast = 0;
		[NonSerialized] public LanternSlotUI ui;

		/// <summary>Burn tinder in this slot by certain amount of time.</summary>
		/// <returns>`true` if succeed, `false` if slot is empty or tinder exhausted.</returns>
		public bool Burn(float time) {
			if(tinder == null)
				return false;
			timeLast -= time;
			if(timeLast < 0) {
				timeLast = 0;
				tinder = null;
				return false;
			}
			return true;
		}
	}
}