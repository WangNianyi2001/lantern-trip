using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using LanternTrip;
using NaughtyAttributes.Test;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

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
                if (collider.CompareTag("Player"))
                {
                    var protagonist = collider.GetComponent<Protagonist>();
                    if (protagonist != null) protagonist.TakeDamage(1.0f);
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
