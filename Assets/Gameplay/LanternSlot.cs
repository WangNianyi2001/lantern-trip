namespace LanternTrip {
	public class LanternSlot {
		Tinder _tinder = null;
		float _timeLeft = 0;

		public readonly LanternSlotUI ui;
		public LanternSlot(LanternSlotUI ui) {
			this.ui = ui;
		}

		public Tinder tinder {
			get => _tinder;
			set {
				_tinder = value;
				ui.Tinder = value;
			}
		}

		public float timeLeft {
			get => _timeLeft;
			set {
				_timeLeft = value;
				ui.SetValue(value);
			}
		}

		public bool Load(Tinder tinder, bool force = false) {
			if(!force) {
				if(tinder != null)
					return false;
			}
			this.tinder = tinder;
			timeLeft = tinder.timeSpan;
			return true;
		}

		/// <summary>Burn tinder in this slot by certain amount of time.</summary>
		/// <returns>`true` if succeed, `false` if slot is empty or tinder exhausted.</returns>
		public bool Burn(float time) {
			if(tinder == null)
				return false;
			timeLeft -= time;
			if(timeLeft <= 0) {
				timeLeft = 0;
				tinder = null;
				return false;
			}
			return true;
		}
	}
}