using UnityEngine;

public class ExtendedQualitySettingsConfiguration : QualitySettingsConfiguration
{
	public int baseQualityLevel;

	private ImageEffectsSettingsController.Effect imageEffects;

	public static ExtendedQualitySettingsConfiguration CopyCurrentSettings(ExtendedQualitySettingsConfiguration copyTo)
	{
		QualitySettingsConfiguration.CopyCurrentSettings(copyTo);
		copyTo.baseQualityLevel = QualitySettings.GetQualityLevel();
		copyTo.imageEffects = ExtendedQualitySettings.imageEffects;
		return copyTo;
	}

	public override void SetQualitySettings()
	{
		base.SetQualitySettings();
		ExtendedQualitySettings.baseQualityLevel = baseQualityLevel;
		ExtendedQualitySettings.imageEffects = imageEffects;
	}
}
