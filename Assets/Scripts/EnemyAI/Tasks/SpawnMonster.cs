using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using LanternTrip;
using UniRx;
using UnityEngine;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

[TaskDescription("以r为半径，在AI前方的一个圆内生成小怪")]
public class SpawnMonster : Action
{
    public float r;
    public float minDist;
    public int posNum;
    public SharedGameObject player;
    public string monsterPath;
    private GameObject monsterPrefab;

    private List<Vector3> posList;
    private List<Tinder.Type> tinderTypeList;
    private GameObject tempGO;
    private int _idx = 0;
    private IDisposable _timer;


    private Entity monsterEntity;
    public override void OnStart()
    {
        if (player == null || monsterPath == "") return;

        _idx = 0;
        monsterPrefab = Resources.Load<GameObject>(monsterPath);

        
        var dir = (player.Value.transform.position - transform.position);
        dir.y = 0;
        dir = dir / Vector3.Magnitude(dir);
        Vector3 oriPos = transform.position + dir * r;
        // posList = Utils.PoissonDiscSampling(oriPos, r, minDist, posNum);
        posList = Utils.NormalSmpling(oriPos, r, posNum);
        tinderTypeList = RandomTinderType(posNum);

        _timer = Observable.Interval(TimeSpan.FromSeconds(1.0f)).Subscribe(_ =>
        {
            tempGO = GameObject.Instantiate(monsterPrefab);
            monsterEntity = monsterPrefab.GetComponent<Entity>();
            monsterEntity.shotType = tinderTypeList[_idx++];
            tempGO.transform.position = posList[_idx++] + Vector3.up * 50f;
        });
    }

    public override TaskStatus OnUpdate()
    {
        if (posList == null) return TaskStatus.Failure;
        if (_idx >= posList.Count)
        {
            _timer.Dispose();
            return TaskStatus.Success;
        }
        
        
        return TaskStatus.Running;
    }

    private List<Tinder.Type> RandomTinderType(int count)
    {
        List<Tinder.Type> res = new List<Tinder.Type>(count);
        for (int i = 0; i < count; i++)
        {
            int r = UnityEngine.Random.Range(0, (int)Tinder.Type.End);
            res.Add((Tinder.Type)r);
        }

        return res;
    }
}
