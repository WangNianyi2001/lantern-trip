using System;
using System.Collections;
using System.Collections.Generic;
using LanternTrip;
using UnityEngine;

public class Car : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var protagonist = other.GetComponent<Protagonist>();
            if (protagonist == null)
            {
                Debug.LogError("Protagonist is null");
                return;
            }

            var rb = protagonist.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }
}
