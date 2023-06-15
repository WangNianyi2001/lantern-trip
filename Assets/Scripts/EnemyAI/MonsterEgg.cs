using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using LanternTrip;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Formats.Alembic.Importer;

public class MonsterEgg : Entity
{
    public float AbcSpeed = 1.0f;
    public bool spawnActor = true;

    private AlembicStreamPlayer _player;
    private GameObject _monster;
    private Enermy _enermy;
    private bool _triggered = false;
    private void Awake()
    {
        _player = GetComponent<AlembicStreamPlayer>();
    }
    
    public void OnMatchedShot()
    {
        if (_triggered)
        {
            return;
        }
        _triggered = true;

        var collider = GetComponent<Collider>();
        collider.isTrigger = true;
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        // step 1: spawn actor
        if (spawnActor) SpawnActor();
        
        // step 2: play crush animation
        StartCoroutine(Crushing());
        
    }

    void SpawnActor()
    {
        _monster = Resources.Load<GameObject>("Monster_" + shotType.ToString());
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