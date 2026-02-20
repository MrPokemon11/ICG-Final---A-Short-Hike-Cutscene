using UnityEngine;

[ExecuteInEditMode]
public class GLLineRenderer : AbstractGLLineRenderer
{
	public Vector3[] points = new Vector3[0];

	private Vector3[] pointsCache;

	public override bool isVisible => true;

	private void Awake()
	{
		UpdatePointTransforms();
	}

	private void OnValidate()
	{
		EnsureAddedToRenderQueue();
		UpdatePointTransforms();
	}

	private void Update()
	{
		if (base.transform.hasChanged)
		{
			UpdatePointTransforms();
			base.transform.hasChanged = false;
		}
	}

	private void UpdatePointTransforms()
	{
		if (pointsCache == null || pointsCache.Length != points.Length)
		{
			pointsCache = new Vector3[points.Length];
		}
		for (int i = 0; i < points.Length; i++)
		{
			pointsCache[i] = base.transform.TransformPoint(points[i]);
		}
	}

	public override Vector3[] GetPoints()
	{
		return pointsCache;
	}
}
