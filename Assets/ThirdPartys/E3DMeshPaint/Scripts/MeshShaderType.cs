using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 缓存ScriptableObject数据
/// </summary>
[CreateAssetMenu(menuName = "E3D/CreatMeshShadersData")]
public class MeshShaderType : ScriptableObject
{
    /// <summary>
    /// 笔刷尺寸
    /// </summary>
    public float brushSize = 10f;
    /// <summary>
    /// 笔刷强度
    /// </summary>
    public float brushStronger = 0.5f;
    /// <summary>
    /// 笔刷区域
    /// </summary>
    public int brushOrthValue = 2;
    /// <summary>
    /// 显示UV
    /// </summary>
    public bool showUV = false;
    /// <summary>
    /// 贴图编号
    /// </summary>
    [SerializeField]
    public int textureIndex = 0;
    /// <summary>
    /// 笔刷编号
    /// </summary>
    public int brushIndex = 0;

    /// <summary>
    /// 是否在绘制
    /// </summary>
    [HideInInspector]
    public bool isPaint = false;
    [HideInInspector]
    public bool lastIsPaint = false;


    /// <summary>
    /// 可绘制层级
    /// </summary>
    [SerializeField]
    public List<string> layers = new List<string>();
    /// <summary>
    /// 所有可绘制shader
    /// </summary>
    [SerializeField]
    public List<CustomData> shaders = new List<CustomData>();

    public List<string> UpdateLayers(List<string> list)
    {
        layers.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            layers.Add(list[i]);
        }
        return layers;
    }

    public List<CustomData> UpdateShaders(List<CustomData> list)
    {
        shaders.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            shaders.Add(list[i]);
        }
        return shaders;
    }
}

[System.Serializable]
public class CustomData
{
    public string shaderName;
}
