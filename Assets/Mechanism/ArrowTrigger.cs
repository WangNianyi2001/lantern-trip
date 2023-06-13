using UnityEngine;
using UnityEngine.Events;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LanternTrip {
	[ExecuteInEditMode]
	public class ArrowTrigger : MonoBehaviour {
		public Tinder type;
		public Transform ball;
		public Entity target;
		public UnityEvent onMatchShot;
		public static float delay = .2f;

		protected IEnumerator OnMatchedShotCoroutine() {
			yield return new WaitForSeconds(delay);
			onMatchShot?.Invoke();
		}

		public void OnMatchedShot() => StartCoroutine(OnMatchedShotCoroutine());

		protected void Start() {
			if(!Application.isPlaying)
				return;
			target.shotType = type?.type ?? Tinder.Type.Invalid;
		}

		protected void EditorUpdate() {
#if UNITY_EDITOR
			var color = type?.mainColor ?? Color.grey;

			if(ball) {
				var renderer = ball.GetComponent<Renderer>();
				if(renderer?.sharedMaterial) {
					var guids = AssetDatabase.FindAssets("t:GameSettings");
					if(guids.Length > 0) {
						var path = AssetDatabase.GUIDToAssetPath(guids[0]);
						var settings = AssetDatabase.LoadAssetAtPath<GameSettings>(path);
						var newMat = new Material(settings.arrowTriggerMaterial);
						newMat.color = color;
						renderer.sharedMaterial = newMat;
					}
				}
			}
			if(target) {
				target.shotType = type?.type ?? Tinder.Type.Invalid;
			}
#endif
		}

		protected void Update() {
			if(!Application.isPlaying) {
				EditorUpdate();
				return;
			}
		}
	}
}