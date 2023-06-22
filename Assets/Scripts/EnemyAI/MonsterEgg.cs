using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using LanternTrip;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Formats.Alembic.Importer;

public class MonsterEgg : Entity
{
    public float AbcSpeed = 1.0f;
    public bool spawnActor = true;

    public GameObject bossPrefab;

    private AlembicStreamPlayer _player;
    private GameObject _monster;
    private Enermy _enermy;
    private bool _triggered = false;

    private IDisposable _timer;
    private void Awake()
    {
        _player = transform.GetChild(0).GetComponent<AlembicStreamPlayer>();

        if (spawnActor && bossPrefab != null)
        {
            _timer = Observable.Timer(TimeSpan.FromSeconds(8.0f)).Subscribe(_ =>
            {
                // 仅能被触发一次
                if (_triggered)
                {
                    return;
                }
                _triggered = true;
                
                

                _monster = bossPrefab;
                
                SpawnActor();
                
                StartCoroutine(Crushing());
            });
        }
    }
    
    public void OnMatchedShot()
    {
        // 仅能被触发一次
        if (_triggered)
        {
            return;
        }
        _triggered = true;
        _timer?.Dispose();

        var collider = GetComponent<Collider>();
        collider.isTrigger = true;
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        // step 1: spawn actor
        if (spawnActor)
        {
            shotType = shotType == Tinder.Type.Invalid ? GetRandomTinderType() : shotType;
            _monster = Resources.Load<GameObject>("Monster_" + shotType.ToString());
            SpawnActor();
        }
        
        // step 2: play crush animation
        StartCoroutine(Crushing());
    }
    
    private Tinder.Type GetRandomTinderType()
    {
        int r = UnityEngine.Random.Range(0, (int)Tinder.Type.End - 1);
        return (Tinder.Type)r;
    }
    

    void SpawnActor()
    {
        
        if (_monster == null)
        {
            return;
        }

        _monster = GameObject.Instantiate(_monster);
        _monster.transform.position = transform.position;

        _enermy = _monster.GetComponent<Enermy>();
        _enermy.Invincible = true;
        _enermy.GetComponent<BehaviorTree>().enabled = false;
    }

    IEnumerator Crushing()
    {
        while (_player.CurrentTime <= _player.EndTime - 0.1f)
        {
            
            _player.CurrentTime += Time.deltaTime * AbcSpeed;
            yield return null;
        }

        _enermy.Invincible = false;
        _enermy.GetComponent<BehaviorTree>().enabled = true;
        GameObject.Destroy(gameObject);
    }
}