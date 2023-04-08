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

    public float TimeInterval = 0.5f;

    private float cur_TimeInterval;

    public float TimeToDestroy = 2.0f;
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
        var dir = other.transform.position - transform.position;
        dir.y = 0;

        var cur_TimeToDestroy = TimeToDestroy;

        go.UpdateAsObservable()
            .Subscribe(_ =>
            {
                go.transform.Translate(dir * Time.deltaTime);
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
