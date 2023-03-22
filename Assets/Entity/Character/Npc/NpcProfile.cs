using UnityEngine;

namespace LanternTrip {
	[CreateAssetMenu(menuName = "LanternTrip/NpcProfile")]
	public class NpcProfile : ScriptableObject {
		new public string name;
		public int hp;
		public bool isEnemy;
		[NaughtyAttributes.ShowIf("isEnemy")] public Tinder.Type type;
	}
}
