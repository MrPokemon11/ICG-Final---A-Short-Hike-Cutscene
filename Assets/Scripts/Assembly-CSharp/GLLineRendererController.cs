using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GLLineRendererController : MonoBehaviour
{
	public HashSet<AbstractGLLineRenderer> lineRenderers = new HashSet<AbstractGLLineRenderer>();

	private void Awake()
	{
		lineRenderers.Clear();
	}

	private void OnPostRender()
	{
		GL.PushMatrix();
		GL.LoadProjectionMatrix(Camera.main.projectionMatrix);
		foreach (AbstractGLLineRenderer lineRenderer in lineRenderers)
		{
			if (!lineRenderer.isVisible)
			{
				continue;
			}
			if (!lineRenderer.material)
			{
				Debug.LogError("Please Assign a material on the inspector!", lineRenderer);
				continue;
			}
			Vector3[] points = lineRenderer.GetPoints();
			if (points != null)
			{
				lineRenderer.material.SetPass(0);
				GL.Begin(2);
				for (int i = 0; i < points.Length; i++)
				{
					GL.Vertex(points[i]);
				}
				GL.End();
			}
		}
		GL.PopMatrix();
	}
}
