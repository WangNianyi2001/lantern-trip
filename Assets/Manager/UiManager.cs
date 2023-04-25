using UnityEngine;
using UnityEngine.UI;

namespace LanternTrip {
	public class UiManager : MonoBehaviour {
		#region Serialized members
		public SlotTrack slotTrack;
		public GameObject lanternSlotUIPrefab;
		public BonusSlotUI bonusSlot;
		public GameObject interactionDirectionEntryPrefab;
		public Text cinderNumberText;
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