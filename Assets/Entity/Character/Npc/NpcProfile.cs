using UnityEngine;
using NaughtyAttributes;

namespace LanternTrip {
	[CreateAssetMenu(menuName = "LanternTrip/NpcProfile")]
	public class NpcProfile : ScriptableObject {
		new public string name;
		public bool undead;
		[HideIf("undead")] public int hp;
		public bool isEnemy;
		[ShowIf("isEnemy")] public Tinder.Type type;
	}
}
