using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace LanternTrip {
	public partial class Protagonist : Character {
		GameplayManager gameplay => GameplayManager.instance;

		#region Internal fields
		Transform shootTarget;
		float chargeUpSpeed = 0;
		float chargeUpValue = 0;
		float previousChargeUpValue = 0;
		#endregion

		#region Serialized fields
		public PixelCrushers.DialogueSystem.ProximitySelector selector;

		[Header("Shooting")]
		public Shooter shooter;
		[MinMaxSlider(1, 20)] public Vector2 shootingRange;
		public GameObject shootTargetPrefab;
		public LineRenderer lineRenderer;
		public LayerMask shootingLayerMask;

		[Header("Death")]
		[Range(1, 100)] public float deathBurnSpeed;
		[Range(0, 5)] public float deathTime;

		public AudioClip bowAimAudio;
		#endregion

		#region Internal functions
		protected override Vector3 CalculateWalkingVelocity() {
			return base.CalculateWalkingVelocity() * gameplay.speedBonusRate;
		}

		protected override void UpdateState() {
			base.UpdateState();
			switch(state) {
				case "Walking":
					if(CanShoot && ChargeUpValue > 0) {
						state = "Shooting";
					}
					break;
				case "Shooting":
					if(ChargeUpValue == 0)
						state = "Walking";
					break;
			}
		}

		protected new void OnStateTransit(string from, string to) {
			switch(to) {
				case "Shooting":
					PlaySfx(bowAimAudio);
					break;
				case "Jumping":
					PlaySfx(jumpAudio);
					break;
			}
		}

		protected override Vector3 CalculateExpectedDirection() {
			if(state == "Shooting") {
				if(!ShootTargetPosition.HasValue)
					return transform.forward;
				Vector3 offset = ShootTargetPosition.Value - transform.position;
				offset = offset.ProjectOnto(transform.up);
				return offset.normalized;
			}
			else
				return base.CalculateExpectedDirection();
		}

		IEnumerable<Vector3> YieldShootingCurveCoordinates(float maxT, float dt = .01f) {
			Vector3 p = shooter.transform.position;
			Vector3 v = shooter.CalculateInitialVelocityByTargetPosition(ClampedShootTargetPosition);
			for(float t = 0; t < maxT; t += dt) {
				yield return p;
				v += Physics.gravity * dt;
				p += v * dt;
			}
		}

		/// <summary>Clamped based on charge-up value</summary>
		Vector3 ClampedShootTargetPosition {
			get {
				Vector3 delta = ShootTargetPosition.Value - transform.position;
				float distance = Mathf.Lerp(shootingRange.x, shootingRange.y, previousChargeUpValue);
				if(distance > delta.magnitude)
					distance = Mathf.Max(delta.magnitude, shootingRange.x);
				delta = delta.normalized * distance;
				return delta + transform.position;
			}
		}

		void DoShootingFrame() {
			// Try getting & setting shoot target position
			RaycastHit hit;
			Camera cam = gameplay.camera.camera;
			Ray ray = cam.ScreenPointToRay(gameplay.input.MousePosition);
			Physics.Raycast(ray, out hit, Mathf.Infinity, shootingLayerMask);
			if(!hit.transform)
				ShootTargetPosition = null;
			else {
				ShootTargetPosition = hit.point;

				// If charged-up, render expected shooting curve
				if(ChargeUpValue > 0) {
					var points = YieldShootingCurveCoordinates(shooter.totalTime).ToArray();
					lineRenderer.positionCount = points.Length;
					lineRenderer.SetPositions(points);
					lineRenderer.enabled = true;
				}
			}
		}

		IEnumerator DyingCoroutine() {
			while(true) {
				float bt = deathBurnSpeed * Time.fixedDeltaTime;
				if(gameplay.TimeLeft <= bt)
					break;
				gameplay.Burn(bt);
				yield return new WaitForFixedUpdate();
			}
			gameplay.Burn(gameplay.TimeLeft);
			yield return new WaitForSeconds(deathTime);
			gameplay.RestoreLastCheckpoint();
		}
		#endregion

		#region Public interfaces
		public bool CanShoot => HoldingBow && (state == "Walking" || state == "Shooting");

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
				if(CanShoot)
					chargeUpValue = value;
				else
					chargeUpValue = 0;
				animationController.ChargingUpValue = chargeUpValue;
			}
		}

		public Vector3? ShootTargetPosition {
			get => shootTarget.gameObject.activeInHierarchy ? shootTarget.position : null;
			set {
				if(!HoldingBow || !value.HasValue) {
					shootTarget.gameObject.SetActive(false);
					return;
				}
				shootTarget.position = value.Value;
				shootTarget.gameObject.SetActive(true);
			}
		}

		public bool HoldingBow {
			get => animationController.HoldingBow;
			set {
				if(value == HoldingBow)
					return;

				animationController.HoldingBow = value;
				if(value == false)
					ShootTargetPosition = null;
			}
		}

		public void Shoot() {
			if(!gameplay.Burn(1))
				return;
			shooter.Shoot(ClampedShootTargetPosition);
		}
		#endregion

		#region Life cycle
		protected new void Start() {
			base.OnStateTransit = OnStateTransit;

			base.Start();

			shootTarget = Instantiate(shootTargetPrefab).transform;
			ShootTargetPosition = null;

			shooter.preShoot.AddListener(arrowObj => {
				var arrow = arrowObj.GetComponent<Arrow>();
				if(!arrow)
					throw new UnityException("Passed-in object does not have an arrow component");
				arrow.Tinder = gameplay.currentLanterSlot.tinder;
			});
		}

		protected new void Update() {
			base.Update();

			lineRenderer.enabled = false;
			if(CanShoot)
				DoShootingFrame();
		}

		protected void FixedUpdate() {
			ChargeUpValue += ChargeUpSpeed * Time.fixedDeltaTime;
		}

		protected override void OnDie() {
			base.OnDie();
			StartCoroutine(DyingCoroutine());
		}
		#endregion
	}
}