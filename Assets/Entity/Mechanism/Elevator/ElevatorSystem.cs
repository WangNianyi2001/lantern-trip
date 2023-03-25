using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using DSUsable = PixelCrushers.DialogueSystem.Wrappers.Usable;

namespace LanternTrip
{
    [RequireComponent(typeof(Entity))]
    public class ElevatorSystem : MonoBehaviour
    {
        public Transform startPosition;
        public Transform endPosition;
        private Transform targetPosition;
        
        
        [Range(0, 5)] public float moveSpeed;
        
        public Transform elevatorSwitch;
        public Transform elevatorPosition;
        //Trigger trigger;

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
            targetPosition = targetPosition==endPosition? startPosition:endPosition;
            onStartElevatorMove.Invoke();
            StartCoroutine(ElevatorMoveToPosition());
            //Destroy(elevator.gameObject);
        }
       private IEnumerator ElevatorMoveToPosition()
        {
            
            while (Vector3.Magnitude( (elevatorPosition.position - targetPosition.position)) > 0.1f)
            {
                elevatorPosition.localPosition = Vector3.MoveTowards(elevatorPosition.localPosition, targetPosition.localPosition, moveSpeed * Time.deltaTime);
                
                yield return 0;
            }
            
        } /**/
    }

}