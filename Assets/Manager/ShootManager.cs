using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LanternTrip {
	public class ShootManager : ManagerBase {
		static int raycastMask;

		Transform targetAnchor;
		Vector3 outVelocity;

		public ShootingSettings settings;
		public new Camera camera;
		public GameObject arrowPrefab;
		public GameObject targetPrefab;
		public LineRenderer lr;

		Vector3 OutPosition => protagonist.transform.localToWorldMatrix.MultiplyPoint(settings.outPosition);
		Vector3 CalculateOutVelocity() {
			Vector3 delta = OutPosition - TargetPosition.Value;

			float dz = Mathf.Lerp(settings.range.x, settings.range.y, gameplay.previousChargeUpValue), dy = delta.y;

			float g = Physics.gravity.magnitude;
			float tMax = settings.maxTime;
			float sMax = settings.maxSlope;

			float s1 = (g / 2 * Mathf.Pow(tMax, 2) - dy) / dz;

			float s, t;
			if(sMax < s1) {
				s = sMax;
				float temp = sMax * dz + dy;    // Might be negative
				if(temp <= 0)
					return outVelocity;
				t = Mathf.Sqrt(2 * temp / g);
			}
			else {
				s = s1;
				t = tMax;
			}

			float vz = dz / t, vy = s * vz;

			Vector3 res = new Vector3(0, vy, vz) * tMax;
			return protagonist.transform.localToWorldMatrix.MultiplyVector(res);
		}

		public Vector3? TargetPosition {
			get => targetAnchor.gameObject.activeSelf ? targetAnchor.position : null;
			set {
				if(!value.HasValue) {
					targetAnchor.gameObject.SetActive(false);
					return;
				}
				targetAnchor.gameObject.SetActive(true);
				targetAnchor.position = value.Value;
			}
		}

		public void MakeShoot() {
			GameObject arrowObj = Instantiate(
				arrowPrefab, OutPosition,
				Quaternion.LookRotation(protagonist.transform.forward, protagonist.transform.up)
			);
			Arrow arrow = arrowObj.GetComponent<Arrow>();
			arrow.Tinder = gameplay.currentLanterSlot.tinder;
			arrow.GetComponent<Rigidbody>().velocity = outVelocity;
		}

		public IEnumerable<Vector3> CalculateProjectilePositions() {
			float dt = .05f;
			Vector3 pos = OutPosition;
			Vector3 vel = outVelocity;
			Vector3 dv = Physics.gravity * dt;
			for(float t = 0; t < settings.maxTime; t += dt) {
				vel += dv;
				pos += vel * dt;
				yield return pos;
				if(pos.y < 0)
					break;
			}
		}

		void Start() {
			raycastMask = LayerMask.GetMask(new string[] { "Default", "Ground" });
			targetAnchor = Instantiate(targetPrefab, transform).transform;
		}

		void Update() {
			Ray ray = camera.ScreenPointToRay(gameplay.input.MousePosition);
			RaycastHit hit;
			Physics.Raycast(ray, out hit, Mathf.Infinity, raycastMask, QueryTriggerInteraction.Ignore);
			TargetPosition = hit.collider ? hit.point : null;
			targetAnchor.rotation = Quaternion.identity;

			// Update velocity
			var isAiming = protagonist.state == "Shooting";
			lr.enabled = isAiming;
			if(isAiming) {
				outVelocity = CalculateOutVelocity();
				Vector3[] positions = CalculateProjectilePositions().ToArray();
				lr.positionCount = positions.Length;
				lr.SetPositions(positions);
			}
		}

		void OnDisable() {
			TargetPosition = null;
		}
	}
}