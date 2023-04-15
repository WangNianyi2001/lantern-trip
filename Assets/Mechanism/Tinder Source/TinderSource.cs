using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LanternTrip {
	[RequireComponent(typeof(Entity))]
	[ExecuteAlways]
	public class TinderSource : MonoBehaviour {
		/// <summary>Temporary solution for getting current tinder.</summary>
		/// <remarks>Might be changed in the future.</remarks>
		public static TinderSource current = null;

		public Tinder type;
		public Transform tinder;
		public UnityEvent onApproach;
		public UnityEvent onLeave;
		public UnityEvent onDeliver;

		public void OnApproach() {
			if(!isActiveAndEnabled)
				return;

			onApproach.Invoke();
			current = this;
		}
		public void OnLeave() {
			if(!isActiveAndEnabled)
				return;

			onLeave.Invoke();
			current = null;
		}
		public void Deliver() {
			if(!isActiveAndEnabled)
				return;
			if(current != this)
				return;

			GameplayManager.instance.LoadTinder(type);
			onDeliver.Invoke();

			current = null;
			tinder.gameObject.SetActive(false);
		}

#if UNITY_EDITOR
		void EditprUpdateTinder() {
			var renderer = tinder?.GetComponentInChildren<MeshRenderer>();
			if(renderer) {
				var material = renderer.sharedMaterial;
				if(material == null)
					material = renderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
				else if(AssetDatabase.Contains(material))
					material = renderer.sharedMaterial = new Material(material);
				Color color = type?.mainColor ?? Color.white;
				material.color = color;
			}
		}
		void EditorUpdate() {
			if(tinder) {
				EditprUpdateTinder();
			}
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