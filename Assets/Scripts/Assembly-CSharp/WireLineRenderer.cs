using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class WireLineRenderer : MonoBehaviour
{
	public Material material;

	public Vector3 renderPosition;

	public ShadowCastingMode shadowMode;

	private Mesh mesh;

	private int[] indicesArray;

	private bool lineRendererMode;

	private LineRenderer line;

	private void Start()
	{
		UpdatePixelWidth();
		PixelFilterAdjuster.onPixelWidthChanged += UpdatePixelWidth;
	}

	private void OnDestroy()
	{
		PixelFilterAdjuster.onPixelWidthChanged -= UpdatePixelWidth;
	}

	private void UpdatePixelWidth()
	{
		if ((float)PixelFilterAdjuster.realPixelWidth > 960f)
		{
			InitializeLineRenderer();
			lineRendererMode = true;
			line.widthCurve = AnimationCurve.Constant(0f, 1f, 0.035f * ((float)PixelFilterAdjuster.realPixelWidth / 960f));
			line.enabled = true;
		}
		else
		{
			lineRendererMode = false;
			if (line != null)
			{
				line.enabled = false;
			}
		}
	}

	private void InitializeLineRenderer()
	{
		line = GetComponent<LineRenderer>();
		if (line == null)
		{
			line = base.gameObject.AddComponent<LineRenderer>();
			line.material = material;
		}
	}

	private void LateUpdate()
	{
		if (!lineRendererMode)
		{
			Graphics.DrawMesh(mesh, renderPosition, Quaternion.identity, material, 0, Camera.main, 0, null, shadowMode);
		}
	}

	public void SetPoints(Vector3[] points)
	{
		if (lineRendererMode)
		{
			line.positionCount = points.Length;
			line.SetPositions(points);
			return;
		}
		if (mesh == null)
		{
			mesh = new Mesh();
		}
		if (indicesArray == null || indicesArray.Length != points.Length)
		{
			indicesArray = Enumerable.Range(0, points.Length).ToArray();
		}
		mesh.vertices = points;
		mesh.SetIndices(indicesArray, MeshTopology.LineStrip, 0);
		mesh.RecalculateBounds();
	}
}
