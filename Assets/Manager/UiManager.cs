using UnityEngine;
using UnityEngine.UI;
using System;

namespace LanternTrip {
	public class UiManager : ManagerBase {
		#region Serialized members
		public SlotTrack slotTrack;
		public GameObject lanternSlotUIPrefab;
		public GameObject bonusSlotPrefab;
		public GameObject selectorPrefab;
		public GameObject interactionDirectionEntryPrefab;
		public Text cinderNumberText;
		public PropUi prop;
		public DashUi dash;
		#endregion

		[NonSerialized] public BonusSlotUI bonusSlot;

		#region Life cycle
		protected void Start() {
			foreach(Transform t in slotTrack.transform)
				Destroy(t.gameObject);

			bonusSlot = Instantiate(bonusSlotPrefab, slotTrack.transform).GetComponent<BonusSlotUI>();
			slotTrack.selector = Instantiate(selectorPrefab, slotTrack.transform).GetComponent<Image>();

			gameplay.lanternSlots = new LanternSlot[gameplay.settings.lanternSlotCount];
			for(int i = 0; i < gameplay.settings.lanternSlotCount; ++i)
				gameplay.lanternSlots[i] = new LanternSlot(Instantiate(lanternSlotUIPrefab, slotTrack.transform).GetComponent<LanternSlotUI>());

			slotTrack.Current = gameplay.lanternSlots[0];
		}

		protected void Update() {
			if(dash) {
				dash.CdProgress = gameplay.protagonist?.dashCdProgress ?? 0;
			}
		}
		#endregion
	}
}