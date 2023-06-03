using System;
using System.Collections;
using System.Collections.Generic;
using LanternTrip;
using UnityEngine;

public class Enermy : MonoBehaviour
{
    public float Hp = 100f;

    public bool Invincible = false;
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Arrow") && !Invincible)
        {
            Hp -= 10f;
        }
    }
}
