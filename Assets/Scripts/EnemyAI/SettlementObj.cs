using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// 伤害结算物
/// </summary>
public class SettlementObj : MonoBehaviour
{
    List<Transform> _enemyTransforms = new List<Transform>();


    private Rigidbody _rb;

    private Subject<Collider> _onHitS = new Subject<Collider>();
    public IObservable<Collider> OnHitCallback => _onHitS;

    public Action onDestroyCallback;

    public void Init(float radius, float settleTime, Action<Collider> hitCallback)
    {
        OnHitCallback.Subscribe(hitCallback);
        //增加刚体
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.useGravity = false;
        //加碰撞器
        var c = gameObject.AddComponent<SphereCollider>();
        c.isTrigger = true;
        c.radius = radius;


        GameObject.Destroy(gameObject, settleTime);
    }


    
    private void Update()
    {

        
    }

    private void OnDestroy()
    {
        onDestroyCallback?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        //transforms.Add();
        if (other.CompareTag("Player"))
        {
            _enemyTransforms.Add(other.transform);
        }
        
        _onHitS.OnNext(other);
    }
}