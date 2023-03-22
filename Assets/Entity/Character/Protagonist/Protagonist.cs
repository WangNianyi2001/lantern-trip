namespace LanternTrip {
	public class Protagonist : Character {
		public bool HoldingBow {
			get => animationController.HoldingBow;
			set => animationController.HoldingBow = value;
		}
	}
}