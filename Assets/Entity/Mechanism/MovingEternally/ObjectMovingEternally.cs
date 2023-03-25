using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LanternTrip
{

    public class ObjectMovingEternally : MonoBehaviour
    {
        // Start is called before the first frame update
        public Transform startPosition;
        public Transform endPosition;
        private Transform targetPosition;
        public Entity objectToMove;
        [Range(0, 5)] public float waitTime;
        public float timer = 0;
        [Range(0, 20)] public float moveSpeed;

        void Start()
        {
            targetPosition = endPosition;
            //moveSpeed = 10.0f;
            //waitTime = 1.0f;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            float distance = Vector3.Magnitude((objectToMove.transform.position - targetPosition.position));
            if (distance > 0.1f) //move
            {
                Move();
            }
            else if (timer < waitTime) //Suspend
            {
                Suspend();
            }

            else
            {
                timer = 0;
                targetPosition = targetPosition == endPosition ? startPosition : endPosition;
            }
        }

        void Move()
        {
            var moveTowards = Vector3.MoveTowards(objectToMove.transform.position, targetPosition.position,
                moveSpeed * Time.deltaTime);
            objectToMove.Rigidbody.MovePosition(moveTowards);
        }

        void Suspend()
        {
            timer += Time.deltaTime;
        }
    }
}