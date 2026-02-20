using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityMeshSimplifier;

public class TerrainBaker : MonoBehaviour
{
	public struct BakedTreeSet
	{
		public Matrix4x4[] matrices;

		public MaterialPropertyBlock instancedProperties;

		public Vector4[] colors;
	}

	[Serializable]
	public struct BakedTreeList
	{
		public List<BakedTree> treeList;

		public override string ToString()
		{
			return "Elements: " + treeList.Count;
		}
	}

	[Serializable]
	public struct BakedTree
	{
		public Vector4 color;

		public Vector3 position;

		public Vector3 scale;
	}

	[Serializable]
	public struct TreeTemplateData
	{
		public Mesh mesh;

		public Material material;

		public ShadowCastingMode shadowCastingMode;
	}

	public static bool BUNDLE_DRAW_CALLS;

	public static bool HIDE_TREES;

	[Header("Tree Render Settings")]
	public float smallTreeFlatCullDistance = 325f;

	[Header("Bake Quality")]
	public int submeshes = 4;

	public int vertices = 64;

	public int lowQualityVertices = 8;

	[Range(0f, 1f)]
	public float simplifierQuality = 0.5f;

	[Header("Material Settings")]
	public Shader addPassShader;

	public Material templateMaterial;

	[Header("Baked Data")]
	public Renderer[] flattenedTerrainRenderers;

	public BakedTreeList[] flattenedTreeList;

	public Mesh[] flattenedLowQualityList;

	public TreeTemplateData[] templateData;

	private Renderer[,] terrainRenderers;

	private BakedTreeSet[,,] treeSets;

	private Mesh[,] lowQualityMeshes;

	private Mesh[,] highQualityMeshes;

	private Vector3[,] terrainRendererCenters;

	private const int MAX_TREES = 1023;

	private Matrix4x4[][] bundledTrees;

	private Vector4[][] bundledColors;

	private int[] bundledCounts;

	private MaterialPropertyBlock bundledMaterialBlock;

	private Terrain terrain;

	private Material[] sharedPassMaterials;

	private Camera mainCamera;

	private int smallTreeLayers;

	private void Awake()
	{
		terrain = GetComponent<Terrain>();
		mainCamera = Camera.main;
		terrainRenderers = flattenedTerrainRenderers.Unflatten2DArray(submeshes, submeshes);
		smallTreeLayers = 0;
		for (int i = 0; i < templateData.Length; i++)
		{
			if (templateData[i].material.name.Contains("Bush") || templateData[i].material.name.Contains("Flower"))
			{
				smallTreeLayers |= 1 << i;
			}
		}
		UpdateTreePrototypeQuality();
		if (!GameSettings.useBakedTerrain)
		{
			base.enabled = false;
		}
	}

	private void Start()
	{
		lowQualityMeshes = flattenedLowQualityList.Unflatten2DArray(submeshes, submeshes);
		highQualityMeshes = new Mesh[submeshes, submeshes];
		terrainRendererCenters = new Vector3[submeshes, submeshes];
		BakedTreeList[,,] array = flattenedTreeList.Unflatten3DArray(submeshes, submeshes, templateData.Length);
		treeSets = new BakedTreeSet[submeshes, submeshes, terrain.terrainData.treePrototypes.Length];
		for (int i = 0; i < submeshes; i++)
		{
			for (int j = 0; j < submeshes; j++)
			{
				highQualityMeshes[j, i] = terrainRenderers[j, i].GetComponent<MeshFilter>().sharedMesh;
				terrainRendererCenters[j, i] = terrainRenderers[j, i].bounds.center;
				terrainRendererCenters[j, i].y = terrainRenderers[j, i].bounds.max.y;
				for (int k = 0; k < templateData.Length; k++)
				{
					List<BakedTree> treeList = array[j, i, k].treeList;
					if (treeList == null || treeList.Count == 0)
					{
						treeSets[j, i, k] = default(BakedTreeSet);
						continue;
					}
					MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
					materialPropertyBlock.SetVectorArray("_TreeInstanceColor", ((IEnumerable<BakedTree>)treeList).Select((Func<BakedTree, Vector4>)((BakedTree t) => (Color)t.color)).ToArray());
					treeSets[j, i, k] = new BakedTreeSet
					{
						matrices = treeList.Select((BakedTree t) => Matrix4x4.TRS(t.position, Quaternion.identity, t.scale)).ToArray(),
						instancedProperties = materialPropertyBlock,
						colors = ((IEnumerable<BakedTree>)treeList).Select((Func<BakedTree, Vector4>)((BakedTree t) => (Color)t.color)).ToArray()
					};
				}
			}
		}
	}

