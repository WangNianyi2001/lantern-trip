#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace HexGrid.Editor
{
    [CustomEditor(typeof(HexGrid))]
    public class HexGridEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate Hex Grid"))
            {
                HexGrid hexGrid = (HexGrid)target;
                hexGrid.CalculateGridData();
            }
        }
    }
}

#endif