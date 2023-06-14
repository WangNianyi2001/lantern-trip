using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using LanternTrip;
using NaughtyAttributes.Test;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Unity.Mathematics;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

public class BoomTask : Action
{
    public bool isTracer = false;

    public float projectileSpeed = 1.0f;

    public SharedGameObject target;
    public Vector3 targetOffset = Vector3.up;
    
    public GameObject bombPrefab;
    public GameObject sparklePrefab;
    
    public Transform oriTrans;
    public Vector3 oriOffset = Vector3.up;


    private int projectileId = 0;
    private Dictionary<int, IDisposable> triggerEnters = new Dictionary<int, IDisposable>(15);
    private Dictionary<int, IDisposable> updates = new Dictionary<int, IDisposable>(15);
    public override void OnStart()
    {
        if (bombPrefab == null)
        {
            Debug.LogError("Please Set Projectile Prefab");
            return;
        }

        if (oriTrans == null)
        {
            oriTrans = transform;
        }
        var targetPos = target.Value.transform.position + targetOffset;
        var originPos = new Vector3(transform.position.x, oriTrans.position.y, transform.position.z) + oriOffset;
        var dir = (targetPos - originPos).normalized;

        
        
        
        var bomb = GameObject.Instantiate(bombPrefab);
        var projectile = bomb.GetComponent<Projectile>();
        projectile.Id = projectileId++;     // 分配 id
        
        bomb.transform.position = originPos;
        var timer = Observable.Timer(TimeSpan.FromSeconds(15.0f)).Subscribe(_ =>
        {
            GameObject.Destroy(bomb);
        });
        updates.TryAdd(projectile.Id, bomb.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (isTracer)
                {
                    Trace(bomb, targetPos, projectileSpeed);
                }else
                    bomb.transform.Translate(dir * projectileSpeed * 6.66f * Time.deltaTime);
            }));
        triggerEnters.TryAdd(projectile.Id, bomb.OnTriggerEnterAsObservable()
            .Subscribe(collider =>
            {
                // 直接造成伤害
                // if (collider.CompareTag("Player"))
                // {
                //     var protagonist = collider.GetComponent<Protagonist>();
                //     if (protagonist != null) protagonist.TakeDamage(1.0f);
                //     GameObject.Destroy(bomb);
                //
                // }
                
                // 爆炸
                if (collider.CompareTag("Player") || collider.CompareTag("Obstacle"))
                {
                    Debug.Log(collider.name);
                    
                    var radius = 5.0f;
                    var settleTime = 0.5f;
                    
                    // 生成伤害结算物
                    var colliderObj = GameObject.Instantiate(Resources.Load<GameObject>("SettlementObj"));
                    colliderObj.transform.position = bomb.transform.position;
                    
                    SettlementObj settlement;
                    settlement = colliderObj.GetComponent<SettlementObj>();
                    if (settlement == null) settlement = colliderObj.AddComponent<SettlementObj>();
                    
                    settlement.Init(radius, settleTime, c =>
                    {
                        if (!c.CompareTag("Player")) 
                            return; 
                        var playerComponent = c.GetComponent<Protagonist>();
                        
                        playerComponent?.TakeDamage(1.0f); 
                        Debug.Log("收到怪物上海:: 1.0");
                    
                    });

                    var sparkle = GameObject.Instantiate(sparklePrefab);
                    sparkle.transform.position = bomb.transform.position;
                    
                    Observable.Timer(TimeSpan.FromSeconds(1.5f)).Subscribe(_ =>
                    {
                        GameObject.Destroy(sparkle);
                        GameObject.Destroy(bomb);
                    });
                    var id = bomb.GetComponent<Projectile>().Id;
                    
                    triggerEnters[id].Dispose();
                    updates[id].Dispose();
                    
                    triggerEnters.Remove(id);
                    updates.Remove(id);
                }
            }));

        
        base.OnStart();
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }

    private void Trace(GameObject Bullet, Vector3 targetPos, float speed = 1.0f, float turnSpeed = 1.0f)
    {
        
            // 计算目标位置
            Vector3 targetPosition = target.Value.transform.position + targetOffset;;

            // 插值计算当前位置与目标位置之间的新位置
            // Bullet.transform.position = Vector3.Lerp(Bullet.transform.position, targetPosition, Time.deltaTime * speed);
            
            // var dir = (targetPosition - Bullet.transform.position).normalized;

            var dis = (transform.position - targetPos).magnitude;
            
            var offset = targetPosition - Bullet.transform.position;
            var dir = offset.normalized;
            var x = offset.magnitude;
            var f = math.remap(0, dis, 1f, 10f, x);
            
            Bullet.transform.Translate(dir * speed * f * Time.deltaTime);
    
            // 根据目标位置计子弹应该旋转的角度
            Vector3 direction = targetPosition - Bullet.transform.position;
            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                Bullet.transform.rotation = Quaternion.Slerp(Bullet.transform.rotation, rotation, Time.deltaTime * turnSpeed);
            }

    }
}
