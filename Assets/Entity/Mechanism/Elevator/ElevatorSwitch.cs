using UnityEngine;
using UnityEngine.Events;
using DSUsable = PixelCrushers.DialogueSystem.Wrappers.Usable;

namespace LanternTrip
{
    [RequireComponent(typeof(Entity))]
    public class ElevatorSwitch : MonoBehaviour
    {
        
        public static ElevatorSwitch current = null;
        public Transform startPosition;
        public Transform endPosition;
        Trigger trigger;
        public Elevator type;

        public UnityEvent onDeleteElevatorSwitch;
        public void DeleteElevatorSwitch() {
            if(!isActiveAndEnabled)
                return;
            if(current != this)
                return;
            onDeleteElevatorSwitch.Invoke();
            Destroy(gameObject);
        }
    }

}