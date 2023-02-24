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

		#region Core methods
		public LanternSlot FirstEmptyLanternSlot {
			get => lanternSlots.First(slot => slot.tinder == null);
		}
		#endregion

		#region Public interfaces
		[NonSerialized] public bool burning;

		/// <summary>Try to put given type of tinder into first empty lantern and start burning.</summary>
		/// <returns>`true` if succeed, `false` otherwise.</returns>
		public bool BurnTinder(Tinder tinder) {
			LanternSlot targetSlot = FirstEmptyLanternSlot;
			if(targetSlot == null)
				return false;
			targetSlot.tinder = tinder;
			targetSlot.timeLast = tinder.timeSpan;
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
				LanternSlot lanternSlot = ScriptableObject.CreateInstance<LanternSlot>();
				lanternSlot.ui = ui.GetComponent<LanternSlotUI>();
				lanternSlots[i] = lanternSlot;
			}
		}

		void FixedUpdate() {
			// If burning, burns
			// If fuel exausted, game end
		}
		#endregion
	}
}