	private void UpdateTreePrototypeQuality()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		for (int i = 0; i < templateData.Length; i++)
		{
			if (templateData[i].material.name.Contains("Bush"))
			{
				templateData[i].shadowCastingMode = ((!GameSettings.useLowQualityOptimizations) ? ShadowCastingMode.On : ShadowCastingMode.Off);
			}
		}
	}

	private void OnEnable()
	{
		Renderer[,] array = terrainRenderers;
		int upperBound = array.GetUpperBound(0);
		int upperBound2 = array.GetUpperBound(1);
		for (int i = array.GetLowerBound(0); i <= upperBound; i++)
		{
			for (int j = array.GetLowerBound(1); j <= upperBound2; j++)
			{
				array[i, j].gameObject.SetActive(value: true);
			}
		}
		terrain.enabled = false;
	}

	private void OnDisable()
	{
		Renderer[,] array = terrainRenderers;
		int upperBound = array.GetUpperBound(0);
		int upperBound2 = array.GetUpperBound(1);
		for (int i = array.GetLowerBound(0); i <= upperBound; i++)
		{
			for (int j = array.GetLowerBound(1); j <= upperBound2; j++)
			{
				array[i, j].gameObject.SetActive(value: false);
			}
		}
		terrain.enabled = true;
	}

	public void SetQuality(bool highQuality)
	{
		for (int i = 0; i < submeshes; i++)
		{
			for (int j = 0; j < submeshes; j++)
			{
				terrainRenderers[j, i].GetComponent<MeshFilter>().sharedMesh = (highQuality ? highQualityMeshes[j, i] : lowQualityMeshes[j, i]);
			}
		}
	}

	private void Update()
	{
		if (!HIDE_TREES)
		{
			if (BUNDLE_DRAW_CALLS)
			{
				DrawMeshesBundled();
			}
			else
			{
				DrawMeshes();
			}
		}
	}

	private void DrawMeshes()
	{
		Vector3 position = mainCamera.transform.position;
		for (int i = 0; i < submeshes; i++)
		{
			for (int j = 0; j < submeshes; j++)
			{
				if (!terrainRenderers[j, i].isVisible)
				{
					continue;
				}
				bool flag = (position - terrainRendererCenters[j, i]).SetY(0f).sqrMagnitude < smallTreeFlatCullDistance * smallTreeFlatCullDistance;
				for (int k = 0; k < templateData.Length; k++)
				{
					BakedTreeSet bakedTreeSet = treeSets[j, i, k];
					bool flag2 = (smallTreeLayers & (1 << k)) > 0;
					if (bakedTreeSet.matrices != null && (flag || !flag2))
					{
						TreeTemplateData treeTemplateData = templateData[k];
						Graphics.DrawMeshInstanced(treeTemplateData.mesh, 0, treeTemplateData.material, bakedTreeSet.matrices, bakedTreeSet.matrices.Length, bakedTreeSet.instancedProperties, treeTemplateData.shadowCastingMode, receiveShadows: true);
					}
				}
			}
		}
	}

	private void DrawMeshesBundled()
	{
		if (bundledTrees == null)
		{
			bundledTrees = new Matrix4x4[templateData.Length][];
			bundledColors = new Vector4[templateData.Length][];
			bundledCounts = new int[templateData.Length];
			bundledMaterialBlock = new MaterialPropertyBlock();
			for (int i = 0; i < templateData.Length; i++)
			{
				bundledTrees[i] = new Matrix4x4[1023];
				bundledColors[i] = new Vector4[1023];
			}
		}
		for (int j = 0; j < templateData.Length; j++)
		{
			bundledCounts[j] = 0;
		}
		for (int k = 0; k < submeshes; k++)
		{
			for (int l = 0; l < submeshes; l++)
			{
				if (!terrainRenderers[l, k].isVisible)
				{
					continue;
				}
				for (int m = 0; m < templateData.Length; m++)
				{
					BakedTreeSet bakedTreeSet = treeSets[l, k, m];
					if (bakedTreeSet.matrices != null)
					{
						int num = bundledCounts[m];
						if (num + bakedTreeSet.matrices.Length < 1023)
						{
							Array.Copy(bakedTreeSet.matrices, 0, bundledTrees[m], num, bakedTreeSet.matrices.Length);
							Array.Copy(bakedTreeSet.colors, 0, bundledColors[m], num, bakedTreeSet.colors.Length);
							bundledCounts[m] += bakedTreeSet.matrices.Length;
						}
					}
				}
			}
		}
		for (int n = 0; n < templateData.Length; n++)
		{
			TreeTemplateData treeTemplateData = templateData[n];
			bundledMaterialBlock.SetVectorArray("_TreeInstanceColor", bundledColors[n]);
			Graphics.DrawMeshInstanced(treeTemplateData.mesh, 0, treeTemplateData.material, bundledTrees[n], bundledCounts[n], bundledMaterialBlock, treeTemplateData.shadowCastingMode, receiveShadows: true);
		}
	}

	public void BakeTerrain()
	{
		if (terrain == null)
		{
			terrain = GetComponent<Terrain>();
		}
		Transform[] array = base.transform.GetChildren().ToArray();
		foreach (Transform transform in array)
		{
			if (transform.gameObject.name.StartsWith("_Baked"))
			{
				UnityEngine.Object.DestroyImmediate(transform.gameObject);
			}
		}
		int num = terrain.terrainData.terrainLayers.Length;
		if (num <= 4 || num >= 9)
		{
			GeneratePassMaterials();
		}
		else
		{
			sharedPassMaterials = new Material[1] { CreateDoublePassMaterial() };
		}
		Renderer[,] array2 = new Renderer[submeshes, submeshes];
		lowQualityMeshes = new Mesh[submeshes, submeshes];
		for (int j = 0; j < submeshes; j++)
		{
			for (int k = 0; k < submeshes; k++)
			{
				Vector2 position = new Vector2((float)k / (float)submeshes, (float)j / (float)submeshes);
				Rect subsection = new Rect(position, Vector2.one * (1f / (float)submeshes));
				GameObject gameObject = CreateBakedGameObject(k, j);
				gameObject.GetComponent<MeshFilter>().sharedMesh = GenerateMesh(vertices, vertices, subsection);
				lowQualityMeshes[k, j] = GenerateMesh(lowQualityVertices, lowQualityVertices, subsection);
				array2[k, j] = gameObject.GetComponent<Renderer>();
			}
		}
		flattenedTerrainRenderers = array2.Flatten2DArray();
		flattenedLowQualityList = lowQualityMeshes.Flatten2DArray();
		BakeTerrainTrees();
		Debug.LogWarning("Don't forget to re-bake occlusion!");
	}

	private void BakeTerrainTrees()
	{
		templateData = terrain.terrainData.treePrototypes.Select((TreePrototype p) => new TreeTemplateData
		{
			mesh = p.prefab.GetComponent<MeshFilter>().sharedMesh,
			material = p.prefab.GetComponent<Renderer>().sharedMaterial,
			shadowCastingMode = p.prefab.GetComponent<Renderer>().shadowCastingMode
		}).ToArray();
		TreePrototype[] treePrototypes = terrain.terrainData.treePrototypes;
		BakedTreeList[,,] array = new BakedTreeList[submeshes, submeshes, templateData.Length];
		TreeInstance[] treeInstances = terrain.terrainData.treeInstances;
		for (int num = 0; num < treeInstances.Length; num++)
		{
			TreeInstance treeInstance = treeInstances[num];
			int num2 = (int)(treeInstance.position.x * (float)submeshes);
			int num3 = (int)(treeInstance.position.z * (float)submeshes);
			int prototypeIndex = treeInstance.prototypeIndex;
			if (array[num2, num3, prototypeIndex].treeList == null)
			{
				array[num2, num3, prototypeIndex].treeList = new List<BakedTree>();
			}
			array[num2, num3, prototypeIndex].treeList.Add(new BakedTree
			{
				position = terrain.GetPosition() + Vector3.Scale(treeInstance.position, terrain.terrainData.size),
				color = (Color)treeInstance.color,
				scale = treePrototypes[prototypeIndex].prefab.transform.localScale * treeInstance.widthScale
			});
		}
		flattenedTreeList = array.Flatten3DArray();
		BakedTreeList[] second = array.Cast<BakedTreeList>().ToArray();
		Debug.Log(flattenedTreeList.SequenceEqual(second));
	}

	private void GeneratePassMaterials()
	{
		Material materialTemplate = terrain.materialTemplate;
		TerrainLayer[] terrainLayers = terrain.terrainData.terrainLayers;
		List<Material> list = new List<Material>();
		for (int i = 0; i < terrainLayers.Length; i += 4)
		{
			Material material = CreatePassMaterial(i, materialTemplate);
			if (i > 0)
			{
				material.shader = addPassShader;
			}
			list.Add(material);
		}
		sharedPassMaterials = list.ToArray();
	}

	public GameObject CreateBakedGameObject(int x, int y)
	{
		GameObject obj = new GameObject("_Baked" + x + "_" + y);
		obj.transform.parent = base.transform;
		obj.AddComponent<MeshFilter>();
		obj.AddComponent<MeshRenderer>().sharedMaterials = sharedPassMaterials;
		obj.layer = base.gameObject.layer;
		return obj;
	}

	public Material CreateDoublePassMaterial()
	{
		TerrainLayer[] terrainLayers = terrain.terrainData.terrainLayers;
		Material material = new Material(templateMaterial);
		for (int i = 0; i < Math.Min(8, terrainLayers.Length); i++)
		{
			TerrainLayer terrainLayer = terrainLayers[i];
			material.SetTexture("_Splat" + i, terrainLayer.diffuseTexture);
			material.SetTextureScale(value: new Vector2(512f / terrainLayer.tileSize.x, 512f / terrainLayer.tileSize.y), name: "_Splat" + i);
		}
		material.SetTexture("_Control1", terrain.terrainData.GetAlphamapTexture(0));
		material.SetTexture("_Control2", terrain.terrainData.GetAlphamapTexture(1));
		return material;
	}

	public Material CreatePassMaterial(int startIndex, Material template)
	{
		TerrainLayer[] terrainLayers = terrain.terrainData.terrainLayers;
		Material material = new Material(template);
		for (int i = 0; i < Math.Min(4, terrainLayers.Length - startIndex); i++)
		{
			TerrainLayer terrainLayer = terrainLayers[startIndex + i];
			material.SetTexture("_Splat" + i, terrainLayer.diffuseTexture);
			material.SetTextureScale(value: new Vector2(512f / terrainLayer.tileSize.x, 512f / terrainLayer.tileSize.y), name: "_Splat" + i);
		}
		return material;
	}

	private Mesh GenerateMesh(int rowVertices, int columnVertices, Rect subsection)
	{
		List<Vector3> list = new List<Vector3>();
		List<Vector2> list2 = new List<Vector2>();
		for (int i = 0; i < columnVertices; i++)
		{
			for (int j = 0; j < rowVertices; j++)
			{
				Vector2 vector = new Vector2((float)j / ((float)rowVertices - 1f), (float)i / ((float)columnVertices - 1f));
				Vector2 vector2 = subsection.min + subsection.size * vector;
				Vector3 terrainPoint = GetTerrainPoint(vector2);
				list.Add(terrainPoint);
				list2.Add(vector2);
			}
		}
		List<int> list3 = new List<int>();
		for (int k = 0; k < columnVertices - 1; k++)
		{
			for (int l = 0; l < rowVertices - 1; l++)
			{
				int item = k * rowVertices + l;
				int item2 = k * rowVertices + l + 1;
				int item3 = k * rowVertices + rowVertices + l;
				int item4 = k * rowVertices + rowVertices + l + 1;
				list3.Add(item);
				list3.Add(item3);
				list3.Add(item2);
				list3.Add(item2);
				list3.Add(item3);
				list3.Add(item4);
			}
		}
		Mesh mesh = new Mesh();
		mesh.vertices = list.ToArray();
		mesh.triangles = list3.ToArray();
		mesh.uv = list2.ToArray();
		if (simplifierQuality < 1f)
		{
			MeshSimplifier meshSimplifier = new MeshSimplifier();
			meshSimplifier.PreserveBorderEdges = true;
			meshSimplifier.Initialize(mesh);
			meshSimplifier.SimplifyMesh(simplifierQuality);
			mesh = meshSimplifier.ToMesh();
			MeshSimplifier meshSimplifier2 = new MeshSimplifier();
			meshSimplifier2.PreserveBorderEdges = true;
			meshSimplifier2.Initialize(mesh);
			meshSimplifier2.SimplifyMeshLossless();
			mesh = meshSimplifier2.ToMesh();
		}
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();
		mesh.RecalculateBounds();
		return mesh;
	}

	private Vector3 GetTerrainPoint(Vector2 percent)
	{
		Vector2 vector = Vector2.Scale(new Vector2(terrain.terrainData.size.x, terrain.terrainData.size.z), percent);
		Vector3 vector2 = terrain.GetPosition() + new Vector3(vector.x, 0f, vector.y);
		float y = terrain.SampleHeight(vector2);
		return vector2.SetY(y);
	}
}
