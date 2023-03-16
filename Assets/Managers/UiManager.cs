using UnityEngine;

namespace LanternTrip {
	public class UiManager : MonoBehaviour {
		#region Inspector members
		public SlotTrack slotTrack;
		public GameObject lanternSlotUIPrefab;
		public BonusSlotUI bonusSlot;
		#endregion

		#region Public interfaces
		public LanternSlotUI CreateLanternSlot() {
			GameObject ui = Instantiate(lanternSlotUIPrefab, slotTrack.transform);
			ui.transform.SetSiblingIndex(bonusSlot.transform.GetSiblingIndex());
			return ui.GetComponent<LanternSlotUI>();
		}
		#endregion

	}
}