using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveGrass
{
	public class GPUGrass : MonoBehaviour
	{
		[Header("Trail")]
		public GameObject[] m_InteractiveObjs;
		public Texture2D m_Trail;
		public int m_TrailMapSize = 256;
		public float m_Radius = 1f;
		public Vector3 m_Offset = Vector3.up;
		public float m_MaxDistance = 1f;
		public LayerMask m_GrassLayer;
		Transform m_GrassTsf;
		Renderer m_GrassRdr;

		void Start()
		{
			m_GrassTsf = GetComponent<Transform>();
			m_GrassRdr = GetComponent<Renderer>();

			Color[] pixels = new Color[m_TrailMapSize * m_TrailMapSize];
			for (int i = 0; i < pixels.Length; i++)
				pixels[i] = new Color(0.5f, 0.5f, 1f, 1f);

			m_Trail = new Texture2D(m_TrailMapSize, m_TrailMapSize, TextureFormat.ARGB32, false, true);
			m_Trail.name = "GrassTrail";
			m_Trail.SetPixels(pixels);
			m_Trail.Apply();
		}
		void Update()
		{
			for (int i = 0; i < m_InteractiveObjs.Length; i++)
				RoundDisplacement(i);

			m_Trail.Apply();
			if (m_GrassRdr)
				m_GrassRdr.material.SetTexture("_TrailTex", m_Trail);
		}
		void RoundDisplacement(int ind)
		{
			Transform tsf = m_InteractiveObjs[ind].GetComponent<Transform>();

			// send GPU where the obj's world space position
			string forceCenterName = string.Format("_ForceCenter{0}", (ind + 1));
			m_GrassRdr.material.SetVector(forceCenterName, tsf.position);

			Vector3 rayOrigin = tsf.TransformPoint(m_Offset);
			Vector2 texCoord, texForward, texRight;
			Transform target = GetWorldToTextureSpaceMatrix(rayOrigin, Vector3.down, m_MaxDistance, m_GrassLayer, out texCoord, out texForward, out texRight);
			if (target == null)
			{
//				Debug.Log(ind + " not collide any grass...");
				return;
			}
			if (target != m_GrassTsf)
			{
//				Debug.Log(ind + " not collide grass " + name);
				return;
			}

			// invert the 2x2 world to texture space matrix.
			Vector2 inverseTexForward, inverseTexRight;
			Invert2x2Matrix(texForward, texRight, out inverseTexForward, out inverseTexRight);
			inverseTexForward.Normalize();
			inverseTexRight.Normalize();

			// convert the world space radius to pixel radius in texture space. Requires square textures.
			int pixelRadius = (int)(m_Radius * texForward.magnitude * m_Trail.width);

			// calculate the pixel coordinates of the point where the raycast hit the texture.
			Vector2 mid = new Vector2(texCoord.x * m_Trail.width, texCoord.y * m_Trail.height);

			// calculate the pixel area where the texture will be changed
			int targetX = (int)(mid.x - pixelRadius);
			int targetY = (int)(mid.y - pixelRadius);
			int rectX = Mathf.Clamp(targetX, 0, m_Trail.width);
			int rectY = Mathf.Clamp(targetY, 0, m_Trail.height);
			int width = Mathf.Min(targetX + pixelRadius * 2, m_Trail.width) - targetX;
			int height = Mathf.Min(targetY + pixelRadius * 2, m_Trail.height) - targetY;

			mid -= new Vector2(targetX, targetY);

			Color[] pixels = m_Trail.GetPixels(rectX, rectY, width, height);
			Vector3 forward = tsf.forward;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					Color pixel = pixels[x + y * width];

					Vector2 dir = (new Vector2(x, y) - mid) / pixelRadius;

					// texture space to world space
					dir = dir.x * inverseTexRight + dir.y * inverseTexForward;

					float pressure = 1f - CalcFalloff(dir.magnitude, 1f);

					// check angle and pressure threshold
					Vector2 characterForward = new Vector2(forward.x, forward.z);
					if (pixel.b > 0.5f && Vector2.Angle(dir, characterForward) < 180)
					{
						Vector2 displacementDir = dir;
						float falloff = CalcFalloff(dir.magnitude, 0.5f);
						dir = displacementDir.normalized * falloff;
						dir = VectorToColorSpace(dir);   // vector to color space
						pixels[x + y * width] = new Color(dir.x, dir.y, pressure, 1);
					}
				}
			}
			m_Trail.SetPixels(rectX, rectY, width, height, pixels);
//			m_Trail.Apply();
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		static Vector2 VectorToColorSpace(Vector2 v)
		{
			return (v + Vector2.one) * 0.5f;
		}
		static Vector2 GetTexCoordDifference(Vector3 pos, Vector2 texCoords, Vector3 offset, float maxDistance, LayerMask layerMask)
		{
			RaycastHit hit;
			Ray ray = new Ray(pos + offset, Vector3.down);
			if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
			{
				return (hit.textureCoord - texCoords) / offset.magnitude;
			}
			ray.direction = -ray.direction;
			if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
			{
				return (texCoords - hit.textureCoord) / offset.magnitude;
			}
			return Vector2.zero;
		}
		static Transform GetWorldToTextureSpaceMatrix(Vector3 pos, Vector3 rayDir, float maxDistance, LayerMask layerMask, out Vector2 texCoord, out Vector2 forward, out Vector2 right)
		{
			Ray ray = new Ray(pos, rayDir.normalized);
			RaycastHit hit;
//			Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
			if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
			{
				texCoord = hit.textureCoord;
				forward = GetTexCoordDifference(pos, texCoord, Vector3.forward * 0.1f, maxDistance, layerMask);
				right = GetTexCoordDifference(pos, texCoord, Vector3.right * 0.1f, maxDistance, layerMask);
				return hit.transform;
			}
			texCoord = Vector2.zero;
			forward = Vector2.zero;
			right = Vector2.zero;
			return null;
		}
		static float CalcFalloff(float distance, float falloff)
		{
			return Mathf.Pow(Mathf.Max(1f - distance, 0f), falloff);
		}
		static void Invert2x2Matrix(Vector2 forward, Vector2 right, out Vector2 inverseForward, out Vector2 inverseRight)
		{
			float det = right.x * forward.y - right.y * forward.x;
			inverseRight = new Vector2(forward.y, -right.y) / det;
			inverseForward = new Vector2(-forward.x, right.x) / det;
		}
	}
}