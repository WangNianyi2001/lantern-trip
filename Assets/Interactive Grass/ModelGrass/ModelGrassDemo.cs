using System;
using UnityEngine;

namespace InteractiveGrass
{
	public class ModelGrassDemo : MonoBehaviour
	{
		public GameObject m_Character;
		public KeyCode m_ForwardButton = KeyCode.UpArrow;
		public KeyCode m_BackwardButton = KeyCode.DownArrow;
		public KeyCode m_RightButton = KeyCode.RightArrow;
		public KeyCode m_LeftButton = KeyCode.LeftArrow;
		ModelGrass[] m_Grasses;

		void Start()
		{
			QualitySettings.antiAliasing = 8;
			m_Grasses = GameObject.FindObjectsOfType<ModelGrass>();
			for (int i = 0; i < m_Grasses.Length; i++)
				m_Grasses[i].Initialize();
		}
		void Update()
		{
			Vector3 dir = Vector3.zero;
			Move(m_ForwardButton, ref dir, m_Character.transform.forward);
			Move(m_BackwardButton, ref dir, -m_Character.transform.forward);
			Move(m_RightButton, ref dir, m_Character.transform.right);
			Move(m_LeftButton, ref dir, -m_Character.transform.right);
			m_Character.transform.position += dir * 4f * Time.deltaTime;

			for (int i = 0; i < m_Grasses.Length; i++)
				m_Grasses[i].DoUpdate();
		}
		void OnGUI()
		{
			GUI.Box(new Rect(10, 10, 200, 25), "Interactive Grass Demo");
			if (GUI.Button(new Rect(10, 40, 80, 30), "Burn Out"))
			{
				Array.Sort(m_Grasses, delegate(ModelGrass a, ModelGrass b) {
					Vector3 posA = a.gameObject.transform.position;
					Vector3 posB = b.gameObject.transform.position;
					Vector3 posCharacter = m_Character.transform.position;
					float distA = Vector3.Distance(posA, posCharacter);
					float distB = Vector3.Distance(posB, posCharacter);
					return distA.CompareTo(distB);
				});
				for (int i = 0; i < m_Grasses.Length; i++)
					m_Grasses[i].BurnOutStart(i * 0.1f, 2f);
			}
			if (GUI.Button(new Rect(95, 40, 80, 30), "Reset"))
			{
				for (int i = 0; i < m_Grasses.Length; i++)
					m_Grasses[i].BurnOutStop();
			}
		}
		void Move(KeyCode key, ref Vector3 moveTo, Vector3 dir)
		{
			if (Input.GetKey(key))
				moveTo = dir;
		}
	}
}