using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace LanternTrip {
	public class InteractionDirectionEntry : MonoBehaviour {
		[SerializeField] TextMeshProUGUI key;
		[SerializeField] TextMeshProUGUI content;

		public Key Key {
			set => key.text = typeof(Key).GetEnumName(value).Substring(0, 1);
		}
		public string Content {
			get => content.text;
			set => content.text = value;
		}
	}
}