namespace LanternTrip {
	public class Enemy : Npc {
		protected new void Start() {
			base.Start();
			damageMultiplier = profile.hp / 2 * 1.01f;
		}
	}
}