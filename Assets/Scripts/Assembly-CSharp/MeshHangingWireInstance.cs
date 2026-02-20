using System.Linq;
using UnityEngine;

public class MeshHangingWireInstance : IHangingWireInstance, IUpdateableWireInstance
{
	private HangingWireRenderer obj;

	private Camera mainCamera;

	private Vector3[] points;

	private Mesh mesh;

	private bool calculatedBounds;

	public bool isVisible { get; private set; }

	public MeshHangingWireInstance(HangingWireRenderer obj)
	{
		this.obj = obj;
	}

	public void Enable()
	{
		points = new Vector3[obj.nodes];
		mainCamera = Camera.main;
		mesh = new Mesh();
		mesh.vertices = points;
		mesh.SetIndices(Enumerable.Range(0, points.Length).ToArray(), MeshTopology.LineStrip, 0);
		ManualHangingWireUpdater.Initalize().objects.Add(this);
	}

	public void Disable()
	{
		if (ManualHangingWireUpdater.singleton != null)
		{
			ManualHangingWireUpdater.singleton.objects.Remove(this);
		}
	}

	public void UpdateEndPoint()
	{
	}

	public void ManualUpdate()
	{
		if (obj.isActiveAndEnabled)
		{
			UpdateVisibility();
			UpdatePoints();
			if (isVisible)
			{
				Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, obj.material, 0);
			}
		}
	}

	private void UpdateVisibility()
	{
		isVisible = false;
		Vector3 position = obj.transform.position;
		if (!((position - mainCamera.transform.position).sqrMagnitude > 160000f))
		{
			Vector3 vector = position;
			Vector3 endPosition = obj.endPosition;
			Vector3 vector2 = mainCamera.WorldToViewportPoint(vector);
			Vector3 vector3 = mainCamera.WorldToViewportPoint(endPosition);
			Vector3 vector4 = mainCamera.WorldToViewportPoint((vector + endPosition) * 0.5f + Vector3.down * obj.droopHeight);
			if (vector2.z > 0f || vector3.z > 0f || vector4.z > 0f)
			{
				bool flag = (vector2.x < 0f && vector3.x < 0f && vector4.x < 0f) || (vector2.y < 0f && vector3.y < 0f && vector4.y < 0f) || (vector2.x > 1f && vector3.x > 1f && vector4.x > 1f) || (vector2.y > 1f && vector3.y > 1f && vector4.y > 1f);
				isVisible = !flag;
			}
		}
	}

	private void UpdatePoints()
	{
		if (points == null || points.Length != obj.nodes)
		{
			points = new Vector3[obj.nodes];
		}
		if (!Application.isPlaying || isVisible)
		{
			HangingWireRenderer.UpdatePoints(points, obj);
			mesh.vertices = points;
			if (!calculatedBounds)
			{
				mesh.RecalculateBounds();
				calculatedBounds = true;
			}
		}
	}
}
