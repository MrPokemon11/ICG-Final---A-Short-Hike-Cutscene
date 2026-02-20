using System;
using UnityEngine;

public class BaseResolutionHandler : MonoBehaviour
{
	private static int width = 384;

	private static int height = 216;

	private static RenderTexture baseResolutionTarget = null;

	public static event Action onBaseResolutionChanged;

	public static void SetBaseResolution(int width, int height)
	{
		bool num = BaseResolutionHandler.width != width || BaseResolutionHandler.height != height;
		BaseResolutionHandler.width = width;
		BaseResolutionHandler.height = height;
		UpdateBaseRenderTexture(width, height);
		PixelFilterAdjuster pixelFilterAdjuster = Singleton<ServiceLocator>.instance.Locate<PixelFilterAdjuster>(allowFail: true);
		if (pixelFilterAdjuster != null)
		{
			pixelFilterAdjuster.originalRenderTexture = baseResolutionTarget;
		}
		else
		{
			Camera.main.targetTexture = baseResolutionTarget;
			GameObject.FindGameObjectWithTag("RenderQuad").GetComponent<Renderer>().material.mainTexture = baseResolutionTarget;
			GameObject gameObject = GameObject.FindGameObjectWithTag("UICamera");
			if ((bool)gameObject)
			{
				gameObject.GetComponent<Camera>().targetTexture = baseResolutionTarget;
			}
		}
		int pixelWidth = GameSettings.pixelWidth;
		if (pixelWidth > 0)
		{
			pixelWidth = PixelFilterAdjuster.GetPixelWidths().MinValue((PixelFilterAdjuster.PixelSize p) => Mathf.Abs(p.width - PixelFilterAdjuster.pixelWidth)).width;
		}
		GameSettings.pixelWidth = pixelWidth;
		if (num)
		{
			BaseResolutionHandler.onBaseResolutionChanged?.Invoke();
		}
	}

	private static void UpdateBaseRenderTexture(int width, int height)
	{
		if (baseResolutionTarget != null)
		{
			if (baseResolutionTarget.width == width && baseResolutionTarget.height == height)
			{
				return;
			}
			baseResolutionTarget.Release();
		}
		baseResolutionTarget = new RenderTexture(width, height, 16);
		baseResolutionTarget.filterMode = ((width > Screen.width) ? FilterMode.Bilinear : FilterMode.Point);
	}
}
