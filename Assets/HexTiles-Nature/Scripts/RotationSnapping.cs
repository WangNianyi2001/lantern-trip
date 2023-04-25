#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace HexGrid
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [RequireComponent(typeof(HexGrid))]
    public class RotationSnapping : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private HexGrid m_hexGrid = null;

        private const float m_rotationIncrement = 60.0f;

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
                    RotationSnapSelectedObject(selectedGameObjects[i]);
                }
            }
        }

        private void RotationSnapSelectedObject(GameObject selectedGameObject)
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
                if (GUIUtility.hotControl == 0)
                {
                    Vector3 currentAngle = selectedGameObject.transform.eulerAngles;
                    currentAngle = new Vector3(currentAngle.x, (Mathf.Round(currentAngle.y / m_rotationIncrement) * m_rotationIncrement), currentAngle.z);
                    selectedGameObject.transform.eulerAngles = currentAngle;
                }
            }
        }

        private void GetHexGrid()
        {
            if (TryGetComponent(out HexGrid hexGrid))
            {
                this.m_hexGrid = hexGrid;
            }
        }
    }
}

#endif