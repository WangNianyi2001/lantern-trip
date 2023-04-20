using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderZone : MonoBehaviour
{
    public UnityEvent onEnterEvent; 
    public UnityEvent onExitEvent;
    public GameObject targetObject;

    public enum tagType { Player, Arrow, MainCamera};
    public tagType TagType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagType.ToString()))
        {
            onEnterEvent.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onExitEvent.Invoke();
        }
    }
}


 
