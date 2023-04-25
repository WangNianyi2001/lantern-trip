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
		private bool dead = false;
		#endregion

		#region Serialized members
		public float damageMultiplier = 1;
		public Tinder.Type shotType = Tinder.Type.Invalid;
		public UnityEvent onDie;
		public UnityEvent onShot;
		public UnityEvent onMatchedShot;
		public float deathY = -100;
		#endregion

		#region Public interfaces
		public Rigidbody Rigidbody => rigidbody;
		public IEnumerable<ContactPoint> ContactingPoints => contactingPoints.Values;

		public float Hp {
			get => Undead ? hp : Mathf.Infinity;
			set {
				hp = value;
				if(Undead)
					return;
				if(hp <= 0) {
					hp = 0;
					OnDie();
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
		#endregion

		#region Private method
		void UpdateCollision(Collision collision) {
			ContactPoint lowest = collision.contacts.Aggregate((a, b) => a.point.y < b.point.y ? a : b);
			contactingPoints[collision.collider] = lowest;
		}
		#endregion

		#region Life cycle
		protected void Start() {
			rigidbody = GetComponent<Rigidbody>();
			contactingPoints = new Dictionary<Collider, ContactPoint>();
		}

		protected void Update() {
			if(!dead) {
				if(transform.position.y < deathY)
					OnDie();
			}
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

		protected virtual void OnDie() {
			dead = true;
			onDie?.Invoke();
		}
		#endregion
	}
}