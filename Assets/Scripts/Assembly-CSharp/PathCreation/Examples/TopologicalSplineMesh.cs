using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathCreation.Examples
{
	public class TopologicalSplineMesh : PathSceneTool
	{
		[Header("Path Settings")]
		public float pathWidth = 0.4f;

		public bool flattenSurface = true;

		public float textureTiling = 1f;

		public Color vertexColor = Color.red;

		public Material pathMaterial;

		[Header("Endpoint Settings")]
		public bool startMarker;

		public bool endMarker;

		public Sprite endpointSprite;

		public Material endpointMaterial;

		public float endpointScale = 1f;

		public Color endpointColor = Color.black;

		[Header("Text Settings")]
		public string trailName = "";

		[Range(0f, 1f)]
		public List<float> trailNameTimes = new List<float> { 0.5f };

		[Range(-0.5f, 5f)]
		public float trailNameSpacingAdjustment = 2f;

		public float trailNameOffset;

		public Font trailFont;

		public float characterSize = 1f;

		public Color trailNameColor = Color.red;

		public FontStyle trailFontStyle;

		public Material letterMaterial;

		private GameObject start;

		private GameObject end;

		private MeshFilter meshFilter;

		private MeshRenderer meshRenderer;

		private Mesh mesh;

		protected override void PathUpdated()
		{
			if (pathCreator != null)
			{
				AssignMeshComponents();
				AssignMaterials();
				CreateRoadMesh();
			}
		}

		private void CreateRoadMesh()
		{
			Vector3[] array = new Vector3[base.path.NumPoints * 8];
			Vector2[] array2 = new Vector2[array.Length];
			Vector3[] array3 = new Vector3[array.Length];
			Color[] array4 = new Color[array.Length];
			int[] array5 = new int[(2 * (base.path.NumPoints - 1) + (base.path.isClosedLoop ? 2 : 0)) * 3];
			int num = 0;
			int num2 = 0;
			int[] array6 = new int[6] { 0, 2, 1, 1, 2, 3 };
			bool flag = base.path.space != PathSpace.xyz || !flattenSurface;
			for (int i = 0; i < base.path.NumPoints; i++)
			{
				Vector3 vector = (flag ? Vector3.Cross(base.path.GetTangent(i), base.path.GetNormal(i)) : base.path.up);
				Vector3 vector2 = (flag ? base.path.GetNormal(i) : Vector3.Cross(vector, base.path.GetTangent(i)));
				Vector3 vector3 = base.path.GetPoint(i) - vector2 * Mathf.Abs(pathWidth);
				Vector3 vector4 = base.path.GetPoint(i) + vector2 * Mathf.Abs(pathWidth);
				array[num] = vector3;
				array[num + 1] = vector4;
				array2[num] = new Vector2(0f, base.path.times[i] * base.path.length / textureTiling);
				array2[num + 1] = new Vector2(1f, base.path.times[i] * base.path.length / textureTiling);
				array3[num] = vector;
				array3[num + 1] = vector;
				array4[num] = vertexColor;
				array4[num + 1] = vertexColor;
				if (i < base.path.NumPoints - 1 || base.path.isClosedLoop)
				{
					for (int j = 0; j < array6.Length; j++)
					{
						array5[num2 + j] = (num + array6[j]) % array.Length;
					}
				}
				num += 2;
				num2 += 6;
			}
			mesh.Clear();
			mesh.vertices = array;
			mesh.uv = array2;
			mesh.colors = array4;
			mesh.normals = array3;
			mesh.subMeshCount = 3;
			mesh.SetTriangles(array5, 0);
			mesh.RecalculateBounds();
			UpdateEndpoint(start, startMarker, 0f);
			UpdateEndpoint(end, endMarker, 1f);
			UpdateTrailName();
		}

		private void AssignMeshComponents()
		{
			GameObject gameObject = base.gameObject;
			if (!gameObject.gameObject.GetComponent<MeshFilter>())
			{
				gameObject.gameObject.AddComponent<MeshFilter>();
			}
			if (!gameObject.GetComponent<MeshRenderer>())
			{
				gameObject.gameObject.AddComponent<MeshRenderer>();
			}
			meshRenderer = gameObject.GetComponent<MeshRenderer>();
			meshFilter = gameObject.GetComponent<MeshFilter>();
			if (mesh == null)
			{
				mesh = new Mesh();
			}
			meshFilter.sharedMesh = mesh;
			CreateEndpoint("Start", ref start);
			CreateEndpoint("End", ref end);
		}

		private void AssignMaterials()
		{
			if (pathMaterial != null)
			{
				meshRenderer.sharedMaterials = new Material[1] { pathMaterial };
			}
		}

		private void UpdateTrailName()
		{
			if (string.IsNullOrEmpty(trailName))
			{
				return;
			}
			foreach (Transform item in base.transform.GetChildren().ToList())
			{
				if (item.gameObject.name.StartsWith("Trail Name"))
				{
					Object.DestroyImmediate(item.gameObject);
				}
			}
			for (int i = 0; i < trailNameTimes.Count; i++)
			{
				GameObject gameObject = base.transform.Find("Trail Name " + i)?.gameObject;
				if (gameObject == null)
				{
					gameObject = new GameObject("Trail Name " + i);
					gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				}
				float num = base.path.length * trailNameTimes[i];
				float num2 = ((!(base.path.GetNormalAtDistance(num).z < 0f)) ? 1 : (-1));
				float num3 = 0f;
				string text = trailName;
				for (int j = 0; j < text.Length; j++)
				{
					char ch = text[j];
					if (trailFont.GetCharacterInfo(ch, out var info, 0, trailFontStyle))
					{
						GameObject obj = new GameObject(ch.ToString());
						obj.transform.SetParent(gameObject.transform, worldPositionStays: false);
						TextMesh textMesh = obj.AddComponent<TextMesh>();
						textMesh.font = trailFont;
						textMesh.fontStyle = trailFontStyle;
						textMesh.fontSize = 0;
						textMesh.text = ch.ToString();
						textMesh.color = trailNameColor;
						textMesh.characterSize = characterSize;
						textMesh.anchor = TextAnchor.MiddleLeft;
						textMesh.GetComponent<Renderer>().sharedMaterial = letterMaterial;
						float num4 = num + num3;
						float num5 = (float)info.advance * characterSize / 10f;
						float num6 = num4 + num5 * 0.5f * (0f - num2);
						Vector3 normalAtDistance = base.path.GetNormalAtDistance(num6);
						Vector3 normalAtDistance2 = base.path.GetNormalAtDistance(num6 + 0.1f);
						float num7 = Vector3.SignedAngle(normalAtDistance, normalAtDistance2, Vector3.up);
						textMesh.transform.position = base.path.GetPointAtDistance(num4) + normalAtDistance * trailNameOffset;
						textMesh.transform.rotation = Quaternion.LookRotation(-Vector3.up, normalAtDistance * num2);
						num3 += (num5 + trailNameSpacingAdjustment * num7) * (0f - num2);
					}
				}
			}
		}

		public void CreateEndpoint(string name, ref GameObject field)
		{
			if (field == null)
			{
				field = base.transform.Find(name)?.gameObject;
				if (field == null)
				{
					field = new GameObject(name);
					field.transform.SetParent(base.transform, worldPositionStays: false);
					field.AddComponent<SpriteRenderer>();
				}
			}
		}

		public void UpdateEndpoint(GameObject endpoint, bool active, float time)
		{
			if (endpoint != null)
			{
				endpoint.transform.forward = -Vector3.up;
				endpoint.transform.position = base.path.GetPointAtTime(time, EndOfPathInstruction.Stop);
				endpoint.transform.localScale = Vector3.one * endpointScale;
				SpriteRenderer component = endpoint.GetComponent<SpriteRenderer>();
				component.sprite = endpointSprite;
				component.color = endpointColor;
				component.sharedMaterial = endpointMaterial;
				endpoint.SetActive(active);
			}
		}
	}
}
