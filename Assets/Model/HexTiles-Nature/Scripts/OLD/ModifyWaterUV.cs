#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace HexTileGrid
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteInEditMode]
    public class ModifyWaterUV : MonoBehaviour
    {
        private static readonly int waterRotateUV = Shader.PropertyToID("Vector1_5414065B");

        private MeshRenderer waterMeshRenderer = null;
        private GameObject rootObject = null;

        private void OnEnable()
        {
            waterMeshRenderer = gameObject.GetComponent<MeshRenderer>();
            rootObject = gameObject;

            Material matInstance = new Material(waterMeshRenderer.sharedMaterial);
            if (waterMeshRenderer.sharedMaterial.GetInstanceID() != matInstance.GetInstanceID())
            {
                waterMeshRenderer.sharedMaterial = matInstance;
            }
        }

        private void Update()
        {

            if (EditorApplication.isPlaying || waterMeshRenderer == null || waterMeshRenderer.sharedMaterial == null)
            {
                return;
            }

            if (rootObject != null)
            {
                if (PrefabUtility.IsPartOfAnyPrefab(rootObject))
                {
                    rootObject = PrefabUtility.GetOutermostPrefabInstanceRoot(rootObject);
                }

                waterMeshRenderer.sharedMaterial.SetFloat(waterRotateUV, -rootObject.transform.rotation.eulerAngles.y + 70.0f);
            }
        }
    }
}
#endif