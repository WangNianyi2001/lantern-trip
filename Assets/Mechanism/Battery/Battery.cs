using System;
using System.Collections;
using System.Collections.Generic;
using LanternTrip;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Battery : MonoBehaviour
{
    public GameObject bullet;

    [Header("炮弹等级")]public int level = 1;

    [Header("炮弹速度")]public float BulletSpeed = 1.0f;
    
    [Header("发射时间间隔")]public float TimeInterval = 0.5f;

    private float cur_TimeInterval;

    [Header("炮弹销毁时间")]public float TimeToDestroy = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        cur_TimeInterval = TimeInterval;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        
        // transform.LookAt(other.transform);
        // transform.eulerAngles = new Vector3(0f
        //     , transform.eulerAngles.y
        //     , 0f);

        cur_TimeInterval -= Time.deltaTime;
        if (cur_TimeInterval > 0f)
        {
            return;
        }
        cur_TimeInterval = TimeInterval;
        var go = GameObject.Instantiate(bullet);
        go.transform.position = transform.position;

        Vector3 dir;
        switch (level)
        {
            // 1级炮台，朝着主角的方向射击
            case 1:
                dir = other.transform.position - transform.position;
                break;
            // 0级炮台：朝着子物体的方向射击
            case 0:
                dir = transform.GetChild(0).position - transform.position;
                break;
            default:
                dir = other.transform.position - transform.position;
                break;
        }

        dir.y = 0;

        var angle = Vector3.Dot(dir, Vector3.right);
        transform.rotation = Quaternion.identity;
        transform.Rotate(Vector3.up, angle);

        var cur_TimeToDestroy = TimeToDestroy;

        
        go.UpdateAsObservable()
            .Subscribe(_ =>
            {
                go.transform.Translate(dir * Time.deltaTime * BulletSpeed);
                cur_TimeToDestroy -= Time.deltaTime;
                if (cur_TimeToDestroy < 0f) GameObject.Destroy(go);
            })
            ;
        go.OnCollisionEnterAsObservable()
            .Where(collision => collision.collider.CompareTag("Player"))
            .Subscribe(collision =>
            {
                var player = collision.collider.GetComponent<Protagonist>();
                player.TakeDamage(10f);
                GameObject.Destroy(go);
            })
            ;
    }
}
