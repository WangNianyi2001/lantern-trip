using UnityEngine;

namespace LanternTrip {
	public class ManagerBase : MonoBehaviour {
		public GameplayManager gameplay => GameplayManager.instance;
		public Protagonist protagonist => gameplay?.protagonist;
	}
}