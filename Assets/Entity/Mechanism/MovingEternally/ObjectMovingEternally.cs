using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovingEternally : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform startPosition;
    public Transform endPosition;
    private Transform targetPosition;
    public Transform objectToMove;
    [Range(0, 5)] public float waitTime;
    public float timer = 0;
    [Range(0, 5)] public float moveSpeed;
    void Start()
    {
        targetPosition = endPosition;
        moveSpeed = 0.3f;
        waitTime = 1.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = Vector3.Magnitude((objectToMove.position - targetPosition.position));
        if (distance > 0.1f)//move
        {
            Move();          
        }
        else if(timer<waitTime)//Suspend
        {
            Suspend();
        }

        else
        {
            timer = 0;
            targetPosition = targetPosition==endPosition? startPosition:endPosition;
        }
    }

    void Move()
    {
        objectToMove.localPosition = Vector3.MoveTowards(objectToMove.localPosition, targetPosition.localPosition, moveSpeed * Time.deltaTime);

    }

    void Suspend()
    {
        timer += Time.deltaTime;
    }
}
