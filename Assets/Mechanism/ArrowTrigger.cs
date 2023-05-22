using UnityEngine;
using UnityEngine.Events;

namespace LanternTrip {
	[ExecuteInEditMode]
	public class ArrowTrigger : MonoBehaviour {
		public Tinder type;
		public Transform ball;
		public Entity target;
		public UnityEvent onMatchShot;

		public void OnMatchedShot() => onMatchShot?.Invoke();

		protected void Start() {
			if(!Application.isPlaying)
				return;
			target.shotType = type?.type ?? Tinder.Type.Invalid;
		}

		protected void EditorUpdate() {
			var color = type?.mainColor ?? Color.grey;

			if(ball) {
				var renderer = ball.GetComponent<Renderer>();
				if(renderer?.sharedMaterial) {
					var newMat = new Material(renderer.sharedMaterial);
					newMat.color = color;
					renderer.sharedMaterial = newMat;
				}
			}
			if(target) {
				target.shotType = type?.type ?? Tinder.Type.Invalid;
			}
		}

		protected void Update() {
			if(!Application.isPlaying) {
				EditorUpdate();
				return;
			}
		}
	}
}