using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskDescription("以r为半径，在AI前方的一个圆内生成小怪")]
public class SpawnMonster : Action
{
    public float r;
    public float minDist;
    public int posNum;
    public SharedGameObject player;
    public GameObject monsterPrefab;

    private List<Vector3> posList;
    private GameObject tempGO;
    private int _idx = 0;
    
    public override void OnStart()
    {
        if (player == null || monsterPrefab == null) return;
        var dir = (player.Value.transform.position - transform.position);
        dir.y = 0;
        dir = dir / Vector3.Magnitude(dir);
        Vector3 oriPos = transform.position + dir * r;
        // posList = Utils.PoissonDiscSampling(oriPos, r, minDist, posNum);
        posList = Utils.NormalSmpling(oriPos, r, posNum);
    }

    public override TaskStatus OnUpdate()
    {
        if (posList == null) return TaskStatus.Failure;
        if (_idx >= posList.Count)
        {
            return TaskStatus.Success;
        }

        tempGO = GameObject.Instantiate(monsterPrefab);
        tempGO.transform.position = posList[_idx++];
        
        return TaskStatus.Running;
    }
}
