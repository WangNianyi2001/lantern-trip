using UnityEngine;
using System;
using System.Linq;

namespace LanternTrip {
	[RequireComponent(typeof(InputManager))]
	public class GameplayManager : ManagerBase {
		public static GameplayManager instance;

		#region Inspector members
		new public Protagonist protagonist;
		public UiManager ui;
		public uint lanternSlotCount = 3u;
		#endregion

		#region Core members
		[NonSerialized] public InputManager input;
		LanternSlot[] lanternSlots;
		#endregion

		#region Public interfaces
		[Range(0, 10)] public float burningRate = 1;

		public LanternSlot FirstEmptyLanternSlot {
			get => lanternSlots.FirstOrDefault(slot => slot.tinder == null);
		}

		public bool burning = true;

		/// <summary>Try to load given type of tinder into first empty lantern and start burning.</summary>
		/// <returns>`true` if succeed, `false` otherwise.</returns>
		public bool LoadTinder(Tinder tinder) {
			LanternSlot targetSlot = FirstEmptyLanternSlot;
			if(targetSlot == null)
				return false;
			targetSlot.Load(tinder, true);
			return true;
		}

		public bool Burn(float time) {
			if(lanternSlots.All(slot => slot.tinder == null))
				return false;
			foreach(var slot in lanternSlots)
				slot.Burn(time);
			return true;
		}
		#endregion

		#region Life cycle
		void Awake() {
			instance = this;
		}

		void Start() {
			// Get component reference
			input = GetComponent<InputManager>();

			// Initialize lantern slots
			lanternSlots = new LanternSlot[lanternSlotCount];
			for(int i = 0; i < lanternSlotCount; ++i)
				lanternSlots[i] = new LanternSlot(ui.CreateLanternSlot());
		}

		void FixedUpdate() {
			if(burning) {
				bool burntOut = !Burn(Time.fixedDeltaTime * burningRate);
				if(burntOut) {
					//
				}
			}
		}
			#endregion
		}
}