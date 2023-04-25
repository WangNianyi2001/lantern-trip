using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlObjectRotation : MonoBehaviour
{
    private bool isRotating = false;
    [SerializeField] private float rotationAngle = 90f;
    [SerializeField] private float rotationTime = 1f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private bool RotateAroundOther = false;
    public GameObject targetObject;
    

    public void RotateObject()
    {
        if (!isRotating)
        {
            switch(RotateAroundOther)
            {
                case false:
                StartCoroutine(RotateCoroutine());
                    break;
                    case true:
                    StartCoroutine(RotateAroundOtherCoroutine());
                    break;   
                        }
            ;
        }
    }

    private IEnumerator RotateCoroutine()
    {
        isRotating = true;
        float elapsedTime = 0f;
        Quaternion startingRotation = transform.rotation;
        Quaternion targetRotation = transform.rotation * Quaternion.AngleAxis(rotationAngle, rotationAxis);

        while (elapsedTime < rotationTime)
        {
            transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime / rotationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //transform.rotation = targetRotation;
        isRotating = false;
    }

    private IEnumerator RotateAroundOtherCoroutine()
    {
        isRotating = true;
        Vector3 targetPosition = targetObject.transform.position;
        Quaternion startRotation = transform.rotation;
        //Quaternion endRotation = Quaternion.AngleAxis(rotationAngle, rotationAxis) * startRotation;
        float t = 0f;
        while (t < rotationTime)
        {
            t += Time.deltaTime;
            transform.RotateAround(targetPosition, rotationAxis, rotationAngle * Time.deltaTime / rotationTime);
            yield return null;
        }
        //transform.rotation = endRotation;
        isRotating = false;

    }

}
   

