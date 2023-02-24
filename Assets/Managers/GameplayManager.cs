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
		public LanternSlot FirstEmptyLanternSlot {
			get => lanternSlots.FirstOrDefault(slot => slot.tinder == null);
		}
		public LanternSlot LastloadedLanternSlot {
			get => lanternSlots.LastOrDefault(slot => slot.tinder != null);
		}

		[NonSerialized] public bool burning;

		/// <summary>Try to load given type of tinder into first empty lantern and start burning.</summary>
		/// <returns>`true` if succeed, `false` otherwise.</returns>
		public bool LoadTinder(Tinder tinder) {
			LanternSlot targetSlot = FirstEmptyLanternSlot;
			if(targetSlot == null)
				return false;
			targetSlot.tinder = tinder;
			targetSlot.timeLast = tinder.timeSpan;
			return true;
		}

		public bool Burn(float time) {
			LanternSlot lastSlot = LastloadedLanternSlot;
			if(lastSlot == null)
				return false;
			if(lastSlot.timeLast >= time) {
				lastSlot.Burn(time);
				return true;
			}
			time -= lastSlot.timeLast;
			lastSlot.Burn(lastSlot.timeLast);
			return Burn(time);
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
				LanternSlot lanternSlot = ScriptableObject.CreateInstance<LanternSlot>();
				lanternSlot.ui = ui.GetComponent<LanternSlotUI>();
				lanternSlots[i] = lanternSlot;
			}
		}

		void FixedUpdate() {
			if(!Burn(Time.fixedDeltaTime)) {
				// All lanterns exhausted
			}
		}
		#endregion
	}
}