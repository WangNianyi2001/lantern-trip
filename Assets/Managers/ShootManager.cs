using UnityEngine;

namespace LanternTrip {
	[RequireComponent(typeof(Camera))]
	public class ShootManager : ManagerBase {
		new Camera camera;
		Transform targetAnchor;

		public GameObject arrowPrefab;
		public GameObject targetPrefab;

		public Vector3? Position {
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
			float speedMin = protagonist.speedRange.x, speedMax = protagonist.speedRange.y;
			float speed = Mathf.Lerp(speedMin, speedMax, gameplay.previousChargeUpValue);
			Vector3 forward = protagonist.transform.forward;
			Vector3 upward = protagonist.transform.up;
			Vector3 velocity = forward * speed + upward * protagonist.verticalSpeed;

			Vector3 outPosition = protagonist.transform.position + forward + upward;

			GameObject arrowObj = Instantiate(arrowPrefab, outPosition, Quaternion.LookRotation(forward, upward));
			Arrow arrow = arrowObj.GetComponent<Arrow>();
			arrow.GetComponent<Rigidbody>().velocity = velocity;
		}

		void Start() {
			camera = GetComponent<Camera>();
			targetAnchor = Instantiate(targetPrefab, transform).transform;
		}

		void FixedUpdate() {
			Ray ray = camera.ScreenPointToRay(gameplay.input.MousePosition);
			RaycastHit hit;
			Physics.Raycast(ray, out hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore);
			Position = hit.collider ? hit.point : null;
			targetAnchor.rotation = Quaternion.identity;
		}

		void OnEnable() {
			//
		}

		void OnDisable() {
			Position = null;
		}
	}
}