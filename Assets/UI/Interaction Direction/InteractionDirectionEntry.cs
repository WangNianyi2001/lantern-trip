using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace LanternTrip {
	public class InteractionDirectionEntry : MonoBehaviour {
		[SerializeField] Text key;
		[SerializeField] Text content;

		public Key Key {
			set => key.text = typeof(Key).GetEnumName(value).Substring(0, 1);
		}
		public string Content {
			get => content.text;
			set => content.text = value;
		}
	}
}