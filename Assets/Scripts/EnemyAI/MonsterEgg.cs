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
    private AlembicStreamPlayer _player;
    private GameObject _monster;
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
        // step 1: play crush animation
        StartCoroutine(Crushing());
        // step 2: spawn actor
        _monster = Resources.Load<GameObject>("Monster_Blue");
        
        _monster.transform.position = transform.position;
        GameObject.Instantiate(_monster);
    }

    IEnumerator Crushing()
    {
        while (_player.CurrentTime <= _player.EndTime - 0.1f)
        {
            
            _player.CurrentTime += Time.deltaTime;
            yield return null;
        }

        GameObject.Destroy(gameObject);
    }
}
