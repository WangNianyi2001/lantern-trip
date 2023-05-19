using UnityEngine;

namespace LanternTrip {
	public class Checkpoint : MonoBehaviour {
		protected GameplayManager gameplay => GameplayManager.instance;

		/// <summary>将游戏还原为 checkpoint 设定的初始状态</summary>
		public void Restore() {
			if(!GameplayManager.instance.protagonist)
				return;
			Transform pt = GameplayManager.instance.protagonist.transform;
			pt.position = transform.position;
			pt.rotation = transform.rotation;
		}
	}
}