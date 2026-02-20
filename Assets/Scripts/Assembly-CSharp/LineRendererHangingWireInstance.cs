using UnityEngine;

public class LineRendererHangingWireInstance : IHangingWireInstance, IUpdateableWireInstance
{
	public const float MIN_PIXEL_WIDTH = 960f;

	private HangingWireRenderer obj;

	private LineRenderer line;

	private Vector3[] points;

	public LineRendererHangingWireInstance(HangingWireRenderer obj)
	{
		this.obj = obj;
	}

	public void Enable()
	{
		points = new Vector3[obj.nodes];
		line = obj.GetComponent<LineRenderer>();
		if (line == null)
		{
			line = obj.gameObject.AddComponent<LineRenderer>();
			line.positionCount = obj.nodes;
			line.material = obj.lineRendererMaterial;
			HangingWireRenderer.UpdatePoints(points, obj);
			line.useWorldSpace = true;
			line.SetPositions(points);
		}
		line.enabled = true;
		line.widthCurve = AnimationCurve.Constant(0f, 1f, 0.035f * ((float)PixelFilterAdjuster.realPixelWidth / 960f));
		ManualHangingWireUpdater.Initalize().objects.Add(this);
	}

	public void Disable()
	{
		line.enabled = false;
		if (ManualHangingWireUpdater.singleton != null)
		{
			ManualHangingWireUpdater.singleton.objects.Remove(this);
		}
	}

	public void ManualUpdate()
	{
		if (line.isVisible)
		{
			HangingWireRenderer.UpdatePoints(points, obj);
			line.SetPositions(points);
		}
	}

	public void UpdateEndPoint()
	{
	}
}
