using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using LanternTrip;
using UnityEngine;

public class Enermy : MonoBehaviour
{
    public float Hp = 100f;

    public bool Invincible = false;

    public GameObject player;
    
    private void Update()
    {
        LookAtPlayer();
    }

    public void LookAtPlayer()
    {
        if (player == null) return;
        var dir = (player.transform.position - transform.position);
        dir.y = 0;
        dir = dir / Vector3.Magnitude(dir);
        var angle = Vector3.Dot(transform.right, dir);
        transform.Rotate(Vector3.up, angle);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Arrow") && !Invincible)
        {
            Hp -= 10f;
        }
    }
}
