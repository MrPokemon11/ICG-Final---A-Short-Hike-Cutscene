using UnityEngine;

public abstract class AbstractGLLineRenderer : MonoBehaviour
{
	public Material material;

	public abstract bool isVisible { get; }

	protected virtual void Start()
	{
		EnsureAddedToRenderQueue();
	}

	protected void EnsureAddedToRenderQueue()
	{
		if (!(Camera.main == null))
		{
			GLLineRendererController gLLineRendererController = Camera.main.GetComponent<GLLineRendererController>();
			if (!gLLineRendererController)
			{
				gLLineRendererController = Camera.main.gameObject.AddComponent<GLLineRendererController>();
			}
			if (!gLLineRendererController.lineRenderers.Contains(this))
			{
				gLLineRendererController.lineRenderers.Add(this);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		if (Camera.main != null)
		{
			GLLineRendererController component = Camera.main.GetComponent<GLLineRendererController>();
			if (component != null)
			{
				component.lineRenderers.Remove(this);
			}
		}
	}

	public abstract Vector3[] GetPoints();

	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = GetGizmoColor();
		Vector3[] points = GetPoints();
		if (points != null)
		{
			for (int i = 0; i < points.Length - 1; i++)
			{
				Gizmos.DrawLine(points[i], points[i + 1]);
			}
		}
	}

	protected virtual Color GetGizmoColor()
	{
		return material.color;
	}
}
