//code by 赫斯基皇
//https://space.bilibili.com/455965619
//https://github.com/Heskey0

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : Singleton<SceneMgr>
{

    public void Load(int index,Action onSceneLoaded)
    {
        SceneManager.sceneLoaded += (arg0, mode) =>
        {
            onSceneLoaded?.Invoke();
        };
        SceneManager.LoadScene(index);
    }
}
