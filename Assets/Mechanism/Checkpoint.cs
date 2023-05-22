using UnityEngine;

namespace LanternTrip {
	public class Checkpoint : MonoBehaviour {
		protected GameplayManager gameplay => GameplayManager.instance;

		/// <summary>����Ϸ��ԭΪ checkpoint �趨�ĳ�ʼ״̬</summary>
		public void Restore() {
			if(!GameplayManager.instance.protagonist)
				return;
			Transform pt = GameplayManager.instance.protagonist.transform;
			pt.position = transform.position;
			pt.rotation = transform.rotation;
		}
	}
}