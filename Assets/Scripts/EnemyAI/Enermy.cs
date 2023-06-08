using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using LanternTrip;
using UniRx;
using UnityEngine;

public class Enermy : MonoBehaviour
{
    public float fullHp = 100f;
    public FloatReactiveProperty curHp;

    
    public bool Invincible = false;

    public GameObject player;

    public Tinder.Type tinderType;
    
    
    private void Start()
    {
        curHp.Value = fullHp;
        curHp.Subscribe(value =>
        {
            if (value<=0)
            {
                //TODO:
                GameObject.Destroy(gameObject);
                Debug.Log("死亡");
                
            }
        });
    }

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
            if (tinderType == collision.collider.GetComponent<Arrow>().Tinder.type)
            {
                curHp.Value -= 35f;
            }
            else
            {
                
                curHp.Value -= 18f;
            }
        }
    }
}
