using UnityEngine;
using NaughtyAttributes;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace LanternTrip {
	[RequireComponent(typeof(InputManager))]
	public class GameplayManager : ManagerBase {
		public static GameplayManager instance;

		#region Inspector members
		new public Protagonist protagonist;
		public InputManager input;
		public UiManager ui;
		public ShootManager shoot;
		[Expandable] public GameSettings settings;
		#endregion

		#region Core members
		[NonSerialized] public LanternSlot[] lanternSlots;
		float bonusTime;
		List<Bonus> activeBonuses = new List<Bonus>();
		float chargeUpSpeed = 0;
		float chargeUpValue = 0;
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

		IEnumerator StartingCoroutine() {
			yield return new WaitForEndOfFrame();
			ui.slotTrack.Current = lanternSlots[0];
		}
		#endregion

		#region Public interfaces
		[Range(0, 10)] public float burningRate = 1;

		public LanternSlot currentLanterSlot => ui.slotTrack.Current;

		[NonSerialized] public bool burning = false;

		/// <summary>Try to load given type of tinder into first empty lantern and start burning.</summary>
		/// <returns>`true` if succeed, `false` otherwise.</returns>
		public bool LoadTinder(Tinder tinder) {
			if(tinder == null) {
				Debug.LogWarning("Tinder to load is null");
				return false;
			}
			if(currentLanterSlot == null)
				return false;
			currentLanterSlot.Load(tinder, true);
			ActivateSatisfiedBonus();
			return true;
		}

		public void LoadTinderFromCurrentSource() {
			Debug.Log($"Load tinder from {TinderSource.current?.name ?? "((null))"}");
			TinderSource.current?.Deliver();
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

		public void ScrollSlot(int delta) {
			if(currentLanterSlot == null) {
				ui.slotTrack.Current = lanternSlots[0];
				return;
			}
			int index = (currentLanterSlot.Index + lanternSlots.Length + delta) % lanternSlots.Length;
			ui.slotTrack.Current = lanternSlots[index];
		}

		public bool HoldingBow {
			get => protagonist.animationController.HoldingBow;
			set {
				if(value == HoldingBow)
					return;

				protagonist.animationController.HoldingBow = value;
				shoot.enabled = value;
			}
		}

		[NonSerialized] public float previousChargeUpValue = 0;
		public float ChargeUpSpeed {
			get => chargeUpSpeed;
			set {
				value = Mathf.Clamp01(value);
				chargeUpSpeed = value;
				if(chargeUpSpeed == 0)
					chargeUpValue = 0;
			}
		}
		public float ChargeUpValue {
			get => chargeUpValue;
			set {
				value = Mathf.Clamp01(value);
				if(value != 0)
					previousChargeUpValue = value;
				if(HoldingBow && protagonist.Idle)
					chargeUpValue = value;
				else
					chargeUpValue = 0;
				protagonist.animationController.ChargingUpValue = chargeUpValue;
			}
		}
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

			StartCoroutine(StartingCoroutine());
		}

		void FixedUpdate() {
			if(burning) {
				bool burntOut = !Burn(Time.fixedDeltaTime * burningRate);
				if(activeBonuses.Count > 0)
					DeactivateUnsatisfiedBonus();
				if(burntOut)
					protagonist.movement.state = Character.Movement.State.Dead;
			}
			ChargeUpValue += ChargeUpSpeed * Time.fixedDeltaTime;
		}
		#endregion
	}
}