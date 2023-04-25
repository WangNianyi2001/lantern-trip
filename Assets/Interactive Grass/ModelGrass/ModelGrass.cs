using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveGrass
{
	public class ModelGrass : MonoBehaviour
	{
		public class Force
		{
			public float m_Time = 0f;
			public Vector3 m_Force;
			public Force(Vector3 force) { m_Force = force; }
		}
		[Range(0f, 8f)] public float m_Amplitude = 3f;
		[Range(0f, 1f)] public float m_BurnProgress = 0f;
		public float m_ColliderRadius = 1f;
		public float m_ColliderStrength = 2f;
		public float m_MaxForceMagnitude = 8f;
		public float m_WaveFrequency = 6f;
		public float m_Resistance = 0.25f;
		List<Force> m_ForceList = new List<Force>();
		Vector3 m_AccForce = Vector3.zero;
		Renderer m_Rd;

		// burn out parameters
		bool m_IsBurnOuting = false;
		float m_BurnOutStartTime = 0f;
		float m_BurnOutDurationTime = 0f;
		float m_BurnOutPassTime = 0f;

		public void Initialize()
		{
			m_Rd = GetComponent<Renderer>();
		
			// generate vertex color from bottom to top (0 ~ 1)
			MeshFilter mf = GetComponent<MeshFilter>();
			Mesh mh = mf.mesh;
		
			float maxh = 0f, minh = 0f;
			Vector3[] p = mh.vertices;
			for (int i = 0; i < p.Length; i++)
			{
				float h = p[i].y;
				minh = Mathf.Min (minh, h);
				maxh = Mathf.Max (maxh, h);
			}
			Color[] vc = new Color[p.Length];
			for (int i = 0; i < p.Length; i++)
			{
				float h = p[i].y;
				float f = h / (maxh - minh);
				vc[i] = new Color (f, f, f, 1f);
			}
			mh.colors = vc;
		}
		public void DoUpdate()
		{
			UpdateForce();
			BurnOutUpdate();
			m_Rd.material.SetFloat("_Amplitude", m_Amplitude);
			m_Rd.material.SetFloat("_BurnAmount", m_BurnProgress);
		}
		void AddForce(Vector3 force)
		{
			Vector3 f = new Vector3(force.x, force.y, force.z);
			if (f.magnitude > m_MaxForceMagnitude)
				f = f.normalized * m_MaxForceMagnitude;
			m_ForceList.Add(new Force(f));
		}
		void UpdateForce()
		{
			if (m_ForceList.Count == 0)
				return;

			m_AccForce = Vector3.zero;
			for (int i = m_ForceList.Count - 1; i >= 0; --i)
			{
				if (m_ForceList[i].m_Force.magnitude > 0.01f)
				{
					// wave force
					float wf = Mathf.Sin(m_ForceList[i].m_Time * m_WaveFrequency);

					// force damping
					float rf = easeOutExpo(1f, 0f, m_Resistance * Time.deltaTime);
					m_ForceList[i].m_Force *= rf;
					m_ForceList[i].m_Time += Time.deltaTime;

					// accumulate
					m_AccForce += m_ForceList[i].m_Force * wf;
				}
				else
				{
					m_ForceList.RemoveAt(i);
				}
			}
			m_AccForce = transform.InverseTransformVector(m_AccForce);  // convert to local space
			m_Rd.material.SetVector("_MoveVec", m_AccForce);
		}
		float easeOutExpo(float start, float end, float value)
		{
			end -= start;
			return end * (-Mathf.Pow(2f, -10f * value) + 1f) + start;
		}
		void OnTriggerEnter(Collider other)
		{
			Transform otherT = other.gameObject.GetComponent<Transform>();
			Transform selfT = GetComponent<Transform>();
			Vector3 dir = selfT.position - otherT.position;
			dir.y = 0f;

			float f = 0.25f + Mathf.Clamp01((m_ColliderRadius - dir.magnitude) / m_ColliderRadius) * 0.75f;
			AddForce(dir.normalized * f * m_ColliderStrength);
		}
		public void BurnOutStart(float delay, float duration)
		{
			m_BurnOutStartTime = Time.time + delay;
			m_BurnOutDurationTime = duration;
		}
		public void BurnOutStop()
		{
			// reset all burn out parameters
			m_BurnProgress = 0f;
			m_IsBurnOuting = false;
			m_BurnOutStartTime = m_BurnOutDurationTime = m_BurnOutPassTime = 0f;
		}
		void BurnOutUpdate()
		{
			if (m_BurnOutStartTime != 0f)
			{
				if (Time.time > m_BurnOutStartTime)
				{
					m_IsBurnOuting = true;
					m_BurnOutStartTime = 0f;
					m_BurnOutPassTime = 0f;
				}
			}
			if (m_IsBurnOuting)
			{
				m_BurnOutPassTime += Time.deltaTime;
				float ratio = m_BurnOutPassTime / m_BurnOutDurationTime;
				m_BurnProgress = Mathf.Lerp(0f, 1f, ratio);
				if (ratio > 1f)
				{
					// end of burn out
					m_IsBurnOuting = false;
					m_BurnOutStartTime = m_BurnOutPassTime = 0f;
				}
			}
		}
	}
}