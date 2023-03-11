//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using System.Collections;
using UnityEngine;

public class CoroutineMgr : Singleton<CoroutineMgr>
{
    GameObject _root;
    MonoBehaviour _coroutineMono;

    public void Init()
    {
        _root = new GameObject("QuickCoroutine");
        GameObject.DontDestroyOnLoad(_root);
        _coroutineMono = _root.AddComponent<CoroutineMono>();
    }


    public Coroutine StartCoroutine(IEnumerator routine) => _coroutineMono.StartCoroutine(routine);
}

public class CoroutineMono : MonoBehaviour
{
}