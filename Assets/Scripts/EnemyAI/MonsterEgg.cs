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

        // step 1: spawn actor
        _monster = Resources.Load<GameObject>("Monster_Blue");

        _enermy = _monster.GetComponent<Enermy>();
 
        _monster.transform.position = transform.position;
        GameObject.Instantiate(_monster);
        
        // step 2: play crush animation
        StartCoroutine(Crushing());
        
    }

    IEnumerator Crushing()
    {
        _enermy.Invincible = true;
        while (_player.CurrentTime <= _player.EndTime - 0.1f)
        {
            
            _player.CurrentTime += Time.deltaTime;
            yield return null;
        }

        _enermy.Invincible = false;
        GameObject.Destroy(gameObject);
    }
}
