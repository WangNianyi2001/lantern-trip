using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace LanternTrip {
	public partial class Protagonist : Character {
		GameplayManager gameplay => GameplayManager.instance;

		#region Internal fields
		float chargeUpSpeed = 0;
		float chargeUpValue = 0;
		float previousChargeUpValue = 0;
		bool dashCding = false;
		Vector3 cachedShootTargetPosition;
		bool kicking = false;
		#endregion

		#region Serialized fields
		public PixelCrushers.DialogueSystem.ProximitySelector selector;

		[Header("Anchors")]
		public Transform bodyAnchor;
		public Transform shootingAnchor;

		[Header("Shooting")]
		public Shooter shooter;
		[MinMaxSlider(1, 20)] public Vector2 shootingRange;
		public RectTransform shootingUi;
		public LineRenderer lineRenderer;
		public LayerMask shootingLayerMask;
		[Range(0, 1)] public float holdingMovementSpeedDebuff;

		[Header("Death")]
		[Range(1, 100)] public float deathBurnSpeed;
		[Range(0, 5)] public float deathTime;

		[Header("Dash")]
		[Tooltip("Dash 一次消耗的燃烧火种数")]
		[Min(0)] public float dashConsuming;
		[Tooltip("移动中 dash 距离")]
		[Range(0, 10)] public float movingDash;
		[Tooltip("静止时 dash 距离")]
		[Range(-5, 5)] public float standingDash;
		[Min(0)] public float dashCd;

		public AudioClip bowAimAudio;
		#endregion

		#region Internal functions
		protected override Vector3 CalculateWalkingVelocity() {
			var v = base.CalculateWalkingVelocity();
			v *= gameplay.speedBonusRate;
			if(HoldingBow)
				v *= holdingMovementSpeedDebuff;
			return v;
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
			if(HoldingBow) {
				if(state == "Shooting" && false) {
					// Deprecated
					if(!ShootTargetPosition.HasValue)
						return transform.forward;
					Vector3 offset = ShootTargetPosition.Value - transform.position;
					offset = offset.ProjectOntoNormal(transform.up);
					return offset.normalized;
				}
				else {
					return gameplay.camera.camera.transform.forward.ProjectOntoNormal(Physics.gravity).normalized;
				}
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
			gameplay.RestartLevel();
		}

		IEnumerator EnrollDashCd() {
			dashCding = true;
			yield return new WaitForSeconds(dashCd);
			dashCding = false;
		}

		IEnumerator KickingCoroutine() {
			string previousState = state;
			state = "Kicking";
			kicking = true;
			animationController.Kicking = true;
			yield return new WaitWhile(() => kicking);
			animationController.Kicking = false;
			state = previousState;
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
				if(HoldingBow) {
					var cam = gameplay.camera;
					cam.Distance = cam.shootingDistance.Lerp(1 - value);
				}
			}
		}

		public Vector3? ShootTargetPosition {
			get {
				if(!shootingUi.gameObject.activeInHierarchy)
					return null;
				return cachedShootTargetPosition;
			}
			set {
				if(!HoldingBow || !value.HasValue) 
					return;
				cachedShootTargetPosition = value.Value;
			}
		}

		public bool HoldingBow {
			get => animationController.HoldingBow;
			set {
				animationController.HoldingBow = value;
				var cam = gameplay.camera;
				if(value) {
					cam.Target = shootingAnchor;
					cam.Distance = cam.shootingDistance.y;
				}
				else {
					ShootTargetPosition = null;
					cam.Target = bodyAnchor;
					cam.Distance = cam.followingDistance;
				}
			}
		}

		public void Shoot() {
			if(!gameplay.Burn(1))
				return;
			shooter.Shoot(ClampedShootTargetPosition);
		}

		public void Dash() {
			if(dashCding)
				return;
			bool moving = walkingVelocity.magnitude > .1f;
			var distance = moving ? movingDash : standingDash;
			distance *= gameplay.speedBonusRate;
			Rigidbody.MovePosition(Rigidbody.position + transform.forward * distance);
			gameplay.Burn(dashConsuming);
			StartCoroutine(EnrollDashCd());
		}

		public void Kick() {
			StartCoroutine(KickingCoroutine());
		}

		public void EndKicking() {
			kicking = false;
		}
		#endregion

		#region Life cycle
		protected new void Start() {
			base.OnStateTransit = OnStateTransit;

			base.Start();

			ShootTargetPosition = null;
			HoldingBow = false;

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