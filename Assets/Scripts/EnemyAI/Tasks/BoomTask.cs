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
    public SharedGameObject target;
    public GameObject bombPrefab;
    public Transform oriPos;
    public override void OnStart()
    {
        if (bombPrefab == null)
        {
            return;
        }
        var targetPos = target.Value.transform.position;
        var originPos = new Vector3(transform.position.x, oriPos.position.y + 1, transform.position.z);
        var dir = targetPos - originPos;

        var bomb = GameObject.Instantiate(bombPrefab);
        bomb.transform.position = originPos;
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
                if (!collider.CompareTag("Enemy"))
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
                    GameObject.Destroy(bomb);
                        
                }
            });
        bomb.UpdateAsObservable()
            .Subscribe(_ =>
            {
                bomb.transform.Translate(dir * Time.deltaTime);
            });
        
        base.OnStart();
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
