namespace LanternTrip {
	public class Enemy : Npc {
		public void TakeArrowDamage(Arrow arrow) {
			int whole = profile.hp;
			int half = (profile.hp + 1) / 2;
			int damage = arrow.Tinder?.type == profile.type ? whole : half;
			TakeDamage(damage);
		}
	}
}