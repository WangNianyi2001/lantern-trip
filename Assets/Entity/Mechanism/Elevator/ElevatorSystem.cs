using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DSUsable = PixelCrushers.DialogueSystem.Wrappers.Usable;

namespace LanternTrip
{
    [RequireComponent(typeof(Entity))]
    public class ElevatorSystem : MonoBehaviour
    {
        
        //public static ElevatorSwitch current = null;
        public Transform startPosition;
        public Transform endPosition;
        private Transform targetPosition;
        
        public Transform elevatorSwitch;
        public Transform elevatorPosition;
        Trigger trigger;
        public Elevator type;

        public UnityEvent onDeleteElevatorSwitch;
        public UnityEvent onStartElevatorMove;
        public void DeleteElevatorSwitch() {
            if(!isActiveAndEnabled)
                return;
            onDeleteElevatorSwitch.Invoke();
            Destroy(elevatorSwitch.gameObject);
        }

        public void StartElevatorMove()
        {
            if(!isActiveAndEnabled)
                return;
            targetPosition = endPosition;
            onStartElevatorMove.Invoke();
            StartCoroutine(ElevatorMoveToPosition());
            //Destroy(elevator.gameObject);
        }
        private IEnumerator ElevatorMoveToPosition()
        {
            
            while (elevatorPosition != targetPosition)
            {
                elevatorPosition.localPosition = Vector3.MoveTowards(elevatorPosition.localPosition, endPosition.localPosition, 10 * Time.deltaTime);
                yield return 0;
            }
            
        }
    }

}