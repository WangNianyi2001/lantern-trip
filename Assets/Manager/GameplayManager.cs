using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using static UnityEngine.Rendering.DebugUI;

namespace LanternTrip {
	[ExecuteInEditMode]
	[RequireComponent(typeof(InputManager))]
	public class GameplayManager : ManagerBase {
		#region Static members
		public static GameplayManager instance;
		static string lastCheckpointName;
		static float cinder = 0;
		#endregion

		#region Serialized members
		new public Protagonist protagonist;
		public InputManager input;
		public UiManager ui;
		new public CameraManager camera;
		public PixelCrushers.DialogueSystem.DialogueSystemController ds;
		[Expandable] public GameSettings settings;
		public Checkpoint startingCheckpoint;
		public new AudioSource audio;
		public AudioClip collectTinderAudio;
		public List<Prop> props;
		public List<Tinder> tinderTemplates;
		#endregion

		#region Internal members
		[NonSerialized] public LanternSlot[] lanternSlots;
		float bonusTime;
		List<Bonus> activeBonuses = new List<Bonus>();
		int safezoneCounter = 0;
		int coldzoneCounter = 0;
		int propIndex = 0;
		bool cheating;
		bool paused = false;
		#endregion

		#region Internal methods
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
			float maxTimeLeft = MaxLanternTimeLeft;
			if(maxTimeLeft < time) {
				foreach(var slot in lanternSlots) {
					if(slot.Burn(maxTimeLeft))
						Cinder += maxTimeLeft;
				}
				return time - maxTimeLeft;
			}
			else {
				foreach(var slot in lanternSlots) {
					if(slot.Burn(time))
						Cinder += time;
				}
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

		IEnumerator ConversationCoroutine(PixelCrushers.DialogueSystem.ConversationController controller) {
			yield return new WaitUntil(() => !controller.isActive);
			// ResumePhysics();
			// mute input
			input.GainPlayerControl();
			// show cursor
			Cursor.lockState = CursorLockMode.Locked;
		}
		#endregion

		#region Public interfaces
		[Range(0, 10)] public float burningRate = 1;

		public Checkpoint LastCheckpoint {
			get => GameObject.Find(lastCheckpointName)?.GetComponent<Checkpoint>() ?? startingCheckpoint;
			set => lastCheckpointName = value?.gameObject?.name;
		}

		public LanternSlot currentLanterSlot => ui.slotTrack.Current;

		[NonSerialized] public bool burning = false;

		public float BonusTime {
			get => bonusTime;
			set {
				bonusTime = value;
				ui.bonusSlot.SetValue(value);
			}
		}

		[NonSerialized] public float speedBonusRate = 1;
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
			PlaySfx(collectTinderAudio);
			ActivateSatisfiedBonus();
			return true;
		}

		public float MaxLanternTimeLeft => lanternSlots.Select(slot => slot.timeLeft).Max();
		public float TimeLeft => MaxLanternTimeLeft + BonusTime;
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

		public float Cinder {
			get => cinder;
			set {
				cinder = value;
				int v = Mathf.FloorToInt(cinder);
				try {
					ui.cinderNumberText.text = v.ToString();
				}
				catch(Exception e) {
					Debug.LogError(e);
				}
			}
		}

		public void PausePhysics() => Time.timeScale = 0;
		public void ResumePhysics() => Time.timeScale = 1;

		public void StartConversation(string name) {
			// PausePhysics();
			
			// mute input
			input.DiscardPlayerControl();
			// show cursor
			Cursor.lockState = CursorLockMode.Confined;

			ds.StartConversation(name);
			StartCoroutine(ConversationCoroutine(ds.ConversationController));
		}

		public void PlaySfx(AudioClip clip) {
			if(!audio)
				return;
			audio.PlayOneShot(clip);
		}

		public void RestartLevel() {
			Paused = false;
			if(Cinder <= settings.respawnCinderCost) {
				SceneLoader.instance.LoadAsync(settings.gameOverScene);
				Destroy(gameObject);
				return;
			}
			Cinder -= settings.respawnCinderCost;
			LoadTinder(settings.respawnGift);
			SceneLoader.instance.LoadAsync(SceneManager.GetActiveScene().name);
		}

		public bool HasProp => props.Count > 0;
		public int PropIndex {
			get => HasProp ? propIndex : -1;
			set {
				propIndex = HasProp ? Mathf.FloorToInt(MathUtil.Mod(value, props.Count)) : -1;
				ui.prop.gameObject.SetActive(HasProp);
				ui.prop.Prop = Prop;
			}
		}
		public Prop Prop {
			get => HasProp ? props[propIndex] : null;
		}
		public void UseProp(int index) {
			Prop prop = props[index];
			props.RemoveAt(index);
			PropIndex = PropIndex;
			prop.onUse?.Invoke();
		}
		public void UseCurrentProp() => UseProp(propIndex);

		public bool Cheating {
			get => cheating;
			set {
				cheating = value;
				if(value) {
					protagonist.state = "Flying";
					protagonist.Rigidbody.useGravity = false;
					foreach(var tinder in tinderTemplates) {
						LoadTinder(tinder);
						ScrollSlot(1);
					}
				}
				else {
					protagonist.Rigidbody.useGravity = true;
					protagonist.state = "Freefalling";
				}
			}
		}

		public bool Paused {
			get => paused;
			set {
				paused = value;
				input.Enabled = !value;
				Time.timeScale = value ? 0 : 1;
				ui.pause.gameObject.SetActive(value);
				Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
			}
		}

		public void StartGame() {
			camera.vCam.enabled = true;
			Paused = false;
		}
		#endregion

		#region Life cycle
		void Awake() {
			if(!Application.isPlaying)
				return;

			if(instance != null && instance != this) {
				Debug.Log("Gameplay instance is not self, destroying.");
				Destroy(gameObject);
				return;
			}

			instance = this;
			DontDestroyOnLoad(gameObject);
			var name = SceneManager.GetActiveScene().name;
			SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) => {
				if(scene.name != name)
					return;
				Reset();
			};
		}

		void Reset() {
			protagonist = FindObjectOfType<Protagonist>();
			Cinder = Cinder;
			LastCheckpoint?.Restore();
			camera.ResetVCam();
			safezoneCounter = 0;
			PropIndex = PropIndex;

			if(Application.isPlaying) {
				if(SceneLoader.instance)	// Might be null (no scene loader present in current scene)
					SceneLoader.instance.gameObject.SetActive(false);
			}
		}

		void Start() {
			if(!Application.isPlaying)
				return;
			Reset();
			Time.fixedDeltaTime = 1 / settings.fps;
			Paused = true;
			ui.pause.gameObject.SetActive(false);
		}

		void FixedUpdate() {
			if(!Application.isPlaying)
				return;
			if(burning && !cheating) {
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
		}

		void EditorUpdate() {
			instance = this;
			Reset();
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