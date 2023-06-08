using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;


public class EditorTools : EditorWindow
{
    private Material[] materials;
    private int selectedMaterialIndex = 0;
    private GameObject selectedGameObject;

    [MenuItem("CustomTool/SetMaterialInBatch", true)]
    private static bool SetMaterialInBatch()   
    {
        if (Selection.gameObjects.Length < 0)
            return false;
        
        foreach (var go in Selection.gameObjects)
        {
            
        }

        return true;
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
            selectedGameObject.GetComponent<Renderer>().material = materials[selectedMaterialIndex];
        }
    }
}
