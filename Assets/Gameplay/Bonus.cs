using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;

namespace LanternTrip {
	[System.Serializable]
	public class Bonus {
		public enum Type { AllDifferent, AllSame }
		public Type type;
		public Tinder.Type tinderType;
		public bool immediate;

		[ShowIf("immediate")] public UnityEvent onGrant;
		[HideIf("immediate")] public UnityEvent onActivate;
		[HideIf("immediate")] public UnityEvent onDeactivate;

		public bool Check(IEnumerable<Tinder.Type> types) {
			if(types == null)
				return false;
			if(types.Any(type => type == Tinder.Type.Invalid))
				return false;
			int andMask = types.Select(type => (int)type).Aggregate((a, b) => a & b);
			switch(type) {
				case Type.AllDifferent:
					return andMask == 0;
				case Type.AllSame:
					return andMask != 0 && types.First() == tinderType;
			}
			return false;
		}
	}
}