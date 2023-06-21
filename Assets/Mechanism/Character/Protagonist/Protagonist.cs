using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System;

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

		private Vector3 cameraForwardPosition;
		[NonSerialized] public float dashCdProgress = 1;
		#endregion

		#region Serialized fields
		public PixelCrushers.DialogueSystem.ProximitySelector selector;
		[Min(1)] public float cheatingSpeedMultiplier;

		[Header("Anchors")]
		public Transform bodyAnchor;
		public Transform shootingAnchor;

		[Header("Shooting")]
		public Shooter shooter;
		[MinMaxSlider(1, 200)] public Vector2 shootingRange;
		public RectTransform shootingUi;
		public LineRenderer lineRenderer;
		public LayerMask shootingLayerMask;
		[Range(0, 1)] public float holdingMovementSpeedDebuff;

		[Header("Death")]
		[Range(1, 100)] public float deathBurnSpeed;
		[Range(0, 5)] public float deathTime;

		[Header("Dash")]
		[Tooltip("Dash һ�����ĵ�ȼ�ջ�����")]
		[Min(0)] public float dashConsuming;
		[Tooltip("�ƶ��� dash ����")]
		[Range(0, 10)] public float movingDash;
		[Tooltip("��ֹʱ dash ����")]
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
			// Ray ray = cam.ScreenPointToRay(gameplay.input.MousePosition);
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
			Physics.Raycast(ray, out hit, Mathf.Infinity, shootingLayerMask);
		
			cameraForwardPosition = cam.transform.position + cam.transform.forward * shootingRange.y * 3.0f;
			if(hit.transform) {
				ShootTargetPosition = hit.point;
			}
			else {
				ShootTargetPosition = cameraForwardPosition;
			}
			
			


			// If charged-up, render expected shooting curve
			if(ChargeUpValue > 0) {
				var points = YieldShootingCurveCoordinates(shooter.totalTime).ToArray();
				lineRenderer.positionCount = points.Length;
				lineRenderer.SetPositions(points);
				lineRenderer.enabled = true;
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

		IEnumerator DashingCoroutine() {
			bool moving = walkingVelocity.magnitude > .1f;
			var distance = moving ? movingDash : standingDash;
			distance *= gameplay.speedBonusRate;

			Vector3 direction = transform.forward;
			float eps = -.1f;
			Vector3 back = direction * eps;
			CapsuleCollider capsule = GetComponent<CapsuleCollider>();
			if(!capsule) {
				Debug.LogWarning("Collider of protagonist must be a capsule to cast along dashing path!");
				yield break;
			}
			bool castHit = Physics.CapsuleCast(
				capsule.PointAlongAxis(-1) + back, capsule.PointAlongAxis(1) + back,
				capsule.radius, direction, distance - eps, shootingLayerMask,
				QueryTriggerInteraction.Ignore
			);
			if(castHit) {
				Debug.LogWarning("Dashing path is colliding with non-passable collider, aborting dashing!");
				yield break;
			}

			Rigidbody.MovePosition(Rigidbody.position + transform.forward * distance);
			gameplay.Burn(dashConsuming);

			yield return EnrollDashCd();
		}

		IEnumerator EnrollDashCd() {
			dashCding = true;
			dashCdProgress = 0;
			float start = Time.time;
			for(float now; (now = Time.time) - start < dashCd;) {
				dashCdProgress = (now - start) / dashCd;
				yield return new WaitForFixedUpdate();
			}
			dashCdProgress = 1;
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
		public bool ShootingUiVisible {
			get => shootingUi.gameObject.activeInHierarchy;
			set => shootingUi.gameObject.SetActive(value);
		}

		public float ChargeUpSpeed {
			get => chargeUpSpeed;
			set {
				value = Mathf.Clamp01(value);
				chargeUpSpeed = value;
				if(value == 0)
					ChargeUpValue = 0;
			}
		}
		public float ChargeUpValue {
			get => chargeUpValue;
			set {
				value = Mathf.Clamp01(value);
				chargeUpValue = value;
				var cam = gameplay.camera;
				if(value != 0) {
					previousChargeUpValue = value;
					cam.Target = shootingAnchor;
				}
				else {
					cam.Target = bodyAnchor;
				}
				animationController.ChargingUpValue = chargeUpValue;
				if(HoldingBow)
					cam.Distance = cam.shootingDistance.Lerp(1 - value);
				ShootingUiVisible = value > 0;
			}
		}

		public Vector3? ShootTargetPosition {
			get => cachedShootTargetPosition;
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
					cam.Distance = cam.shootingDistance.y;
				}
				else {
					cam.Distance = cam.followingDistance;
				}
			}
		}

		public void Shoot() {
			if(!gameplay.Burn(1))
				return;
			shooter.Shoot(ShootTargetPosition.Value);
			// shooter.Shoot(math.remap(shootingRange.x, shootingRange.y, 0, 1, ChargeUpValue));
		}

		public void Dash() {
			if(dashCding)
				return;
			StartCoroutine(DashingCoroutine());
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

			HoldingBow = false;
			ShootingUiVisible = false;

			shooter.preShoot.AddListener(arrowObj => {
				var arrow = arrowObj.GetComponent<Arrow>();
				if(!arrow)
					throw new UnityException("Passed-in object does not have an arrow component");
				arrow.Tinder = gameplay.currentLanterSlot.tinder;
			});
		}

		protected new void Update() {
			if(state == "Flying") {
				animationController.Freefalling = true;
				Vector3 velocity = gameplay.camera.camera.transform.localToWorldMatrix.MultiplyVector(gameplay.input.rawInputMovement);
				velocity *= movementSettings.walking.speed;
				velocity *= cheatingSpeedMultiplier;
				Rigidbody.velocity = velocity;
			}
			else {
				base.Update();

				lineRenderer.enabled = false;
				if(CanShoot)
					DoShootingFrame();
			}
		}

		protected void FixedUpdate() {
			ChargeUpValue += ChargeUpSpeed * Time.fixedDeltaTime;
			if(ChargeUpValue > 0 && !HoldingBow) {
				Debug.Log("Automatically taking out bow");
				HoldingBow = true;
			}
		}

		protected override void OnDie() {
			base.OnDie();
			StartCoroutine(DyingCoroutine());
		}
		#endregion
	}
}