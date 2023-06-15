using System;
using System.Collections;
using System.Collections.Generic;
using LanternTrip;
using UnityEngine;
using UniRx;

public class Boss : Enermy
{
    public ParticleSystem levelUpCustom;
    private IDisposable _WeaknessTimer;

    public float particleUpdateDeltaTime = 1.0f;
    void Start()
    {
        curHp.Value = fullHp;
        Invincible = true;
        
        Observable.Interval(TimeSpan.FromSeconds(particleUpdateDeltaTime)).Subscribe(_ =>
        {
            ResetParticle();
        });
    }

    private Tinder.Type GetRandomTinderType()
    {
        int r = UnityEngine.Random.Range(0, (int)Tinder.Type.End - 1);
        return (Tinder.Type)r;
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
