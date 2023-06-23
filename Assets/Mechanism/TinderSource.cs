using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LanternTrip {
	[RequireComponent(typeof(Entity))]
	[ExecuteAlways]
	public class TinderSource : MonoBehaviour {
		[SerializeField] private Tinder type;
		public Transform tinder;
		public new AudioSource audio;
		public UnityEvent onDeliver;

		enum ConfirmState {
			Idle, Confirming,
		}
		ConfirmState confirmState = ConfirmState.Idle;

		public void ConfirmDelivery() {
			Animation anim = GameplayManager.instance.ui.slotTrack.selector.GetComponentInChildren<Animation>();
			if(anim) {
				anim.Play();
			}
		}

		public void Deliver() {
			if(!isActiveAndEnabled)
				return;

			GameplayManager.instance.LoadTinder(type);
			onDeliver.Invoke();

			tinder.gameObject.SetActive(false);
			audio?.Stop();
			enabled = false;
		}

		public void OnInteract() {
			Deliver();
			confirmState = ConfirmState.Idle;
			return;
			// No need to confirm anymore
			switch(confirmState) {
				case ConfirmState.Idle:
					ConfirmDelivery();
					confirmState = ConfirmState.Confirming;
					break;
				case ConfirmState.Confirming:
					Deliver();
					confirmState = ConfirmState.Idle;
					break;
			}
		}

		public void OnLeave() {
			confirmState = ConfirmState.Idle;
		}

		public Tinder Type {
			get => type;
			set {
				if(tinder) {
					var renderer = tinder?.GetComponentInChildren<MeshRenderer>();
					if(renderer) {
						var material = renderer.sharedMaterial;
						if(material == null)
							material = renderer.sharedMaterial = new Material(Shader.Find("HDRP/Lit"));
						else if(AssetDatabase.Contains(material))
							material = renderer.sharedMaterial = new Material(material);
						Color color = type?.mainColor ?? Color.white;
						material.color = color;
					}
				}
			}
		}

#if UNITY_EDITOR
		void EditorUpdate() {
			Type = Type;
			if(audio)
				audio.playOnAwake = !!tinder;
		}
#endif

		void Update() {
#if UNITY_EDITOR
			if(!Application.isPlaying) {
				EditorUpdate();
				return;
			}
#endif
		}
	}
}