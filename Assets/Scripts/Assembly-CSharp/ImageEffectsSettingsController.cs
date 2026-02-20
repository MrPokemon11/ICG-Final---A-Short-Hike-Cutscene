using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class ImageEffectsSettingsController : MonoBehaviour
{
	[Flags]
	public enum Effect
	{
		EdgeDetection = 1,
		ColorCorrection = 2,
		All = -1
	}

	private static Dictionary<Effect, Type> EFFECT_TYPES = new Dictionary<Effect, Type>
	{
		{
			Effect.EdgeDetection,
			typeof(EdgeDetection)
		},
		{
			Effect.ColorCorrection,
			typeof(ColorCorrectionCurves)
		}
	};

	private static Effect _allowedEffects = Effect.All;

	public static Effect allowedEffects
	{
		get
		{
			return _allowedEffects;
		}
		set
		{
			_allowedEffects = value;
			UpdateAllEffectSettings();
		}
	}

	public static bool IsEffectAllowed(Effect type)
	{
		return (type & allowedEffects) == type;
	}

	public static void UpdateAllEffectSettings()
	{
		ImageEffectsSettingsController[] array = UnityEngine.Object.FindObjectsOfType<ImageEffectsSettingsController>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateActivatedCameraEffects();
		}
	}

	private void Awake()
	{
		UpdateActivatedCameraEffects();
	}

	public void UpdateActivatedCameraEffects()
	{
		foreach (Effect key in EFFECT_TYPES.Keys)
		{
			IEnumerable<MonoBehaviour> enumerable = GetComponents(EFFECT_TYPES[key]).Cast<MonoBehaviour>();
			bool flag = IsEffectAllowed(key);
			foreach (MonoBehaviour item in enumerable)
			{
				SetIfNotNull(item, flag);
			}
		}
		EdgeDetection component = GetComponent<EdgeDetection>();
		if (component != null)
		{
			component.sampleDist = Mathf.Ceil((float)(PixelFilterAdjuster.realPixelWidth + 1) / 960f);
			component.edgeExp = ((PixelFilterAdjuster.realPixelWidth < 960) ? 0.8f : 0.6f);
		}
	}

	private void SetIfNotNull(MonoBehaviour behaviour, bool enabled)
	{
		if (behaviour != null)
		{
			behaviour.enabled = enabled;
		}
	}
}
