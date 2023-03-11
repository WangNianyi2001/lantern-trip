//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public static class UnityExtern
{
    public static T Find<T>(this GameObject parent, string path) where T : class
    {
        var targetObj = parent.transform.Find(path);
        if (targetObj == null) return null;
        return targetObj.GetComponent<T>();
    }

    public static void DestroyAllChildren(this GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject.Destroy(parent.transform.GetChild(i).gameObject);
        }
    }

    public static bool FloatEqual(this float a, float b)
    {
        return Mathf.Abs(a - b) < 0.01f;
    }
    
}
