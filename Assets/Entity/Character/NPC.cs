namespace LanternTrip {
	public class Npc : Character {
		public int HP = 100;
		public Tinder.Type type = Tinder.Type.Red;

		public void GainDamage(Tinder.Type type) {
			if(this.type == type)
				HP -= 100;
			else
				HP -= 50;

			if(HP <= 0)
				Die();
		}

		public void Die() {
			//
		}
	}
}
