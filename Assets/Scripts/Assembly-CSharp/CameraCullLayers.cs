using UnityEngine;

public class CameraCullLayers : MonoBehaviour
{
	public const int CULL_REALLY_EARLY_LAYER = 25;

	public const int CULL_EARLY_LAYER = 24;

	public const int WATERDRIFTERS_LAYER = 18;

	public const int BOBBER_LAYER = 20;

	public const int HOLDABLE_LAYER = 11;

	public const int MOVING_PROP_LAYER = 14;

	public float cullEarlyPercent = 0.7f;

	public float cullReallyEarlyPercent = 0.45f;

	private Camera myCamera;

	private float[] layers = new float[32];

	private void Start()
	{
		myCamera = GetComponent<Camera>();
		this.RegisterTimer(1.1f, UpdateCullLayers, isLooped: true, useUnscaledTime: true);
	}

	private void UpdateCullLayers()
	{
		layers[25] = myCamera.farClipPlane * cullReallyEarlyPercent;
		layers[18] = myCamera.farClipPlane * cullReallyEarlyPercent;
		layers[20] = myCamera.farClipPlane * cullReallyEarlyPercent;
		layers[11] = myCamera.farClipPlane * cullReallyEarlyPercent;
		layers[24] = myCamera.farClipPlane * cullEarlyPercent;
		layers[14] = myCamera.farClipPlane * cullEarlyPercent;
		myCamera.layerCullDistances = layers;
	}
}
