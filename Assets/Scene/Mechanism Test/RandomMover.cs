using UnityEngine;

public class RandomMover : MonoBehaviour {
	private Vector3 originalPosition;
	private Vector3 velocity;
	private Quaternion angularVelocity;

	[Range(0, 1)] public float centeringStrength;
	[Range(0, 10)] public float strength;

	[Range(0, 1)] public float angularCenteringStrength;
	[Range(0, 10)] public float angularStrength;

	protected void Start() {
		originalPosition = transform.position;
		angularVelocity = Quaternion.identity;
	}
	protected void FixedUpdate() {
		Vector3 force = Random.insideUnitSphere;
		force *= strength;
		force += (originalPosition - transform.position) * centeringStrength;

		Quaternion angularForce = Random.rotation;
		angularForce = Quaternion.Slerp(Quaternion.identity, angularForce, angularStrength);
		angularForce *= Quaternion.Slerp(Quaternion.identity, Quaternion.Inverse(angularVelocity), angularCenteringStrength);

		velocity += force * Time.fixedDeltaTime;
		angularVelocity *= Quaternion.Slerp(Quaternion.identity, angularForce, Time.fixedDeltaTime);

		transform.position += velocity * Time.fixedDeltaTime;
		transform.rotation *= Quaternion.Slerp(Quaternion.identity, angularVelocity, Time.fixedDeltaTime * 10);
	}
}
