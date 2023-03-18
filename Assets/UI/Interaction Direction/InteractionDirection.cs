using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LanternTrip {
	[RequireComponent(typeof(Canvas))]
	public class InteractionDirection : MonoBehaviour {
		[Serializable]
		public struct InspectorAssembly {
			public Key key;
			public string content;
		}

		#region Inspector fields
		public List<InspectorAssembly> entryList;
		public LayoutGroup layout;
		#endregion

		#region Public interfaces
		public void AddEntry(Key key, string content) {
			GameObject prefab = GameplayManager.instance.ui.interactionDirectionEntryPrefab;
			GameObject obj = Instantiate(prefab, layout.transform);
			var entry = obj.GetComponentInChildren<InteractionDirectionEntry>();
			entry.Key = key;
			entry.Content = content;
			layout.CalculateLayoutInputVertical();
		}

		public void Show() {
			gameObject.SetActive(true);
		}

		public void Hide() {
			gameObject.SetActive(false);
		}
		#endregion

		#region Life cycle
		void Start() {
			foreach(var t in layout.GetComponentsInChildren<InteractionDirectionEntry>())
				Destroy(t.gameObject);
			foreach(var assembly in entryList)
				AddEntry(assembly.key, assembly.content);
			Hide();
		}
		#endregion
	}
}