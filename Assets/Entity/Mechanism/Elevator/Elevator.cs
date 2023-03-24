using UnityEngine;

namespace LanternTrip {
    [CreateAssetMenu(menuName = "LanternTrip/Elevator")]
    public class Elevator : ScriptableObject {
        public enum Type {
            Invalid = 0,
            Red = 1,
            Green = 2,
            Blue = 4
        }
        public Type type;
    }
}
