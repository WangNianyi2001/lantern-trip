using UnityEngine;
using System;
using System.Linq;

namespace LanternTrip {
	[RequireComponent(typeof(InputManager))]
	public class GameplayManager : ManagerBase {
		public static GameplayManager instance;

		#region Inspector members
		new public Protagonist protagonist;
		public RectTransform lanternSlotTrack;
		public GameObject lanternSlotUIPrefab;
		#endregion

		#region Core members
		InputManager input;
		const uint lanternSlotCount = 3u;
		LanternSlot[] lanternSlots;
		#endregion

		#region Public interfaces
		[Range(0, 10)] public float burningRate = 1;

		public LanternSlot FirstEmptyLanternSlot {
			get => lanternSlots.FirstOrDefault(slot => slot.tinder == null);
		}

		[NonSerialized] public bool burning;

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

			// Initialize lantern slots & track UI
			// First clean all editor-preview slots in the track
			foreach(Transform child in lanternSlotTrack.transform)
				Destroy(child.gameObject);
			lanternSlots = new LanternSlot[lanternSlotCount];
			for(int i = 0; i < lanternSlotCount; ++i) {
				GameObject ui = Instantiate(lanternSlotUIPrefab, lanternSlotTrack);
				lanternSlots[i] = new LanternSlot(ui.GetComponent<LanternSlotUI>());
			}
		}

		void FixedUpdate() {
			if(!Burn(Time.fixedDeltaTime * burningRate)) {
				//
			}
		}
		#endregion
	}
}