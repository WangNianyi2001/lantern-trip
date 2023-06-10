using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using LanternTrip;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public class Enermy : MonoBehaviour
{
    public float fullHp = 100f;
    public FloatReactiveProperty curHp;

    
    public bool Invincible = false;

    public GameObject player;

    public Tinder.Type tinderType;

    private Animator _animator;
    private NavMeshAgent _agent;
    private BehaviorTree _behavior;

    private IDisposable _timer;
    private IDisposable _timer2;
    
    
    public void LookAtPlayer()
    {
        if (player == null) return;
        var dir = (player.transform.position - transform.position);
        dir.y = 0;
        dir = dir / Vector3.Magnitude(dir);
        var angle = Vector3.Dot(transform.right, dir);
        transform.Rotate(Vector3.up, angle);
    }

    private void OnAttack(float cur_hp)
    {
        // Die
        if (cur_hp<=0)
        {
            _animator.SetTrigger("Death");
            _behavior.enabled = false;
            _timer?.Dispose();_timer2?.Dispose();
            _timer = Observable.Timer(TimeSpan.FromSeconds(3.1f)).Subscribe(_ =>
            {
                    
                GameObject.Destroy(gameObject);
            });
            Debug.Log("死亡");
                
        }
        else
        {
            _animator.SetTrigger("Hit");
            _behavior.enabled = false;
            _timer?.Dispose();_timer2?.Dispose();
            _timer = Observable.Timer(TimeSpan.FromSeconds(2.5f)).Subscribe(_ =>
            {
                _behavior.enabled = true;
            });
        }
    }

    public void Attack(string AnimTriggerName)
    {
        var radius = 5.0f;
        var settleTime = 0.5f;
        
        // 开始攻击
        _animator.SetTrigger(AnimTriggerName);
        _behavior.enabled = false;
        _agent.isStopped = true;
        _timer?.Dispose();
        
        
        Debug.Log(string.Format("<color=#ff0000>{0}</color>", "Attack Animation"));
        
        // 生成伤害结算物
        _timer = Observable.Timer(TimeSpan.FromSeconds(1.0f)).Subscribe(_ =>
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
        
        // 完成攻击
        _timer2 = Observable.Timer(TimeSpan.FromSeconds(3.0f)).Subscribe(_ =>
        {
            _behavior.enabled = true;
            _agent.isStopped = false;
        });
    }
    
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _behavior = GetComponent<BehaviorTree>();
        
        curHp.Value = fullHp;
        curHp.Subscribe(value =>
        {
            OnAttack(value);
        });
    }

    private void Update()
    {
        LookAtPlayer();
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
