using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LanternTrip {
	[ExecuteAlways]
	[RequireComponent(typeof(InputManager))]
	public class GameplayManager : ManagerBase {
		public static GameplayManager instance;
		public static string lastCheckpointName;

		#region Serialized members
		new public Protagonist protagonist;
		public InputManager input;
		public UiManager ui;
		new public CameraManager camera;
		[Expandable] public GameSettings settings;
		public Checkpoint startingCheckpoint;
		#endregion

		#region Internal members
		[NonSerialized] public LanternSlot[] lanternSlots;
		float bonusTime;
		List<Bonus> activeBonuses = new List<Bonus>();
		float chargeUpSpeed = 0;
		float chargeUpValue = 0;
		int safezoneCounter = 0;
		int coldzoneCounter = 0;
		#endregion

		#region Internal methods
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

		bool IsDuplicatedOnStart() {
			if(instance == null)
				return false;
			return instance == this;
		}
		#endregion

		#region Public interfaces
		[Range(0, 10)] public float burningRate = 1;

		public Checkpoint LastCheckpoint {
			get => GameObject.Find(lastCheckpointName)?.GetComponent<Checkpoint>();
			set => lastCheckpointName = value?.gameObject?.name;
		}
		public void RestoreLastCheckpoint() {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		public LanternSlot currentLanterSlot => ui.slotTrack.Current;

		[NonSerialized] public bool burning = false;

		public float speedBonusRate = 1;
		public bool coldDebuffEnabled = true;
		public bool InCold => coldzoneCounter > 0;

		public void EnterColdzone() => ++coldzoneCounter;
		public void ExitColdzone() => --coldzoneCounter;

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

		public void GrantBonusTime(float time) => BonusTime += time;

		public void ScrollSlot(int delta) {
			if(currentLanterSlot == null) {
				ui.slotTrack.Current = lanternSlots[0];
				return;
			}
			int index = (currentLanterSlot.Index + lanternSlots.Length + delta) % lanternSlots.Length;
			ui.slotTrack.Current = lanternSlots[index];
		}

		public void EnterSafezone() {
			++safezoneCounter;
			burning = false;
		}
		public void ExitSafezone() {
			--safezoneCounter;
			if(safezoneCounter < 0)
				safezoneCounter = 0;
			burning = safezoneCounter == 0;
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
				if(protagonist.CanShoot)
					chargeUpValue = value;
				else
					chargeUpValue = 0;
				protagonist.animationController.ChargingUpValue = chargeUpValue;
			}
		}
		#endregion

		#region Life cycle
		void Awake() {
			if(!Application.isPlaying)
				return;

			if(IsDuplicatedOnStart()) {
				Destroy(gameObject);
				return;
			}

			instance = this;
		}

		void OnRestart() {
			var cp = LastCheckpoint ?? startingCheckpoint;
			cp?.Restore();
		}

		void Start() {
			if(!Application.isPlaying)
				return;

			// Initialize lantern slots
			lanternSlots = new LanternSlot[settings.lanternSlotCount];
			for(int i = 0; i < settings.lanternSlotCount; ++i)
				lanternSlots[i] = new LanternSlot(ui.CreateLanternSlot());
			ui.slotTrack.Current = lanternSlots[0];

			OnRestart();
		}

		void FixedUpdate() {
			if(!Application.isPlaying)
				return;
			if(burning) {
				float burnTime = Time.fixedDeltaTime * burningRate;
				if(coldDebuffEnabled && InCold) {
					int redCount = lanternSlots.Where(slot => slot.tinder?.type == Tinder.Type.Red).Count();
					burnTime *= redCount == 3 ? 1 : redCount > 0 ? 1.5f : 2;
				}
				bool burntOut = !Burn(burnTime);
				if(activeBonuses.Count > 0)
					DeactivateUnsatisfiedBonus();
				if(burntOut && protagonist.state != "Dead")
					protagonist.SendMessage("OnDie");
			}
			ChargeUpValue += ChargeUpSpeed * Time.fixedDeltaTime;
		}

		void EditorUpdate() {
			instance = this;
			OnRestart();
		}

		void Update() {
			if(!Application.isPlaying) {
				EditorUpdate();
				return;
			}
		}
		#endregion
	}
}