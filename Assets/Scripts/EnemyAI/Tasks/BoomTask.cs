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
using Action = BehaviorDesigner.Runtime.Tasks.Action;

public class BoomTask : Action
{
    public bool isTracer = false;

    public float projectileSpeed = 1.0f;

    public SharedGameObject target;
    public Vector3 targetOffset = Vector3.up;
    public GameObject bombPrefab;
    public Transform oriPos;
    public override void OnStart()
    {
        if (bombPrefab == null)
        {
            return;
        }
        var targetPos = target.Value.transform.position + targetOffset;
        var originPos = new Vector3(transform.position.x, oriPos.position.y + 1, transform.position.z);
        var dir = targetPos - originPos;

        var bomb = GameObject.Instantiate(bombPrefab);
        bomb.transform.position = originPos;
        var timer = Observable.Timer(TimeSpan.FromSeconds(15.0f)).Subscribe(_ =>
        {
            GameObject.Destroy(bomb);
        });
        bomb.OnTriggerEnterAsObservable()
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
                if (!collider.CompareTag("Enemy") && !collider.CompareTag("SettlementObj"))
                {
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
                    timer.Dispose();
                    GameObject.Destroy(bomb);
                        
                }
            });
        bomb.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (isTracer)
                {
                    Trace(bomb, targetPos, projectileSpeed);
                }else
                    bomb.transform.Translate(dir * projectileSpeed * Time.deltaTime);
            });
        
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
            var dir = targetPosition - Bullet.transform.position;
            Bullet.transform.Translate(dir * speed * 6.66f * Time.deltaTime);
    
            // 根据目标位置计子弹应该旋转的角度
            Vector3 direction = targetPosition - Bullet.transform.position;
            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                Bullet.transform.rotation = Quaternion.Slerp(Bullet.transform.rotation, rotation, Time.deltaTime * turnSpeed);
            }

    }
}
