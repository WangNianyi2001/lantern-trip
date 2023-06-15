using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace LanternTrip {
	[ExecuteInEditMode]
	public partial class ArrowTrigger : MonoBehaviour {
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

		protected void Update() {
#if UNITY_EDITOR
			if(!Application.isPlaying) {
				EditorUpdate();
				return;
			}
#endif
		}
	}
}