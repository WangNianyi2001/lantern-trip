//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class ResourcesMgr : Singleton<ResourcesMgr>
{
    ResLoader mResLoader = ResLoader.Allocate();


    public T QfGet<T>(string resName) where T : Object
    {
        return mResLoader.LoadSync<T>(resName);
    }

    public GameObject QfGetInstance(string resName)
    {
        return mResLoader.LoadSync<GameObject>(resName).Instantiate();
    }


    public T Get<T>(string resPath) where T : Object
    {
        return Resources.Load<T>(resPath);
    }

    public GameObject GetInstance(string resPath)
    {
        return GameObject.Instantiate(Get<GameObject>(resPath));
    }
}