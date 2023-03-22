using UnityEngine;

namespace LanternTrip {
	[RequireComponent(typeof(Camera))]
	public class ShootManager : ManagerBase {
		new Camera camera;
		Transform targetAnchor;

		public GameObject arrowPrefab;
		public GameObject targetPrefab;

		public Vector3? Position {
			get => targetAnchor.gameObject.active ? targetAnchor.position : null;
			set {
				if(!value.HasValue) {
					targetAnchor.gameObject.SetActive(false);
					return;
				}
				targetAnchor.gameObject.SetActive(true);
				targetAnchor.position = value.Value;
			}
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