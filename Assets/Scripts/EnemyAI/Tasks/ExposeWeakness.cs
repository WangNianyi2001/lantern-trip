using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using LanternTrip;
using UnityEngine;

public class ExposeWeakness : Action
{
    public float time;
    public Tinder.Type tinderType = Tinder.Type.Invalid;
    public override void OnStart()
    {
        GetComponent<Boss>()?.ExposeWeakness(time, tinderType);
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
