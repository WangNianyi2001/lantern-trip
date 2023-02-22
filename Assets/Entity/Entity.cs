using UnityEngine;

namespace LanternTrip {
	[RequireComponent(typeof(Rigidbody))]
	public class Entity : MonoBehaviour {
		#region Core members
		protected new Rigidbody rigidbody;
		#endregion

		#region Life cycle
		protected void Start() {
			// Get component references
			rigidbody = GetComponent<Rigidbody>();
			rigidbody.isKinematic = false;
			rigidbody.useGravity = true;
		}
		#endregion
	}
}