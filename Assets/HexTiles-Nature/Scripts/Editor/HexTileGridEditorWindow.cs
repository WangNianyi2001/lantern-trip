#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace HexTileGrid
{
    public class HexTileGridEditorWindow : EditorWindow
    {
        private static HexTileGrid hexTileGrid = null;

        private float m_hexTileSize = 0.808f;
        private string m_gridLayerName = "SnapHexGrid";
        private Vector3 m_gridOffset = Vector3.zero;
        private int m_gridWidth = 10;
        private int m_gridHeight = 10;
        private int m_gridDepth = 3;
        private Color m_topCellsColor = Color.white;
        private bool m_displayCells = true;
        private bool m_displayCenterPoints = false;
        private bool m_randomizeCellRotationsOnClick = false;
        private bool m_displayOuterLowerGridCells = false;
        private bool m_displayAllLowerInsideGridCells = false;
        private Color m_lowerCellsColor = new Color(1.0f, 1.0f, 1.0f, 0.3f);
        private bool m_enableGridAroundSelectedObject = false;
        private int m_selectedObjectGridDistance = 6;
        private bool m_enablePositionSnapping = true;
        private bool m_enableRotationSnapping = true;
        private float m_rotationSensitivity = 4.0f;

        [MenuItem("Tools/Hex Tile Grid Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(HexTileGridEditorWindow), true, "Hex Tile Grid Window", true);

            HexTileGrid grid = GameObject.FindObjectOfType<HexTileGrid>();
            if (grid != null)
            {
                hexTileGrid = grid;
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            hexTileGrid = (HexTileGrid)EditorGUILayout.ObjectField(hexTileGrid, typeof(HexTileGrid), true);
            EditorGUILayout.EndHorizontal();

            if (hexTileGrid == null)
            {
                GUILayout.Label("Please select the hex tile grid component from the scene");
            }
            else
            {
                EditorGUIUtility.labelWidth = 220;
                EditorGUILayout.Space();
                this.m_hexTileSize = EditorGUILayout.FloatField("Hex Tile Size", hexTileGrid.m_hexTileSize);
                this.m_gridLayerName = EditorGUILayout.TextField("Grid Layer Name", hexTileGrid.m_gridLayerName);
                this.m_gridOffset = EditorGUILayout.Vector3Field("Grid Offset", hexTileGrid.m_gridOffset);

                EditorGUILayout.Space();
                GUILayout.Label("The number of cells that should spawn");
                this.m_gridWidth = EditorGUILayout.IntField("Grid Width", hexTileGrid.m_gridWidth);
                this.m_gridHeight = EditorGUILayout.IntField("Grid Height", hexTileGrid.m_gridHeight);
                this.m_gridDepth = EditorGUILayout.IntField("Grid Depth", hexTileGrid.m_gridDepth);
                this.m_topCellsColor = EditorGUILayout.ColorField("Top Cells Color", hexTileGrid.m_topCellsColor);
                this.m_displayCells = EditorGUILayout.Toggle("Display Cells", hexTileGrid.m_displayCells);
                this.m_displayCenterPoints = EditorGUILayout.Toggle("Display Center Points", hexTileGrid.m_displayCenterPoints);
                this.m_randomizeCellRotationsOnClick = EditorGUILayout.Toggle("Randomize Cell Rotations on Click", hexTileGrid.m_randomizeCellRotationsOnClick);

                EditorGUILayout.Space();
                this.m_displayOuterLowerGridCells = EditorGUILayout.Toggle("Display Outer Lower Grid Cells", hexTileGrid.m_displayOuterLowerGridCells);
                this.m_displayAllLowerInsideGridCells = EditorGUILayout.Toggle("Display All Lower Inside Grid Cells", hexTileGrid.m_displayAllLowerInsideGridCells);
                this.m_lowerCellsColor = EditorGUILayout.ColorField("Lower Cells Color", hexTileGrid.m_lowerCellsColor);

                EditorGUILayout.Space();
                GUILayout.Label("This setting overrides the full grid display setting");
                this.m_enableGridAroundSelectedObject = EditorGUILayout.Toggle("Enable Grid Around Selected Objects", hexTileGrid.m_enableGridAroundSelectedObject);
                this.m_selectedObjectGridDistance = EditorGUILayout.IntField("Selected Object Grid Distance", hexTileGrid.m_selectedObjectGridDistance);

                EditorGUILayout.Space();
                this.m_enablePositionSnapping = EditorGUILayout.Toggle("Enable Position Snapping", hexTileGrid.m_enablePositionSnapping);
                this.m_enableRotationSnapping = EditorGUILayout.Toggle("Enable Rotation Snapping", hexTileGrid.m_enableRotationSnapping);

                if (GUI.changed)
                {
                    UpdateHexGridData();
                }
            }
        }

        private void UpdateHexGridData()
        {
            hexTileGrid.m_hexTileSize = this.m_hexTileSize;
            hexTileGrid.m_gridLayerName = this.m_gridLayerName;
            hexTileGrid.m_gridOffset = this.m_gridOffset;
            hexTileGrid.m_gridWidth = this.m_gridWidth;
            hexTileGrid.m_gridHeight = this.m_gridHeight;
            hexTileGrid.m_gridDepth = this.m_gridDepth;
            hexTileGrid.m_topCellsColor = this.m_topCellsColor;
            hexTileGrid.m_displayCells = this.m_displayCells;
            hexTileGrid.m_displayCenterPoints = this.m_displayCenterPoints;
            hexTileGrid.m_randomizeCellRotationsOnClick = this.m_randomizeCellRotationsOnClick;
            hexTileGrid.m_displayOuterLowerGridCells = this.m_displayOuterLowerGridCells;
            hexTileGrid.m_displayAllLowerInsideGridCells = this.m_displayAllLowerInsideGridCells;
            hexTileGrid.m_lowerCellsColor = this.m_lowerCellsColor;
            hexTileGrid.m_enableGridAroundSelectedObject = this.m_enableGridAroundSelectedObject;
            hexTileGrid.m_selectedObjectGridDistance = this.m_selectedObjectGridDistance;
            hexTileGrid.m_enablePositionSnapping = this.m_enablePositionSnapping;
            hexTileGrid.m_enableRotationSnapping = this.m_enableRotationSnapping;

            hexTileGrid.UpdateTileData();
            hexTileGrid.Update();
            hexTileGrid.enabled = false;
            hexTileGrid.enabled = true;
        }
    }
}
#endif