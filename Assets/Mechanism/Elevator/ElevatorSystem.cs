using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace LanternTrip {
	public class ElevatorSystem : MonoBehaviour {
		public Transform startPosition;
		public Transform endPosition;
		private Transform targetPosition;


		[Range(0, 5)] public float moveSpeed;

		public Transform elevatorSwitch;
		public Transform elevatorPosition;
		Rigidbody elevatorRb;
		//Trigger trigger;

		public UnityEvent onDeleteElevatorSwitch;
		public UnityEvent onStartElevatorMove;
		public void DeleteElevatorSwitch() {
			if(!isActiveAndEnabled)
				return;
			onDeleteElevatorSwitch.Invoke();
			Destroy(elevatorSwitch.gameObject);
		}

		public void StartElevatorMove() {
			if(!isActiveAndEnabled)
				return;
			targetPosition = targetPosition == endPosition ? startPosition : endPosition;
			onStartElevatorMove.Invoke();
			StartCoroutine(ElevatorMoveToPosition());
			//Destroy(elevator.gameObject);
		}
		private IEnumerator ElevatorMoveToPosition() {
			Debug.Log(elevatorRb);
			while((elevatorRb.position - targetPosition.position).magnitude > 0.1f) {
				Vector3 moveVector = Vector3.MoveTowards(elevatorRb.position, targetPosition.position, moveSpeed * Time.deltaTime);
				elevatorRb.MovePosition(moveVector);

				yield return new WaitForEndOfFrame();
			}

		} /**/

		void Start() {
			elevatorRb = elevatorPosition.GetComponentInChildren<Rigidbody>();
			elevatorRb.useGravity = false;
			elevatorRb.isKinematic = true;
			elevatorRb.constraints &= ~RigidbodyConstraints.FreezePosition;
		}
	}

}