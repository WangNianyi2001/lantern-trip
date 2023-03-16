//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using UnityEngine;

public class Singleton<T> where T : Singleton<T>, new()
{
    private static T _instance = null;
    private static object LockObj = new object();
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (LockObj)
                {
                    if (_instance==null)
                    {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
    }
}