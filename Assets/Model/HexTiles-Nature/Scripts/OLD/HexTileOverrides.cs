#if UNITY_EDITOR
using UnityEngine;

namespace HexTileGrid
{
    [DisallowMultipleComponent]
    public class HexTileOverrides : MonoBehaviour
    {
        [SerializeField] private Vector3 m_positionOffset = Vector3.zero;
        [SerializeField] private bool m_displayPositionOffsetDebug = false;

        [Space, Space]
        [SerializeField] private Vector3 m_minScalingOffset = Vector3.one;
        [SerializeField] private Vector3 m_maxScalingOffset = Vector3.one;
        [Header("If using single scale factor, only xMin and xMax will be used")]
        [SerializeField] private bool m_useSingleScaleFactor = true;
        [SerializeField] private bool m_enableRandomScalingOnClick = false;

        [Space, Space]
        [SerializeField] private bool m_alwaysDisableRotationSnapping = false;
        [SerializeField] private bool m_alwaysDisableRotationChangeOnClick = false;

        public Vector3 PositionOffset() => m_positionOffset;
        public bool DisplayPositionOffsetDebug() => m_displayPositionOffsetDebug;

        public Vector3 MinScalingOffset() => m_minScalingOffset;
        public Vector3 MaxScalingOffset() => m_maxScalingOffset;
        public bool UseSingleScaleFactor() => m_useSingleScaleFactor;
        public bool EnableRandomScalingOnClick() => m_enableRandomScalingOnClick;

        public bool AlwaysDisableRotationSnapping() => m_alwaysDisableRotationSnapping;
        public bool AlwaysDisableRotationChangeOnClick() => m_alwaysDisableRotationChangeOnClick;
    }
}
#endif