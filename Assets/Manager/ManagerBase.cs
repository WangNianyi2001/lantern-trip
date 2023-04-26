using UnityEngine;

namespace LanternTrip {
	public class ManagerBase : MonoBehaviour {
		public GameplayManager gameplay {
			get {
				if(Application.isPlaying)
					return GameplayManager.instance;
				return FindObjectOfType<GameplayManager>();
			}
		}
		public Protagonist protagonist => gameplay?.protagonist;
	}
}