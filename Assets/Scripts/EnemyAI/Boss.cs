using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using LanternTrip;
using UnityEngine;
using UniRx;
using UnityEngine.AI;

public class Boss : Enermy
{
    public ParticleSystem levelUpCustom;
    private IDisposable _WeaknessTimer;

    public float particleUpdateDeltaTime = 1.0f;
    void Start()
    {
        Invincible = true;
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _behavior = GetComponent<BehaviorTree>();
        
        curHp.Value = fullHp;
        curHp.Subscribe(value =>
        {
            OnAttack(value);
        });
    }

    private Tinder.Type GetRandomTinderType()
    {
        int r = UnityEngine.Random.Range(0, (int)Tinder.Type.End - 1);
        return (Tinder.Type)r;
    }
    
    private void OnAttack(float cur_hp)
    {
        // Die
        if (cur_hp<=0)
        {
            // _animator.SetTrigger("Death");
            // _behavior.enabled = false;
            // _agent.isStopped = true;
            // Invincible = true;
            // _timer?.Dispose();_timer2?.Dispose();
            // _timer = Observable.Timer(TimeSpan.FromSeconds(2.1f)).Subscribe(_ =>
            // {
            //     onDie?.Invoke();
            //     GameObject.Destroy(gameObject);
            // });
            // Debug.Log("死亡");
                
        }
        else if (cur_hp <= fullHp - 0.1f)
        {
            // _animator.SetTrigger("Hit");
            // _behavior.enabled = false;
            // _agent.isStopped = true;
            // _timer?.Dispose();_timer2?.Dispose();
            // _timer = Observable.Timer(TimeSpan.FromSeconds(onHitTimeSpan)).Subscribe(_ =>
            // {
            //     _behavior.enabled = true;
            //     _agent.isStopped = false;
            // });
        }
    }
    
    void ResetParticle()
    {
        levelUpCustom.Clear();
        levelUpCustom.Play();
    }

    // 暴露弱点
    public void ExposeWeakness(float time, Tinder.Type inputTinder = Tinder.Type.Invalid)
    {
        
        Invincible = false;
        var weaknessTinder = inputTinder == Tinder.Type.Invalid ? GetRandomTinderType() : inputTinder;
        tinderType = weaknessTinder;
        var color = Color.blue;
        switch (tinderType)
        {
            case Tinder.Type.Red:
                color = Color.red;
                break;
            case Tinder.Type.Blue:
                color = Color.blue;
                break;
            case Tinder.Type.Green:
                color = Color.green;
                break;
        }

        Shader.SetGlobalColor("Color_01", color);
        Shader.SetGlobalColor("Color_02", color);
        
        _WeaknessTimer?.Dispose();
        levelUpCustom.gameObject.SetActive(true);
        _WeaknessTimer = Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(_ =>
        {
            Invincible = true;
            tinderType = Tinder.Type.Invalid;
            levelUpCustom.gameObject.SetActive(false);
            levelUpCustom.Clear();
        });
        
        
    }
}
