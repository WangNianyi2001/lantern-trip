#if UNITY_EDITOR

using System.Collections.Generic;
using HexTileGrid;
using UnityEditor;
using UnityEngine;
using static HexGrid.HexGrid;

namespace HexGrid
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [RequireComponent(typeof(HexGrid))]
    public class GridSnapping : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private HexGrid m_hexGrid = null;

        private void OnEnable()
        {
            EditorApplication.update += Update;

            if (m_hexGrid == null)
            {
                GetHexGrid();
            }
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        private void OnDestroy()
        {
            EditorApplication.update -= Update;
        }

        private void Update()
        {
            if (EditorApplication.isPlaying || m_hexGrid == null)
            {
                return;
            }

            GameObject[] selectedGameObjects = Selection.gameObjects;

            if (selectedGameObjects != null && selectedGameObjects.Length > 0)
            {
                for (int i = 0; i < selectedGameObjects.Length; ++i)
                {
                    SnapSelectedObjectToGrid(selectedGameObjects[i]);
                }
            }
        }

        private void SnapSelectedObjectToGrid(GameObject selectedGameObject)
        {
            if (selectedGameObject != null && PrefabUtility.IsPartOfAnyPrefab(selectedGameObject))
            {
                selectedGameObject = PrefabUtility.GetOutermostPrefabInstanceRoot(selectedGameObject);
            }

            if (selectedGameObject == null)
            {
                return;
            }

            string layerName = m_hexGrid.GetSnappingHexGridLayerName();
            if (selectedGameObject.layer == LayerMask.NameToLayer(layerName) || string.IsNullOrEmpty(layerName))
            {
                List<HexGridCellCache[]> hexGridCellCache = FindNearestChunks(selectedGameObject);

                HexTileOverrides tileOverrides = GetTileOverrides(selectedGameObject);
                Vector3 positionOffset = tileOverrides != null ? tileOverrides.PositionOffset() : Vector3.zero;

                selectedGameObject.transform.position = FindNearestCell(hexGridCellCache, selectedGameObject, positionOffset) + positionOffset;
            }
        }

        private List<HexGridCellCache[]> FindNearestChunks(GameObject selectedGameObject)
        {
            float nearestDist = float.MaxValue;

            List<HexGridCellCache[]> hexGridCellCache = new List<HexGridCellCache[]>();
            for (int i = 0; i < m_hexGrid.HexGridCacheChunks.Length; ++i)
            {
                if (!m_hexGrid.HexGridCacheChunks[i].isRendered)
                {
                    continue;
                }

                if (FoundCloserDistance(selectedGameObject.transform.position, m_hexGrid.HexGridCacheChunks[i].centerPos, ref nearestDist))
                {
                    hexGridCellCache.Add(m_hexGrid.HexGridCacheChunks[i].hexGridCellCache);
                }
            }

            hexGridCellCache.Reverse();

            return hexGridCellCache;
        }

        private Vector3 FindNearestCell(List<HexGridCellCache[]> hexGridCellCache, GameObject selectedGameObject, Vector3 positionOffset)
        {
            float nearestDist = float.MaxValue;
            Vector3 position = Vector3.zero;

            int length = hexGridCellCache.Count > 4 ? 4 : hexGridCellCache.Count;

            for (int i = 0; i < length; ++i)
            {
                for (int j = 0; j < hexGridCellCache[i].Length; ++j)
                {
                    if (FoundCloserDistance(selectedGameObject.transform.position, hexGridCellCache[i][j].pos, ref nearestDist))
                    {
                        position = hexGridCellCache[i][j].pos;
                    }
                }
            }

            return position;
        }

        private bool FoundCloserDistance(Vector3 position1, Vector3 position2, ref float nearestDist)
        {
            float cellDist = Vector3.Distance(position1, position2);

            if (cellDist < nearestDist)
            {
                nearestDist = cellDist;

                return true;
            }

            return false;
        }

        private void GetHexGrid()
        {
            if (TryGetComponent(out HexGrid hexGrid))
            {
                this.m_hexGrid = hexGrid;
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