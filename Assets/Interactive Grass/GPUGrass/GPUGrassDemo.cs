using UnityEngine;

namespace InteractiveGrass
{
	public class GPUGrassDemo : MonoBehaviour
	{
		public GameObject[] m_Grass;
		public GameObject[] m_Character;
		public KeyCode m_ForwardButton = KeyCode.UpArrow;
		public KeyCode m_BackwardButton = KeyCode.DownArrow;
		public KeyCode m_RightButton = KeyCode.RightArrow;
		public KeyCode m_LeftButton = KeyCode.LeftArrow;
		Renderer[] m_GrassRdr;

		[Header("Grass")]
		[Range(4, 16)] public int m_Tessellation = 12;
		public bool m_DistanceLod = false;
		public float m_TessellationMinDistance = 1f;
		public float m_TessellationMaxDistance = 32f;
		[Range(0.05f, 1f)] public float m_WindStrength = 0.4f;
		public float m_ForceRange = 1.2f;
		[Range(1f, 5f)] public float m_ForceIntensity = 4f;
		public bool m_EnableTrail = true;

		void Start()
		{
			QualitySettings.antiAliasing = 8;

			m_GrassRdr = new Renderer[m_Grass.Length];
			for (int i = 0; i < m_Grass.Length; i++)
				m_GrassRdr[i] = m_Grass[i].GetComponent<Renderer>();
		}
		void Update()
		{
			Vector3 dir = Vector3.zero;
			for (int i = 0; i < m_Character.Length; i++)
			{
				GameObject c = m_Character[i];
				Move(m_ForwardButton, ref dir, c.transform.forward);
				Move(m_BackwardButton, ref dir, -c.transform.forward);
				Move(m_RightButton, ref dir, c.transform.right);
				Move(m_LeftButton, ref dir, -c.transform.right);
				c.transform.position += dir * 4f * Time.deltaTime;
			}
			for (int i = 0; i < m_Grass.Length; i++)
			{
				m_GrassRdr[i].material.SetFloat("_Tessellation", m_Tessellation);
				if (m_DistanceLod)
					m_GrassRdr[i].material.EnableKeyword("ENABLE_DIST_LOD");
				else
					m_GrassRdr[i].material.DisableKeyword("ENABLE_DIST_LOD");
				m_GrassRdr[i].material.SetFloat("_TessellationMinDist", m_TessellationMinDistance);
				m_GrassRdr[i].material.SetFloat("_TessellationMaxDist", m_TessellationMaxDistance);
				m_GrassRdr[i].material.SetFloat("_WindStrength", m_WindStrength);
				m_GrassRdr[i].material.SetFloat("_ForceRange", m_ForceRange);
				m_GrassRdr[i].material.SetFloat("_ForceIntensity", m_ForceIntensity);
				if (m_EnableTrail)
					m_GrassRdr[i].material.EnableKeyword("ENABLE_TRAIL");
				else
					m_GrassRdr[i].material.DisableKeyword("ENABLE_TRAIL");
			}
		}
		void Move(KeyCode key, ref Vector3 moveTo, Vector3 dir)
		{
			if (Input.GetKey(key))
				moveTo = dir;
		}
	}
}