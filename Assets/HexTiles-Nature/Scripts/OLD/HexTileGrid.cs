#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HexTileGrid
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class HexTileGrid : MonoBehaviour
    {
        [SerializeField] public float m_hexTileSize = 0.808f;
        [SerializeField] public string m_gridLayerName = "SnapHexGrid";
        [SerializeField] public Vector3 m_gridOffset = Vector3.zero;

        [Header("The number of cells that should spawn")]
        [SerializeField] public int m_gridWidth = 10;
        [SerializeField] public int m_gridHeight = 10;
        [SerializeField] public int m_gridDepth = 3;

        [SerializeField] public Color m_topCellsColor = Color.white;

        [SerializeField] public bool m_displayCells = true;
        [SerializeField] public bool m_displayCenterPoints = false;
        [SerializeField] public bool m_randomizeCellRotationsOnClick = false;

        [Space, Space]
        [SerializeField] public bool m_displayOuterLowerGridCells = false;
        [SerializeField] public bool m_displayAllLowerInsideGridCells = false;
        [SerializeField] public Color m_lowerCellsColor = new Color(1.0f, 1.0f, 1.0f, 0.3f);

        [Header("This setting overrides the full grid display setting")]
        [SerializeField] public bool m_enableGridAroundSelectedObject = false;
        [SerializeField] public int m_selectedObjectGridDistance = 6;

        [Space, Space]
        [SerializeField] public bool m_enablePositionSnapping = true;
        [SerializeField] public bool m_enableRotationSnapping = true;

        private const int maxLayers = 31;
        private const string m_searchDirectory = "Assets/HexTiles-Nature";
        private const string m_prefabExtension = ".prefab";

        private float m_innerRadius = 0.0f;
        private Vector3[] m_corners = new Vector3[6];
        private int m_totalSelectedObjectGridDistance = 0;
        private float m_rotationIncrement = 60.0f;

        private GameObject[] m_selectedGameObjects = null;
        private GameObject m_mainSelectedGameObject = null;

        private Vector3[] m_cellPositions;
        private Vector3[] m_distanceSortedCellPositions;

        private readonly string[] m_prefabsToAddToLayer =
        {
            "PF_ConiferTree_1",
            "PF_ConiferTree_2",
            "PF_ConiferTree_3",
            "PF_ConiferTree_4",

            "PF_RegTree_1",
            "PF_RegTree_2",
            "PF_RegTree_3",
            "PF_RegTree_4",

            "PF_TallTree_1",
            "PF_TallTree_2",

            "PF_Roots_4",

            "PF_HexBoard_StoneChiseled_1",
            "PF_HexBoard_StoneVar0",
            "PF_HexBoard_StoneVar1",
            "PF_HexBoard_StoneVar2",

            "PF_HexGrass_Base",
            "PF_HexGrass_BendPath_60deg",
            "PF_HexGrass_EdgeGrass_Inner",
            "PF_HexGrass_EdgeGrass_Outer",
            "PF_HexGrass_EndPath",
            "PF_HexGrass_HexBoardGrass",
            "PF_HexGrass_StraightPath",
            "PF_HexGrass_Surface",

            "PF_HexTerrain_Mid",
            "PF_HexTerrain_Ramp_Bottom",
            "PF_HexTerrain_Ramp_Top",
            "PF_HexTerrain_Top",
            "PF_HexTerrain_Top_NoSurface",
            "PF_HexTerrain_Top_Surface",

            "PF_StoneFloor",
            "PF_StoneFloor_NoGrass",
            "PF_StoneSteps_Straight",
            "PF_StoneSteps_Straight_Offset",
            "PF_StoneSteps_Straight_Walls",

            "PF_StoneWall_Inner_1",
            "PF_StoneWall_Inner_1_Half",
            "PF_StoneWall_Inner_2",
            "PF_StoneWall_Inner_2_Half",
            "PF_StoneWall_Outer_1",
            "PF_StoneWall_Outer_1_Half",
            "PF_StoneWall_Outer_2",
            "PF_StoneWall_Outer_2_Half",
            "PF_StoneWall_Straight",
            "PF_StoneWall_Straight_ForSteps",
            "PF_StoneWall_Straight_Steps",

            "PF_WoodenBridge",
            "PF_WoodenBridge_Broken",
            "PF_WoodenBridge_Support"
        };

        private void OnEnable()
        {
            UpdateTileData();

            Selection.selectionChanged += OnSelectionChanged;
            EditorApplication.update += Update;
        }

        private void OnValidate()
        {
            UpdateTileData();
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            EditorApplication.update -= Update;
        }

        private void OnDestroy()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            EditorApplication.update -= Update;
        }

        public void UpdateTileData()
        {
            //if the grid layer doesn't exist then add it
            if (!LayerExists(m_gridLayerName))
            {
                AddHexGridLayer();
                AddPrefabsToLayer();
            }
            
            m_totalSelectedObjectGridDistance = m_selectedObjectGridDistance * 3;
            m_cellPositions = new Vector3[m_gridWidth * m_gridHeight * m_gridDepth];

            m_innerRadius = m_hexTileSize * 0.866025404f;

            m_corners = new[] {
                new Vector3(0f, 0f, m_hexTileSize),
                new Vector3(m_innerRadius, 0f, 0.5f * m_hexTileSize),
                new Vector3(m_innerRadius, 0f, -0.5f * m_hexTileSize),
                new Vector3(0f, 0f, -m_hexTileSize),
                new Vector3(-m_innerRadius, 0f, -0.5f * m_hexTileSize),
                new Vector3(-m_innerRadius, 0f, 0.5f * m_hexTileSize)
            };

            //generate hex tiles
            int i = 0;
            for (int x = 0; x < m_gridWidth; ++x)
            {
                for (int y = 0; y < m_gridDepth; y++)
                {
                    for (int z = 0; z < m_gridHeight; z++)
                    {
                        GenerateHexPosition(x, y, z, i++);
                    }
                }
            }
        }

        public void Update()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            m_selectedGameObjects = Selection.gameObjects;
            m_mainSelectedGameObject = Selection.activeGameObject;

            //only snap selected objects which also on the correct layer
            if (m_selectedGameObjects != null && m_selectedGameObjects.Length > 0)
            {
                //user is moving the entire hex tile grid
                for (int i = 0; i < m_selectedGameObjects.Length; ++i)
                {
                    if (m_selectedGameObjects[i].scene.IsValid() && m_selectedGameObjects[i] == gameObject)
                    {
                        UpdateTileData();
                        break;
                    }
                }

                //snap objects to hex tile
                for (int i = 0; i < m_selectedGameObjects.Length; ++i)
                {
                    if (m_selectedGameObjects[i].scene.IsValid() && m_selectedGameObjects[i].layer == LayerMask.NameToLayer(m_gridLayerName))
                    {
                        SnapSelectedObjectToGrid(m_selectedGameObjects[i]);

                        if (m_enableGridAroundSelectedObject)
                        {
                            SortHexCellPositions();
                        }
                    }
                }
            }

            if (!m_displayCells)
            {
                return;
            }

            if (m_enableGridAroundSelectedObject)
            {
                //draw around selected game object
                if (m_selectedGameObjects != null && m_selectedGameObjects.Length > 0)
                {
                    if (m_mainSelectedGameObject.scene.IsValid() && m_mainSelectedGameObject.layer == LayerMask.NameToLayer(m_gridLayerName))
                    {
                        for (int i = 0; i < m_distanceSortedCellPositions.Length; ++i)
                        {
                            if (i >= m_totalSelectedObjectGridDistance) break;

                            HexTileOverrides tileOverrides = GetTileOverrides(m_mainSelectedGameObject);
                            Vector3 positionoffset = tileOverrides != null ? tileOverrides.PositionOffset() : Vector3.zero;

                            Color color = Math.Abs(m_mainSelectedGameObject.transform.position.y - m_distanceSortedCellPositions[i].y - positionoffset.y) < 0.05f ? m_topCellsColor : m_lowerCellsColor;
                            DrawHexTile(m_distanceSortedCellPositions[i], color);
                        }
                    }
                }
            }
            else
            {
                //draw everywhere
                int i = 0;
                for (int x = 0; x < m_gridWidth; ++x)
                {
                    for (int y = 0; y < m_gridDepth; y++)
                    {
                        for (int z = 0; z < m_gridHeight; z++)
                        {
                            bool shouldDraw = (m_displayOuterLowerGridCells == true && (z == 0 || z == m_gridHeight - 1 || x == 0 || x == m_gridWidth - 1)) || y == 0 || m_displayAllLowerInsideGridCells ? true : false;

                            if (shouldDraw)
                            {
                                Color color = y == 0 ? m_topCellsColor : m_lowerCellsColor;
                                DrawHexTile(m_cellPositions[i], color);
                            }

                            i++;
                        }
                    }
                }
            }
        }

        private bool AddHexGridLayer()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");

            if (!PropertyExists(layersProp, 0, maxLayers, m_gridLayerName))
            {
                SerializedProperty sp;

                for (int i = 8, j = maxLayers; i < j; i++)
                {
                    sp = layersProp.GetArrayElementAtIndex(i);
                    if (sp.stringValue == "")
                    {
                        sp.stringValue = m_gridLayerName;
                        Debug.Log("Layer: " + m_gridLayerName + " has been added");
                        tagManager.ApplyModifiedProperties();
                        return true;
                    }
                }
            }

            Debug.Log("Unable to add layer " + m_gridLayerName);
            return false;
        }

        private bool LayerExists(string layerName)
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            // Layers Property
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            return PropertyExists(layersProp, 0, maxLayers, layerName);
        }

        private bool PropertyExists(SerializedProperty property, int start, int end, string value)
        {
            for (int i = start; i < end; i++)
            {
                SerializedProperty t = property.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        private void AddPrefabsToLayer()
        {
            if (!LayerExists(m_gridLayerName) || !Directory.Exists(m_searchDirectory))
            {
                return;
            }

            DirectoryInfo dir = new DirectoryInfo(m_searchDirectory);
            FileInfo[] info = dir.GetFiles("*" + m_prefabExtension, SearchOption.AllDirectories);

            for (int i = 0; i < info.Length; ++i)
            {
                string culledName = info[i].Name.Replace(m_prefabExtension, "");
                if (m_prefabsToAddToLayer.Contains(culledName))
                {
                    int assetIndex = info[i].FullName.IndexOf("Assets", StringComparison.Ordinal);

                    if (assetIndex != -1)
                    {
                        string assetPath = info[i].FullName.Remove(0, assetIndex);

                        GameObject hexTilePrefab = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                        hexTilePrefab.layer = LayerMask.NameToLayer(m_gridLayerName);
                    }
                }
            }
        }

        private void GenerateHexPosition(int x, int y, int z, int tileIndex)
        {
            Vector3 position = transform.position + m_gridOffset;
            position.y -= y * (m_hexTileSize * 1.5f);
            position.z += z * (m_hexTileSize * 1.5f);

            if (z % 2 == 0)
            {
                position.x += (x + z * 0.5f - z * 0.5f) * (m_innerRadius * 2.0f);
            }
            else
            {
                position.x += (x + z * 0.5f - z * 0.5f) * (m_innerRadius * 2.0f) + m_innerRadius;
            }

            m_cellPositions[tileIndex] = position;
        }

        private void DrawHexTile(Vector3 cell, Color color)
        {
            for (int i = 0; i < m_corners.Length; i++)
            {
                Vector3 startPos = i != 0 ? m_corners[i - 1] : m_corners[m_corners.Length - 1];
                Debug.DrawLine(startPos + cell, m_corners[i] + cell, color);
            }

            if (m_displayCenterPoints)
            {
                Debug.DrawLine(new Vector3(cell.x, cell.y + 0.25f, cell.z), new Vector3(cell.x, cell.y - 0.25f, cell.z), Color.blue);
            }
        }

        private void SortHexCellPositions()
        {
            m_distanceSortedCellPositions = m_cellPositions.OrderBy(point => Vector3.Distance(m_mainSelectedGameObject.transform.position, point)).ToArray();
        }

        private void SnapSelectedObjectToGrid(GameObject selectedGameobject)
        {
            if (selectedGameobject != null && PrefabUtility.IsPartOfAnyPrefab(selectedGameobject))
            {
                selectedGameobject = PrefabUtility.GetOutermostPrefabInstanceRoot(selectedGameobject);
            }

            HexTileOverrides tileOverrides = GetTileOverrides(selectedGameobject);

            if (m_enablePositionSnapping)
            {
                float nearestDist = float.MaxValue;
                Vector3 position = Vector3.zero;

                for (int i = 0; i < m_cellPositions.Length; ++i)
                {
                    Vector3 posOffset = tileOverrides != null ? tileOverrides.PositionOffset() : Vector3.zero;

                    float dist = Vector3.Distance(selectedGameobject.transform.position, m_cellPositions[i] + posOffset);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        position = m_cellPositions[i];
                    }
                }

                Vector3 newPos = tileOverrides != null ? position + tileOverrides.PositionOffset() : position;

                selectedGameobject.transform.position = newPos;
            }

            if (!m_enableRotationSnapping || tileOverrides == null || tileOverrides.AlwaysDisableRotationSnapping() == false)
            {
                if (selectedGameobject != null && GUIUtility.hotControl == 0)
                {
                    Vector3 currentAngle = selectedGameobject.transform.eulerAngles;
                    currentAngle = new Vector3(currentAngle.x, (Mathf.Round(currentAngle.y / m_rotationIncrement) * m_rotationIncrement), currentAngle.z);
                    selectedGameobject.transform.eulerAngles = currentAngle;
                }
            }

            if (tileOverrides != null && tileOverrides.DisplayPositionOffsetDebug())
            {
                DisplayPositionOffsetDebug(selectedGameobject.transform.position, tileOverrides.PositionOffset());
            }
        }

        private void DisplayPositionOffsetDebug(Vector3 pos, Vector3 offset)
        {
            Debug.DrawLine(pos, pos - offset, Color.blue);
        }

        private void OnSelectionChanged()
        {
            m_selectedGameObjects = Selection.gameObjects;

            if (Selection.activeGameObject != null && PrefabUtility.IsPartOfAnyPrefab(Selection.activeGameObject))
            {
                m_mainSelectedGameObject = PrefabUtility.GetOutermostPrefabInstanceRoot(Selection.activeGameObject);
            }

            if (m_mainSelectedGameObject != null)
            {
                HexTileOverrides tileOverrides = GetTileOverrides(m_mainSelectedGameObject);

                if (m_randomizeCellRotationsOnClick)
                {
                    if (tileOverrides == null || (tileOverrides != null && !tileOverrides.AlwaysDisableRotationChangeOnClick()))
                    {
                        Vector3 eulerAngles = new Vector3(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
                        eulerAngles = new Vector3(0.0f, (Mathf.Round(eulerAngles.y / m_rotationIncrement) * m_rotationIncrement), 0.0f);
                        m_mainSelectedGameObject.transform.eulerAngles = eulerAngles;
                    }
                }

                if (tileOverrides != null && tileOverrides.EnableRandomScalingOnClick())
                {
                    Vector3 minScaling = tileOverrides.MinScalingOffset();
                    Vector3 maxScaling = tileOverrides.MaxScalingOffset();

                    Vector3 newScaling = Vector3.zero;
                    if (tileOverrides.UseSingleScaleFactor())
                    {
                        float scaling = Random.Range(minScaling.x, maxScaling.x);
                        newScaling = new Vector3(scaling, scaling, scaling);
                    }
                    else
                    {
                        newScaling = new Vector3(Random.Range(minScaling.x, maxScaling.x), Random.Range(minScaling.y, maxScaling.y), Random.Range(minScaling.z, maxScaling.z));
                    }

                    m_mainSelectedGameObject.transform.localScale = newScaling;
                }
            }
        }

        private HexTileOverrides GetTileOverrides(GameObject gameObject)
        {
            if (gameObject != null)
            {
                if (gameObject.TryGetComponent(out HexTileOverrides hexTileOverrides))
                {
                    return hexTileOverrides;
                }
            }

            return null;
        }
    }
}

#endif