using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class FacePlayerTask : Action
{
    public SharedGameObject player;
    public override TaskStatus OnUpdate()
    {
        if (player.Value == null) return TaskStatus.Failure;
        var dir = (player.Value.transform.position - transform.position);
        dir.y = 0;
        dir = dir / Vector3.Magnitude(dir);
        var angle = Vector3.Dot(transform.right, dir);
        transform.Rotate(Vector3.up, angle * 115 * Time.deltaTime);
        return Mathf.Abs(angle) < Mathf.PI / 36 ? TaskStatus.Success : TaskStatus.Running;
    }
}
