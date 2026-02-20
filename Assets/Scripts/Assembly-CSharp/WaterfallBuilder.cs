using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterfallBuilder : MonoBehaviour
{
	public enum ColliderBuilderNode
	{
		Nothing = 0,
		CollisionRegion = 1,
		IgnoreRegion = 2
	}

	[Serializable]
	public struct Node
	{
		public Vector3 localPosition;

		public Quaternion localRotation;

		public float width;

		public Color color;

		public ColliderBuilderNode colliderRegion;

		public Vector3 GetWorldPosition(Transform parent)
		{
			return parent.TransformPoint(localPosition);
		}

		public Quaternion GetWorldRotation(Transform parent)
		{
			return parent.rotation * localRotation;
		}

		public void SetWorldPosition(Transform parent, Vector3 position)
		{
			localPosition = parent.InverseTransformPoint(position);
		}

		public void SetWorldRotation(Transform parent, Quaternion rotation)
		{
			localRotation = Quaternion.Inverse(parent.rotation) * rotation;
		}
	}

	private const string COLLISION_REGION_PREFIX = "CollisionRegion";

	public Node[] nodes;

	public int rowQuads = 1;

	public float verticalUVMultiplier = 0.5f;

	public float collisionThickness = 8f;

	public void BuildMesh()
	{
		if (!GetComponent<MeshFilter>())
		{
			base.gameObject.AddComponent<MeshFilter>();
		}
		MeshFilter component = GetComponent<MeshFilter>();
		rowQuads = Math.Max(rowQuads, 1);
		int num = rowQuads + 1;
		Mesh mesh = ((component.sharedMesh == null) ? new Mesh() : component.sharedMesh);
		List<Vector3> list = GenerateMeshVertices(num, 0, nodes.Length - 1);
		List<Vector2> list2 = new List<Vector2>();
		float num2 = 0f;
		for (int i = 0; i < nodes.Length; i++)
		{
			for (int j = 0; j < num; j++)
			{
				list2.Add(new Vector2((float)j / (float)(num - 1), num2));
			}
			if (i < nodes.Length - 1)
			{
				Vector3 vector = nodes[i + 1].localPosition - nodes[i].localPosition;
				vector.y *= verticalUVMultiplier;
				num2 += vector.magnitude;
			}
		}
		List<int> list3 = GenerateMeshTriangles(num, nodes.Length - 1);
		List<Color> list4 = new List<Color>();
		for (int k = 0; k < nodes.Length; k++)
		{
			for (int l = 0; l < num; l++)
			{
				list4.Add(nodes[k].color);
			}
		}
		mesh.Clear();
		mesh.vertices = list.ToArray();
		mesh.triangles = list3.ToArray();
		mesh.uv = list2.ToArray();
		mesh.colors = list4.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();
		mesh.RecalculateBounds();
		component.sharedMesh = mesh;
	}

	private List<int> GenerateMeshTriangles(int rowVertices, int length)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < rowVertices - 1; j++)
			{
				int item = i * rowVertices + j;
				int item2 = i * rowVertices + j + 1;
				int item3 = i * rowVertices + rowVertices + j;
				int item4 = i * rowVertices + rowVertices + j + 1;
				list.Add(item);
				list.Add(item2);
				list.Add(item3);
				list.Add(item2);
				list.Add(item4);
				list.Add(item3);
			}
		}
		return list;
	}

	private List<Vector3> GenerateMeshVertices(int rowVertices, int startIndex, int endIndex)
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = startIndex; i <= endIndex; i++)
		{
			Node node = nodes[i];
			Vector3 vector = node.localRotation * Vector3.right;
			float num = node.width / 2f;
			Vector3 a = node.localPosition + vector * num;
			Vector3 b = node.localPosition - vector * num;
			for (int j = 0; j < rowVertices; j++)
			{
				list.Add(Vector3.Lerp(a, b, (float)j / (float)(rowVertices - 1)));
			}
		}
		return list;
	}

	public void GenerateCollisionMeshes()
	{
		foreach (Transform item in base.transform.GetChildren().ToList())
		{
			if (item.gameObject.name.StartsWith("CollisionRegion"))
			{
				UnityEngine.Object.DestroyImmediate(item.gameObject);
			}
		}
		int num = 0;
		for (int i = 0; i < nodes.Length; i++)
		{
			switch (nodes[i].colliderRegion)
			{
			case ColliderBuilderNode.IgnoreRegion:
				num = i;
				break;
			case ColliderBuilderNode.CollisionRegion:
				GenerateCollisionMeshRegion(num, i);
				num = i;
				break;
			}
		}
		if (num != nodes.Length - 1)
		{
			GenerateCollisionMeshRegion(num, nodes.Length - 1);
		}
	}

	private void GenerateCollisionMeshRegion(int startIndex, int endIndex)
	{
		GameObject obj = new GameObject("CollisionRegion (" + startIndex + "-" + endIndex + ")");
		obj.transform.SetParent(base.transform, worldPositionStays: false);
		obj.layer = 4;
		Mesh mesh = new Mesh();
		List<Vector3> vertices = GenerateMeshVertices(2, startIndex, endIndex);
		List<int> list = GenerateMeshTriangles(2, endIndex - startIndex);
		list.AddRange(from i in list.ToArray()
			select i + vertices.Count);
		vertices.AddRange(from v in vertices.ToArray()
			select v + Vector3.down * collisionThickness);
		mesh.vertices = vertices.ToArray();
		mesh.triangles = list.ToArray();
		mesh.RecalculateBounds();
		MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
		meshCollider.convex = true;
		meshCollider.isTrigger = true;
		obj.AddComponent<WaterRegion>().waterHeightOffset = -0.5f;
		WaterCurrent waterCurrent = obj.AddComponent<WaterCurrent>();
		IEnumerable<Node> source = nodes.Skip(startIndex).Take(endIndex - startIndex + 1);
		waterCurrent.positions = source.Select((Node n) => n.GetWorldPosition(base.transform)).ToArray();
		waterCurrent.tangents = source.Select((Node n) => n.GetWorldRotation(base.transform) * Vector3.forward * n.width).ToArray();
	}
}
