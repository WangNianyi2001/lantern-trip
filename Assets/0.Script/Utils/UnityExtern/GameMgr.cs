//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using QFramework;
using UnityEngine;

public class GameMgr : Singleton<GameMgr>
{
    GameObject _engineRoot;


    public void Init()
    {
        // QMsgCenter.Instance.ForwardMsg = ForwardMsgCallback;

        if (_engineRoot == null)
        {
            _engineRoot = new GameObject("GameEngine");
            GameObject.DontDestroyOnLoad(_engineRoot);
            _engineRoot.AddComponent<GameEngine>();
            // _engineRoot.AddComponent<Test>();
        }

        CoroutineMgr.Instance.Init();
    }

    void ForwardMsgCallback(QMsg msg)
    {
        switch (msg.ManagerID)
        {
            case QMgrID.Game:
                //SkillManager.Instance.SendMsg(msg);
                break;

            default:
                break;
        }
    }
}