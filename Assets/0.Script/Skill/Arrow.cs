using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace LanternTrip
{
    public class Arrow : MonoBehaviour
    {
        public TinderType element;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            (enemy != null).Do(_ =>
            {
                CharacterManager.Instance.Attack(element, enemy);
            });
        }
    }
}
