using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using LanternTrip;

public class StumblingSphere : MonoBehaviour
{
    public float TimeInterval = 0.5f;
    public float TimeToDestroy = 2.0f;
    public GameObject Sphere;
    private Transform Slot1;
    private Transform Slot2;
    private Transform Slot3;
    
    void Start()
    {
        Slot1 = transform.GetChild(0);
        Slot2 = transform.GetChild(1);
        Slot3 = transform.GetChild(2);
        if (Sphere != null && Slot1 != null && Slot2 != null && Slot3 != null)
        {
            
        }
        StartCoroutine(SpawnSphere());
    }

    void Update()
    {
        
    }

    IEnumerator SpawnSphere()
    {
        while (Sphere!=null)
        {
            var go1 = GameObject.Instantiate(Sphere);
            go1.transform.position = Slot1.position;
            SpawActor(TimeToDestroy, go1);
            yield return new WaitForSeconds(TimeInterval);

            var go2 = GameObject.Instantiate(Sphere);
            go2.transform.position = Slot2.position;
            SpawActor(TimeToDestroy, go2);
            yield return new WaitForSeconds(TimeInterval);
            
            var go3 = GameObject.Instantiate(Sphere);
            go3.transform.position = Slot3.position;
            SpawActor(TimeToDestroy, go3);
            yield return new WaitForSeconds(TimeInterval);
        }
    }

    private void SpawActor(float _TimeToDestroy, GameObject go)
    {
        var cur_TimeToDestroy = _TimeToDestroy;
        go.UpdateAsObservable()
            .Subscribe(_ =>
            {
                cur_TimeToDestroy -= Time.deltaTime;
                if (cur_TimeToDestroy < 0)
                    GameObject.Destroy(go);
            });
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
