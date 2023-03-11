//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
    void Start()
    {
        var timer = TimerMgr.Instance.CreateTimer(1f, -1, () => { Debug.Log(1); });
        
        timer.Start();
    }

    void Update()
    {
        
        TimerMgr.Instance.Loop(Time.deltaTime);
    }
    
}
