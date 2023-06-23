using UnityEngine;
using UnityEngine.UI;

namespace LanternTrip {
	public class HpUi : MonoBehaviour {
		public Image bar;

		protected GameplayManager Gameplay => GameplayManager.instance;
		protected Protagonist Protagonist => Gameplay?.protagonist;
		protected GameSettings Settings => Gameplay?.settings;

		public float Percentage => Protagonist == null ? 0 : Protagonist.Hp / Settings.protagonistInitialHp;

		protected void Update() {
			bar.fillAmount = Percentage;
		}
	}
}