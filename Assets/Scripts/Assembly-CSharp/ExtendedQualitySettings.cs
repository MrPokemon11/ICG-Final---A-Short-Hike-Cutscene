using System.Collections.Generic;
using UnityEngine;

public static class ExtendedQualitySettings
{
	private const string EFFECTS_PREF = "saved_effects";

	private const string SHADOWS_PREF = "shadows_settings";

	private const string BASE_LEVEL_PREF = "base_level";

	private const string VSYNC_PREF = "vsync";

	public const int CUSTOM_QUALITY_LEVEL = 5;

	public const int PERFECT_QUALITY_LEVEL = 5;

	private static List<ImageEffectsSettingsController.Effect> EFFECT_DEFAULTS = new List<ImageEffectsSettingsController.Effect>
	{
		(ImageEffectsSettingsController.Effect)0,
		ImageEffectsSettingsController.Effect.ColorCorrection,
		ImageEffectsSettingsController.Effect.EdgeDetection | ImageEffectsSettingsController.Effect.ColorCorrection,
		ImageEffectsSettingsController.Effect.EdgeDetection | ImageEffectsSettingsController.Effect.ColorCorrection,
		ImageEffectsSettingsController.Effect.EdgeDetection | ImageEffectsSettingsController.Effect.ColorCorrection,
		ImageEffectsSettingsController.Effect.EdgeDetection | ImageEffectsSettingsController.Effect.ColorCorrection
	};

	private static int _vSync = 1;

	private static int _baseQualityLevel = 4;

	public static ImageEffectsSettingsController.Effect imageEffects
	{
		get
		{
			return ImageEffectsSettingsController.allowedEffects;
		}
		set
		{
			ImageEffectsSettingsController.allowedEffects = value;
			PlayerPrefsAdapter.SetInt("saved_effects", (int)value);
		}
	}

	public static ShadowQuality shadows
	{
		get
		{
			return QualitySettings.shadows;
		}
		set
		{
			PlayerPrefsAdapter.SetInt("shadows_settings", (int)value);
			QualitySettings.shadows = value;
		}
	}

	public static int vSync
	{
		get
		{
			return _vSync;
		}
		set
		{
			PlayerPrefsAdapter.SetInt("vsync", value);
			QualitySettings.vSyncCount = value;
			_vSync = value;
		}
	}

	public static int baseQualityLevel
	{
		get
		{
			return _baseQualityLevel;
		}
		set
		{
			_baseQualityLevel = value;
			PlayerPrefsAdapter.SetInt("base_level", value);
		}
	}

	public static void EnsureCustomSettings()
	{
		if (QualitySettings.GetQualityLevel() != 5)
		{
			ExtendedQualitySettingsConfiguration extendedQualitySettingsConfiguration = ExtendedQualitySettingsConfiguration.CopyCurrentSettings(new ExtendedQualitySettingsConfiguration());
			QualitySettings.SetQualityLevel(5);
			extendedQualitySettingsConfiguration.SetQualitySettings();
		}
	}

	public static void SynchronizeSettingsWithQualityLevel()
	{
		baseQualityLevel = QualitySettings.GetQualityLevel();
		imageEffects = EFFECT_DEFAULTS[QualitySettings.GetQualityLevel()];
		shadows = QualitySettings.shadows;
		vSync = vSync;
	}

	public static void LoadSettingsPrefs()
	{
		vSync = PlayerPrefsAdapter.GetInt("vsync", 1);
		if (QualitySettings.GetQualityLevel() != 5)
		{
			SynchronizeSettingsWithQualityLevel();
			return;
		}
		baseQualityLevel = PlayerPrefsAdapter.GetInt("base_level", -1);
		if (baseQualityLevel != -1)
		{
			QualitySettings.SetQualityLevel(baseQualityLevel);
		}
		else
		{
			baseQualityLevel = QualitySettings.GetQualityLevel();
		}
		int num = PlayerPrefsAdapter.GetInt("saved_effects", -1);
		imageEffects = ((num != -1) ? ((ImageEffectsSettingsController.Effect)num) : EFFECT_DEFAULTS[QualitySettings.GetQualityLevel()]);
		shadows = (ShadowQuality)PlayerPrefsAdapter.GetInt("shadows_settings", 1);
		vSync = PlayerPrefsAdapter.GetInt("vsync", 1);
	}
}
