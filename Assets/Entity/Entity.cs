using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

namespace LanternTrip {
	[RequireComponent(typeof(Rigidbody))]
	public partial class Entity : MonoBehaviour {
		#region Core members
		protected new Rigidbody rigidbody;
		protected Dictionary<Collider, ContactPoint> contactingPoints = new Dictionary<Collider, ContactPoint>();
		private float hp;
		private bool undead;
		#endregion

		#region Public members
		public float damageMultiplier = 1;
		public Tinder.Type shotType = Tinder.Type.Invalid;
		public UnityEvent onDie;
		public UnityEvent onShot;
		public UnityEvent onMatchedShot;
		#endregion

		#region Public interfaces
		public IEnumerable<ContactPoint> ContactingPoints => contactingPoints.Values;

		public float Hp {
			get => Undead ? hp : Mathf.Infinity;
			set {
				hp = value;
				if(Undead)
					return;
				if(hp <= 0) {
					hp = 0;
					Die();
				}
			}
		}

		public bool Undead {
			get => undead;
			set => undead = value;
		}

		public void TakeDamage(float amount) => Hp -= amount;

		public void Shot(Arrow arrow) {
			float damage = damageMultiplier;
			if(arrow.Tinder?.type == shotType)
				damage *= 2;
			TakeDamage(damage);
			onShot?.Invoke();
			if(arrow.Tinder?.type == shotType)
				onMatchedShot?.Invoke();
		}

		public void Die() {
			onDie?.Invoke();
		}
		#endregion

		#region Private method
		void UpdateCollision(Collision collision) {
			ContactPoint lowest = collision.contacts.Aggregate((a, b) => a.point.y < b.point.y ? a : b);
			contactingPoints[collision.collider] = lowest;
		}
		#endregion

		#region Life cycle
		protected void Start() {
			// Get component references
			rigidbody = GetComponent<Rigidbody>();
			rigidbody.isKinematic = false;
			rigidbody.useGravity = true;

			// Initialize
			contactingPoints = new Dictionary<Collider, ContactPoint>();
		}

		protected void OnCollisionEnter(Collision collision) {
			UpdateCollision(collision);
		}

		protected void OnCollisionStay(Collision collision) {
			UpdateCollision(collision);
		}

		protected void OnCollisionExit(Collision collision) {
			contactingPoints.Remove(collision.collider);
		}
		#endregion
	}
}