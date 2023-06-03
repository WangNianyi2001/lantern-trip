using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CheckHP : Action
{
    public SharedFloat hpThreshold;
    private Enermy _enermy;
    public override void OnStart()
    {
        _enermy = GetComponent<Enermy>();
        base.OnStart();
    }

    public override TaskStatus OnUpdate()
    {
        if (_enermy == null)
        {
            return TaskStatus.Failure;
        }
        return _enermy.Hp < hpThreshold.Value ? TaskStatus.Success : TaskStatus.Failure;
    }
}
