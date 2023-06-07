using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using LanternTrip;
using UnityEngine;
using UnityEngine.AI;
using Action = BehaviorDesigner.Runtime.Tasks.Action;


public class SeekAttack : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the agent is seeking")]
    public SharedGameObject target;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("If target is null then use the target position")]
    public SharedVector3 targetPosition;

    public string AnimTriggerName;
    public string BlendTreeParamName;
    public float AttackDis;
    public float CD;
    private float cd;

    // private Player player;
    private Protagonist player;

    private Animator _animator;
    private NavMeshAgent _agent;
    private TimeLine _timeLine = new TimeLine();

    private bool isAttacking = false;

    /// <summary>
    /// 获取目标位置，如果target为空则返回targetPosition
    /// </summary>
    /// <returns></returns>
    private Vector3 Target()
    {
        if (target.Value != null) {
            return target.Value.transform.position;
        }
        return targetPosition.Value;
    }
    
    private bool SetDestination(Vector3 destination)
    {
        _agent.isStopped = false;
        return _agent.SetDestination(destination);
    }

    private void InitSkill()
    {
        var radius = 5.0f;
        var settleTime = 0.5f;
        
        // 播放攻击动画
        _timeLine.AddEvent(0, 0, i =>
        {
            _animator.SetTrigger(AnimTriggerName);
            isAttacking = true;
            string.Format("<color=#ff0000>{0}</color>", "Attack Animation");
        });
        
        // 解算伤害
        _timeLine.AddEvent(0.5f, 1, i =>
        {
            var colliderObj = GameObject.Instantiate(Resources.Load<GameObject>("SettlementObj"));
            colliderObj.transform.position = transform.position + Vector3.forward * 1.5f;

            SettlementObj settlement;
            settlement = colliderObj.GetComponent<SettlementObj>();
            if (settlement == null)
                settlement = colliderObj.AddComponent<SettlementObj>();
            
            settlement.Init(radius, settleTime, c =>
            {
                if (!c.CompareTag("Player"))
                    return;
                var playerComponent = c.GetComponent<Protagonist>();
                
                playerComponent?.TakeDamage(1.0f);
                Debug.Log("收到怪物上海:: 1.0");
            });
            
            
        });
        
        // 攻击结束
        _timeLine.AddEvent(3.0f, 1, i =>
        {
            isAttacking = false;
        });
    }
    
    public void RunTimeLine(float dt)
    {
        _timeLine.Loop(dt);
    }


    /// <summary>
    /// 同Start()
    /// </summary>
    public override void OnStart()
    {
        cd = CD;
        player = target.Value.GetComponent<Protagonist>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        SetDestination(Target());

        InitSkill();
    }
    
    /// <summary>
    /// 同Update
    /// </summary>
    /// <returns></returns>
    public override TaskStatus OnUpdate()
    {
        cd -= Time.deltaTime;
        _animator.SetFloat(BlendTreeParamName, _agent.velocity.magnitude);

        RunTimeLine(Time.deltaTime);
        if (isAttacking)
        {
            _agent.isStopped = true;
            return TaskStatus.Running;
        }
        
        var dis = Vector3.Magnitude(target.Value.transform.position - transform.position);
        if (dis > AttackDis)
        {
            Seek();
        }
        else
        {
            Attack();
        }
        return TaskStatus.Running;
    }

    private void Seek()
    {
        // _animator.SetTrigger("Walk");
        
        SetDestination(Target());
    }

    /// <summary>
    /// 攻击
    /// </summary>
    private void Attack()
    {
        if (cd > 0)
        {
            return;
        }
        cd = CD;
        
        
        
        _timeLine.Start();
        
        
        

    }
}
