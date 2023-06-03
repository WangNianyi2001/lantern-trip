using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using LanternTrip;
using UnityEngine;

public class SetInvincible : Action
{
    public SharedBool value;
    public override TaskStatus OnUpdate()
    {
        var enemyComponent = GetComponent<Enermy>();
        if (enemyComponent != null)
        {
            enemyComponent.Invincible = value.Value;
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
