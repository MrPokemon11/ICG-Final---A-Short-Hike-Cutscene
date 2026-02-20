using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PixelFilterAdjuster : ServiceMonoBehaviour
{
	public struct PixelSize
	{
		public int width;

		public Func<string> name;

		public int height => Mathf.RoundToInt((float)width * 9f / 16f);

		public PixelSize(int width, Func<string> name)
		{
			this.width = width;
			this.name = name;
		}
	}

	public static readonly List<PixelSize> PIXEL_WIDTHS_4K = new List<PixelSize>
	{
		new PixelSize(384, () => I18n.STRINGS.pixelsBig),
		new PixelSize(480, () => I18n.STRINGS.pixelsMedium),
		new PixelSize(640, () => I18n.STRINGS.pixelsSmall),
		new PixelSize(960, () => I18n.STRINGS.pixelsTiny),
		new PixelSize(1920, () => I18n.STRINGS.pixelsMicro)
	};

	public static readonly List<PixelSize> PIXEL_WIDTHS_1080P = new List<PixelSize>
	{
		new PixelSize(384, () => I18n.STRINGS.pixelsBig),
		new PixelSize(480, () => I18n.STRINGS.pixelsMedium),
		new PixelSize(640, () => I18n.STRINGS.pixelsSmall),
		new PixelSize(960, () => I18n.STRINGS.pixelsTiny)
	};

	public static readonly List<PixelSize> PIXEL_WIDTHS_720P = new List<PixelSize>
	{
		new PixelSize(426, () => I18n.STRINGS.pixelsBig),
		new PixelSize(640, () => I18n.STRINGS.pixelsSmall)
	};

	public const int DEFAULT_PIXEL_WIDTH = 384;

	private static int _PIXEL_WIDTH = 384;

	public MeshRenderer renderQuad;

	public MeshRenderer uiRenderQuad;

	public Camera mainCamera;

	public Camera uiCamera;

	public RenderTexture originalRenderTexture;

	private RenderTexture allocatedTexture;

	private GlobalShaderParameters globalShaders;

	public static int pixelWidth
	{
		get
		{
			return _PIXEL_WIDTH;
		}
		set
		{
			_PIXEL_WIDTH = value;
			ConfigureWidth(value);
		}
	}

	public static int realPixelWidth
	{
		get
		{
			if (pixelWidth > 0)
			{
				return pixelWidth;
			}
			return Screen.width;
		}
	}

	public static event Action onPixelWidthChanged;

	public static List<PixelSize> GetPixelWidths()
	{
		if (Screen.width >= 3840)
		{
			return PIXEL_WIDTHS_4K;
		}
		if (Screen.width == 1280)
		{
			return PIXEL_WIDTHS_720P;
		}
		return PIXEL_WIDTHS_1080P;
	}

	private void Awake()
	{
		globalShaders = UnityEngine.Object.FindObjectOfType<GlobalShaderParameters>();
	}

	private void Start()
	{
		ConfigureWidth(pixelWidth);
	}

	private void OnDestroy()
	{
		if (allocatedTexture != null)
		{
			allocatedTexture.Release();
		}
	}

	public void SetPixelWidth(int width)
	{
		SetPixelSize(width, Mathf.RoundToInt((float)width * 9f / 16f));
	}

	public void SetPixelSize(int width, int height)
	{
		if (width != originalRenderTexture.width)
		{
			if (allocatedTexture == null || allocatedTexture.width != width || allocatedTexture.height != height)
			{
				if (allocatedTexture != null)
				{
					allocatedTexture.Release();
				}
				allocatedTexture = new RenderTexture(width, height, 24);
				allocatedTexture.filterMode = ((width > Screen.width) ? FilterMode.Bilinear : FilterMode.Point);
			}
			mainCamera.targetTexture = allocatedTexture;
			renderQuad.material.mainTexture = allocatedTexture;
			uiCamera.targetTexture = originalRenderTexture;
			uiRenderQuad.material.mainTexture = originalRenderTexture;
			uiCamera.clearFlags = CameraClearFlags.Color;
			uiRenderQuad.gameObject.SetActive(value: true);
			if ((bool)globalShaders)
			{
				globalShaders.premultiplyAlpha = 1f;
			}
		}
		else
		{
			if (allocatedTexture != null)
			{
				allocatedTexture.Release();
				allocatedTexture = null;
			}
			mainCamera.targetTexture = originalRenderTexture;
			renderQuad.material.mainTexture = originalRenderTexture;
			uiCamera.targetTexture = originalRenderTexture;
			uiRenderQuad.material.mainTexture = originalRenderTexture;
			uiCamera.clearFlags = CameraClearFlags.Nothing;
			uiRenderQuad.gameObject.SetActive(value: false);
			if ((bool)globalShaders)
			{
				globalShaders.premultiplyAlpha = 0f;
			}
		}
		float x = (float)width / (float)height;
		renderQuad.transform.localScale = renderQuad.transform.localScale.SetX(x);
	}

	private static void ConfigureWidth(int width)
	{
		PixelFilterAdjuster pixelFilterAdjuster = Singleton<ServiceLocator>.instance.Locate<PixelFilterAdjuster>(allowFail: true);
		if ((bool)pixelFilterAdjuster)
		{
			if (width <= 0)
			{
				pixelFilterAdjuster.SetPixelWidth(Screen.width);
			}
			else
			{
				pixelFilterAdjuster.SetPixelWidth(width);
			}
			ImageEffectsSettingsController.UpdateAllEffectSettings();
			PixelFilterAdjuster.onPixelWidthChanged?.Invoke();
		}
	}

	private static void ConfigureScale(float scale)
	{
		PixelFilterAdjuster pixelFilterAdjuster = Singleton<ServiceLocator>.instance.Locate<PixelFilterAdjuster>(allowFail: true);
		if ((bool)pixelFilterAdjuster)
		{
			if (scale <= 0f)
			{
				pixelFilterAdjuster.SetPixelWidth(Screen.width);
			}
			else
			{
				pixelFilterAdjuster.SetPixelWidth(Mathf.RoundToInt(384f * scale));
			}
		}
	}

	public static string GetScaleString(float scale)
	{
		if (scale < 0f)
		{
			string text = (string.IsNullOrEmpty(OptionsMenu.OPTIONS_RED_HEX) ? "red" : OptionsMenu.OPTIONS_RED_HEX);
			return "max <color=" + text + ">(not recommended)</color>";
		}
		if (scale != 1f)
		{
			return scale * 100f + "%";
		}
		return "default";
	}

	public static string GetWidthString(int width)
	{
		if (width <= 0)
		{
			return I18n.STRINGS.minimum;
		}
		PixelSize pixelSize = GetPixelWidths().FirstOrDefault((PixelSize p) => p.width == width);
		if (pixelSize.width == 0)
		{
			return "??? (" + width + ")";
		}
		return pixelSize.name();
	}
}
