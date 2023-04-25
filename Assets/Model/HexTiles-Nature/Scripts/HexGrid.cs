#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HexGrid
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class HexGrid : MonoBehaviour
    {
        [SerializeField]
        private bool m_hideAllCells = false;
        //[SerializeField]
        //private bool m_hideLowerGridCells = true;

        [Space]
        [SerializeField]
        private string m_snappingLayerName = "SnapHexGrid";

        [SerializeField]
        private float m_hexTileSize = 0.808f;

        [SerializeField]
        private Color m_cellColor = Color.white;

        [Header("The number of chunks that should spawn")]
        [SerializeField]
        private int m_gridWidth = 10;
        [SerializeField]
        private int m_gridHeight = 10;
        [SerializeField]
        private int m_gridDepth = 5;

        [SerializeField]
        private int m_chunkSize = 8;

        [Space]
        [SerializeField]
        private int m_cellRenderDistance = 80;
        [SerializeField]
        private int m_maxRenderDistance = 300;

        [Space]
        [SerializeField]
        private bool m_displayDebugCellColors = false;

        [Header("This setting can cause lag when moving the grid")]
        [SerializeField]
        private bool m_moveGridInRealTime = false;

        [SerializeField, HideInInspector]
        private Transform m_cameraTransform = null;

        //[SerializeField]
        public HexGridCacheChunk[] HexGridCacheChunks; //{ get; private set; }

        private Vector3[] m_corners = new Vector3[m_cornerNum];
        private float m_innerRadius = 0.0f;
        private int m_fullChunkSize = 0;
        private bool m_reCalculateCellColors = false;
        private Vector3 m_previousHexGridPos = Vector3.zero;

        private const float m_innerRadiusMultiplier = 0.866025404f;
        private const int m_cornerNum = 6;

        //private readonly Vector3 m_debugOffset = new Vector3(0.0f, 2.0f, 0.0f);

        //stores a collection of cells as a chunk
        //[Serializable]
        public class HexGridCacheChunk
        {
            public HexGridCellCache[] hexGridCellCache = null;
            public Vector2Int offset = default;

            public readonly List<AdditionalLines> additionalLines = new List<AdditionalLines>();

            public Vector3 centerPos = default;
            public bool isRendered = false;
            public Color chunkDebugColor = Color.black;

            public HexGridCacheChunk(HexGridCellCache[] hexGridCellCache, Vector2Int offset)
            {
                this.hexGridCellCache = hexGridCellCache;
                this.offset = offset;
            }
        }

        //stores a cell
        //[Serializable]
        public class HexGridCellCache
        {
            public readonly HexGridLineCache[] hexGridLines = null;
            public readonly Vector3 pos = default;
            public Color cellColor = Color.white;

            public HexGridCellCache(HexGridLineCache[] hexGridLines, Vector3 pos, Color cellColor)
            {
                this.hexGridLines = hexGridLines;
                this.pos = pos;
                this.cellColor = cellColor;
            }
        }

        //stores the lines which make up a cell
        //[Serializable]
        public class HexGridLineCache
        {
            public readonly Vector3 startPos = default;
            public readonly Vector3 endPos = default;
            public readonly bool shouldRender = false;

            public HexGridLineCache(Vector3 startPos, Vector3 endPos, bool shouldRender)
            {
                this.startPos = startPos;
                this.endPos = endPos;
                this.shouldRender = shouldRender;
            }
        }

        [Serializable]
        public class AdditionalLines
        {
            public readonly Vector3 from = default;
            public readonly Vector3 to = default;
            public Color color = Color.white;

            public AdditionalLines(Vector3 from, Vector3 to, Color color)
            {
                this.from = from;
                this.to = to;
                this.color = color;
            }
        }

        private void OnValidate()
        {
            if (m_gridWidth < 0) m_gridWidth = 0;
            if (m_gridHeight < 0) m_gridHeight = 0;
            if (m_gridDepth < 0) m_gridDepth = 0;
            if (m_chunkSize < 0) m_chunkSize = 0;
            if (m_cellRenderDistance < 0) m_cellRenderDistance = 0;
            if (m_maxRenderDistance < 0) m_maxRenderDistance = 0;

            if (HexGridCacheChunks != null && m_reCalculateCellColors != m_displayDebugCellColors)
            {
                m_reCalculateCellColors = m_displayDebugCellColors;
                UpdateCellColors();
            }
        }

        private void OnEnable()
        {
            EditorApplication.update += Update;

            m_hideAllCells = false;

            SetupGrid();
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;

            m_hideAllCells = true;
        }

        private void SetupGrid()
        {
            Camera[] cameras = SceneView.GetAllSceneCameras();

            if (cameras != null && cameras.Length > 0)
            {
                m_cameraTransform = cameras[0].transform;
            }

            m_previousHexGridPos = transform.position;

            CalculateGridData();
        }

        private void OnDrawGizmos()
        {
            if (m_cameraTransform == null)
            {
                SetupGrid();

                return;
            }

            if (m_hideAllCells)
            {
                return;
            }

            for (int i = 0; i < HexGridCacheChunks.Length; ++i)
            {
                float distanceToChunk = Vector3.Distance(m_cameraTransform.position, HexGridCacheChunks[i].centerPos);

                //render lower chunk rects
                if (distanceToChunk < m_maxRenderDistance)
                {
                    for (int j = 0; j < HexGridCacheChunks[i].additionalLines?.Count; ++j)
                    {
                        AdditionalLines additionalLine = HexGridCacheChunks[i].additionalLines[j];

                        Gizmos.color = additionalLine.color;
                        Gizmos.DrawLine(additionalLine.from, additionalLine.to);
                    }
                }

                if (distanceToChunk > m_cellRenderDistance)
                {
                    HexGridCacheChunks[i].isRendered = false;
                    continue;
                }

                HexGridCacheChunks[i].isRendered = true;

                HexGridCellCache[] hexGridCellCache = HexGridCacheChunks[i].hexGridCellCache;
                for (int j = 0; j < hexGridCellCache.Length; ++j)
                {
                    RenderCell(hexGridCellCache[j]);
                }
            }
        }

        private void Update()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            //check if user is trying to move the hex grid itself
            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects != null && selectedGameObjects.Length > 0)
            {
                for (int i = 0; i < selectedGameObjects.Length; ++i)
                {
                    if (selectedGameObjects[i] == gameObject)
                    {
                        UpdateCellPositions();

                        break;
                    }
                }
            }
        }

        private void RenderCell(HexGridCellCache hexGridCellCache)
        {
            float distanceToCell = Vector3.Distance(m_cameraTransform.position, hexGridCellCache.pos);
            if (distanceToCell > m_cellRenderDistance || (true && Math.Abs(hexGridCellCache.pos.y - transform.position.y) > Mathf.Epsilon))
            {
                return;
            }

            Gizmos.color = hexGridCellCache.cellColor;

            HexGridLineCache[] hexGridLineCache = hexGridCellCache.hexGridLines;
            for (int k = 0; k < hexGridLineCache.Length; ++k)
            {
                if (hexGridLineCache[k].shouldRender)
                {
                    Gizmos.DrawLine(hexGridLineCache[k].startPos, hexGridLineCache[k].endPos);
                }
            }
        }

        public void CalculateGridData()
        {
            m_innerRadius = m_hexTileSize * m_innerRadiusMultiplier;
            float halfHexTileSize = 0.5f * m_hexTileSize;

            m_corners = new[]
            {
                new Vector3(0.0f, 0.0f, m_hexTileSize),
                new Vector3(m_innerRadius, 0.0f, halfHexTileSize),
                new Vector3(m_innerRadius, 0f, -halfHexTileSize),
                new Vector3(0.0f, 0.0f, -m_hexTileSize),
                new Vector3(-m_innerRadius, 0.0f, -halfHexTileSize),
                new Vector3(-m_innerRadius, 0.0f, halfHexTileSize)
            };

            m_fullChunkSize = m_chunkSize * m_chunkSize;

            HexGridCacheChunks = new HexGridCacheChunk[m_gridWidth * m_gridHeight];

            for (int x = 0; x < m_gridWidth; ++x)
            {
                for (int z = 0; z < m_gridHeight; ++z)
                {
                    int index = x + (z * m_gridWidth);
                    HexGridCacheChunks[index] = new HexGridCacheChunk(new HexGridCellCache[m_fullChunkSize * m_gridDepth], new Vector2Int(x * m_chunkSize, z * m_chunkSize));

                    ProcessChunk(HexGridCacheChunks[index], index);
                }
            }

            for (int i = 0; i < HexGridCacheChunks.Length; ++i)
            {
                string jsonData = EditorJsonUtility.ToJson(HexGridCacheChunks[i]);
                //Debug.Log(jsonData);
            }


        }

        private void ProcessChunk(HexGridCacheChunk chunk, int chunkIndex)
        {
            for (int y = 0; y < m_gridDepth; ++y)
            {
                for (int i = 0; i < m_fullChunkSize; ++i)
                {
                    int x = (i / m_chunkSize) + chunk.offset.x;
                    int z = (i % m_chunkSize) + chunk.offset.y;

                    chunk.hexGridCellCache[i + (y * m_fullChunkSize)] = ProcessCell(x, y, z, chunk);

                    if (y == m_gridDepth - 1)
                    {
                        HexGridCellCache gridCell = chunk.hexGridCellCache[i + (y * m_fullChunkSize)];
                        GenerateAdditionalLines(chunk, gridCell, i);
                    }
                }
            }
        }

        private void GenerateAdditionalLines(HexGridCacheChunk chunk, HexGridCellCache gridCell, int cellIndex)
        {
            if (cellIndex == 0 || cellIndex == m_chunkSize - 1 || cellIndex == m_fullChunkSize - m_chunkSize || cellIndex == m_fullChunkSize - 1)
            {
                //float haldHexTileSize = m_hexTileSize;
                chunk.additionalLines.Add(new AdditionalLines(new Vector3(gridCell.pos.x, chunk.centerPos.y, gridCell.pos.z), new Vector3(gridCell.pos.x, chunk.centerPos.y - (m_gridDepth * m_hexTileSize), gridCell.pos.z), gridCell.cellColor));
                //AdditionalLines tempLine = new AdditionalLines(new Vector3(gridCell.pos.x, chunk.centerPos.y, gridCell.pos.z), new Vector3(gridCell.pos.x, chunk.centerPos.y - (m_gridDepth * m_hexTileSize), gridCell.pos.z), gridCell.cellColor);

                int additionalLinesCount = chunk.additionalLines.Count;
                if (additionalLinesCount > 1 && cellIndex == m_fullChunkSize - 1)
                {
                    chunk.additionalLines.Add(new AdditionalLines(chunk.additionalLines[additionalLinesCount - 1].to, chunk.additionalLines[additionalLinesCount - 2].to, gridCell.cellColor));
                    chunk.additionalLines.Add(new AdditionalLines(chunk.additionalLines[additionalLinesCount - 1].to, chunk.additionalLines[additionalLinesCount - 3].to, gridCell.cellColor));

                    chunk.additionalLines.Add(new AdditionalLines(chunk.additionalLines[additionalLinesCount - 4].to, chunk.additionalLines[additionalLinesCount - 2].to, gridCell.cellColor));
                    chunk.additionalLines.Add(new AdditionalLines(chunk.additionalLines[additionalLinesCount - 4].to, chunk.additionalLines[additionalLinesCount - 3].to, gridCell.cellColor));
                }
            }
        }

        private HexGridCellCache ProcessCell(int x, int y, int z, HexGridCacheChunk chunk)
        {
            float hexTileMultiplier = m_hexTileSize * 1.5f;

            Vector3 position = transform.position + CalculatePositionOffset(m_gridWidth, m_gridHeight, hexTileMultiplier);

            position.y -= y * hexTileMultiplier;
            position.z += z * hexTileMultiplier;

            float xPos = (x + z * 0.5f - z * 0.5f) * (m_innerRadius * 2.0f);
            position.x += z % 2 == 0 ? xPos : xPos + m_innerRadius;

            int halfChunkSize = Mathf.RoundToInt((m_chunkSize * m_hexTileSize) * 1.0f);
            chunk.centerPos = new Vector3(position.x - halfChunkSize, transform.position.y, position.z - halfChunkSize);

            HexGridCellCache hexGridCellCache = new HexGridCellCache(new HexGridLineCache[m_cornerNum], position, SetCellColor(chunk, y));

            bool shouldRender = hexGridCellCache.pos.x == 0 || hexGridCellCache.pos.x == m_gridWidth - 1 || hexGridCellCache.pos.y == 0 || hexGridCellCache.pos.y == m_gridHeight - 1;

            for (int i = 0; i < m_cornerNum; ++i)
            {
                Vector3 startPos = i != 0 ? m_corners[i - 1] : m_corners[m_cornerNum - 1];

                hexGridCellCache.hexGridLines[i] = new HexGridLineCache(startPos + position, m_corners[i] + position, shouldRender || i % 2 == 0);
            }

            return hexGridCellCache;
        }

        private Vector3 CalculatePositionOffset(float width, float height, float multiplier)
        {
            float x = -((width * m_chunkSize) * m_innerRadius) + m_innerRadius;
            float z = -(((height * 0.5f) * m_chunkSize) * multiplier) + multiplier;

            return new Vector3(x, 0.0f, z);
        }

        private Color SetCellColor(HexGridCacheChunk chunk, int yPos)
        {
            if (m_displayDebugCellColors)
            {
                if (chunk.chunkDebugColor == Color.black)
                {
                    chunk.chunkDebugColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                }

                return chunk.chunkDebugColor;
            }
            else
            {
                //return yPos == 0 ? m_topCellsColor : m_lowerCellsColor;
                return m_cellColor;
            }
        }

        private void UpdateCellColors()
        {
            for (int i = 0; i < HexGridCacheChunks.Length; ++i)
            {
                HexGridCacheChunk hexGridCacheChunk = HexGridCacheChunks[i];

                for (int j = 0; j < hexGridCacheChunk.hexGridCellCache.Length; ++j)
                {
                    HexGridCellCache hexGridCellCache = hexGridCacheChunk.hexGridCellCache[j];
                    hexGridCellCache.cellColor = SetCellColor(hexGridCacheChunk, Mathf.RoundToInt(hexGridCellCache.pos.y));
                }

                for (int j = 0; j < hexGridCacheChunk.additionalLines.Count; ++j)
                {
                    hexGridCacheChunk.additionalLines[j].color = m_displayDebugCellColors ? hexGridCacheChunk.chunkDebugColor : m_cellColor;
                }
            }
        }

        private void UpdateCellPositions()
        {
            if (m_moveGridInRealTime && GUIUtility.hotControl != 0 || (GUIUtility.hotControl == 0 && (m_previousHexGridPos != transform.position)))
            {
                m_previousHexGridPos = transform.position;
                CalculateGridData();
            }
        }

        public string GetSnappingHexGridLayerName()
        {
            return m_snappingLayerName;
        }
    }
}

#endif