using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LanternTrip {
	[RequireComponent(typeof(InputManager))]
	public class GameplayManager : ManagerBase {
		public static GameplayManager instance;

		#region Inspector members
		new public Protagonist protagonist;
		public InputManager input;
		public UiManager ui;
		[Expandable] public GameSettings settings;
		#endregion

		#region Core members
		[NonSerialized] public LanternSlot[] lanternSlots;
		float bonusTime;
		List<Bonus> activeBonuses = new List<Bonus>();
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

		void ActivateSatisfiedBonus() {
			foreach(var bonus in settings.bonuses) {
				var types = lanternSlots.Select(slot => slot.tinder?.type ?? Tinder.Type.Invalid);
				if(!bonus.Check(types))
					continue;
				if(bonus.immediate)
					bonus.onGrant.Invoke();
				else {
					activeBonuses.Add(bonus);
					bonus.onActivate.Invoke();
				}
			}
		}

		void DeactivateUnsatisfiedBonus() {
			var survivedList = new List<Bonus>();
			foreach(var bonus in activeBonuses) {
				var types = lanternSlots.Select(slot => slot.tinder?.type ?? Tinder.Type.Invalid);
				if(bonus.Check(types)) {
					survivedList.Add(bonus);
					continue;
				}
				bonus.onDeactivate.Invoke();
			}
			activeBonuses.Clear();
			activeBonuses.AddRange(survivedList);
		}
		#endregion

		#region Public interfaces
		[Range(0, 10)] public float burningRate = 1;

		public LanternSlot SelectedLanternSlot {
			get => lanternSlots.FirstOrDefault(slot => slot.tinder == null);
		}

		bool burning = true;

		/// <summary>Try to load given type of tinder into first empty lantern and start burning.</summary>
		/// <returns>`true` if succeed, `false` otherwise.</returns>
		public bool LoadTinder(Tinder tinder) {
			if(SelectedLanternSlot == null)
				return false;
			SelectedLanternSlot.Load(tinder, true);
			ActivateSatisfiedBonus();
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
			// Initialize lantern slots
			lanternSlots = new LanternSlot[settings.lanternSlotCount];
			for(int i = 0; i < settings.lanternSlotCount; ++i)
				lanternSlots[i] = new LanternSlot(ui.CreateLanternSlot());
		}

		void FixedUpdate() {
			if(burning) {
				bool burntOut = !Burn(Time.fixedDeltaTime * burningRate);
				if(activeBonuses.Count > 0)
					DeactivateUnsatisfiedBonus();
				if(burntOut) {
					//
				}
			}
		}
			#endregion
		}
}