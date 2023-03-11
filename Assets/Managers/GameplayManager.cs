using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LanternTrip {
	[RequireComponent(typeof(InputManager))]
	public class GameplayManager : ManagerBase {
		public static GameplayManager instance;

		#region Inspector members
		new public Protagonist protagonist;
		public UiManager ui;
		public uint lanternSlotCount = 3u;

		[Header("Bonus")]
		public UnityEvent allDifferentBonus;
		[Serializable]
		public struct AllSameBonus {
			public string typeName;
			public UnityEvent bonus;
		}
		public List<AllSameBonus> allSameBonus;
		#endregion

		#region Core members
		[NonSerialized] public InputManager input;
		[NonSerialized] public LanternSlot[] lanternSlots;
		float bonusTime;
		#endregion

		#region Core methods
		public float BonusTime {
			get => bonusTime;
			set {
				bonusTime = value;
				ui.bonusSlot.SetValue(value);
			}
		}

		float BurnBonus(float time) {
			if(BonusTime >= time) {
				BonusTime -= time;
				time = 0;
			}
			else {
				time -= bonusTime;
				BonusTime = 0;
			}
			return time;
		}

		/// <summary>Returns 0 if it's possible to burn any lantern by given time.</summary>
		float BurnLanterns(float time) {
			if(lanternSlots.All(slot => slot.tinder == null))
				return time;
			float maxTimeLeft = lanternSlots.Select(slot => slot.timeLeft).Max();
			if(maxTimeLeft < time) {
				foreach(var slot in lanternSlots)
					slot.Burn(maxTimeLeft);
				return time - maxTimeLeft;
			}
			else {
				foreach(var slot in lanternSlots)
					slot.Burn(time);
				return 0;
			}
		}

		/// <summary>Check if certain bonus condition is satisfied.</summary>
		/// <returns>Bonus action that should be granted.</returns>
		List<UnityEvent> CheckForBonus() {
			var bonuses = new List<UnityEvent>();
			if(lanternSlots.Any(slot => slot.tinder == null))
				return bonuses;
			bool allSame = true, allDifferent = true;
			string firstType = lanternSlots[0].tinder.typeName;
			for(int i = 1; i < lanternSlots.Count(); ++i) {
				LanternSlot slot = lanternSlots[i];
				if(slot.tinder.typeName != firstType)
					allSame &= false;
				if(lanternSlots.Take(i - 1).Any(other => slot.tinder.typeName == other.tinder.typeName))
					allDifferent &= false;
			}
			if(allSame)
				bonuses.Add(allSameBonus.Find(bonus => bonus.typeName == firstType).bonus);
			if(allDifferent)
				bonuses.Add(allDifferentBonus);
			return bonuses;
		}
		#endregion

		#region Public interfaces
		[Range(0, 10)] public float burningRate = 1;

		public LanternSlot SelectedLanternSlot {
			get => lanternSlots.FirstOrDefault(slot => slot.tinder == null);
		}

		public bool burning = true;

		/// <summary>Try to load given type of tinder into first empty lantern and start burning.</summary>
		/// <returns>`true` if succeed, `false` otherwise.</returns>
		public bool LoadTinder(Tinder tinder) {
			if(SelectedLanternSlot == null)
				return false;
			SelectedLanternSlot.Load(tinder, true);
			CheckForBonus().ForEach(action => action.Invoke());
			return true;
		}

		public bool Burn(float time) {
			if(time == 0)
				return true;
			time = BurnBonus(time);
			if(time == 0)
				return true;
			time = BurnLanterns(time);
			return time == 0;
		}

		public void AddBonusTime(float time) => BonusTime += time;
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