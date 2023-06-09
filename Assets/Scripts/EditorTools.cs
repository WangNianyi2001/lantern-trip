using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;


using UnityEngine;
using UnityEditor;

public class EditorTools : EditorWindow
{
    private Material[] materials;
    private int selectedMaterialIndex = 0;
    private GameObject selectedGameObject;

    [MenuItem("Tools/Test")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<EditorTools>("Test Tool");
    }

    private void OnGUI()
    {
        selectedGameObject = Selection.activeGameObject;

        if (selectedGameObject == null)
        {
            EditorGUILayout.HelpBox("Please select a game object", MessageType.Info);
            return;
        }

        materials = Resources.FindObjectsOfTypeAll<Material>();

        string[] materialNames = new string[materials.Length];

        for (int i = 0; i < materials.Length; i++)
        {
            materialNames[i] = materials[i].name;
        }

        selectedMaterialIndex = EditorGUILayout.Popup("Select Material", selectedMaterialIndex, materialNames);

        if (GUILayout.Button("OK"))
        {
            // selectedGameObject.GetComponent<Renderer>().material = materials[selectedMaterialIndex];
            SetMaterialRecursive(selectedGameObject, materials[selectedMaterialIndex]);
        }
    }

    private static void SetMaterialRecursive(GameObject obj, Material selectedMaterial)
    {
        if (obj.GetComponent<Renderer>() != null)
        {
            // 将对象的材质更改为用户选择的材质
            obj.GetComponent<Renderer>().material = selectedMaterial;
        }

        // 遍历所有子物体，并递归应用该材质
        foreach (Transform child in obj.transform)
        {
            SetMaterialRecursive(child.gameObject, selectedMaterial);
        }
    }
}
