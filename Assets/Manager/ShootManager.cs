using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LanternTrip {
	public class ShootManager : ManagerBase {
		static int raycastMask;

		Transform targetAnchor;
		Vector3 outVelocity;
		Vector3 outPosition;

		public new Camera camera;
		public GameObject arrowPrefab;
		public GameObject targetPrefab;
		public LineRenderer lr;

		Vector3 forward => protagonist.transform.forward;
		Vector3 upward => protagonist.transform.up;

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
			GameObject arrowObj = Instantiate(arrowPrefab, outPosition, Quaternion.LookRotation(forward, protagonist.transform.up));
			Arrow arrow = arrowObj.GetComponent<Arrow>();
			arrow.Tinder = gameplay.currentLanterSlot.tinder;
			arrow.GetComponent<Rigidbody>().velocity = outVelocity;
		}

		public IEnumerable<Vector3> CalculateProjectilePositions() {
			float dt = .1f;
			Vector3 pos = outPosition;
			Vector3 vel = outVelocity;
			Vector3 dv = Physics.gravity * dt;
			for(float t = 0; t < 10; t += dt) {
				vel += dv;
				pos += vel * dt;
				if(pos.y < 0)
					break;
				yield return pos;
			}
		}

		void Start() {
			raycastMask = LayerMask.GetMask(new string[] { "Default", "Ground" });
			targetAnchor = Instantiate(targetPrefab, transform).transform;
		}

		void FixedUpdate() {
			Ray ray = camera.ScreenPointToRay(gameplay.input.MousePosition);
			RaycastHit hit;
			Physics.Raycast(ray, out hit, Mathf.Infinity, raycastMask, QueryTriggerInteraction.Ignore);
			TargetPosition = hit.collider ? hit.point : null;
			targetAnchor.rotation = Quaternion.identity;

			// Update velocity
			lr.enabled = protagonist.state == "Shooting";
			if(lr.enabled) {
				float speedMin = protagonist.speedRange.x, speedMax = protagonist.speedRange.y;
				float speed = Mathf.Lerp(speedMin, speedMax, gameplay.previousChargeUpValue);
				outVelocity = forward * speed + upward * protagonist.verticalSpeed * speed * protagonist.shootingAngleRate;
				outPosition = protagonist.transform.position + forward + upward;
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