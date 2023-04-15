using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlObjectRotation : MonoBehaviour
{
    private bool isRotating = false;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float rotationTime = 1f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    public void RotateObject()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateCoroutine());
        }
    }

    private IEnumerator RotateCoroutine()
    {
        isRotating = true;
        float elapsedTime = 0f;
        Quaternion startingRotation = transform.rotation;
        Quaternion targetRotation = transform.rotation * Quaternion.AngleAxis(rotationSpeed, rotationAxis);

        while (elapsedTime < rotationTime)
        {
            transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime / rotationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        isRotating = false;
    }
}
   

