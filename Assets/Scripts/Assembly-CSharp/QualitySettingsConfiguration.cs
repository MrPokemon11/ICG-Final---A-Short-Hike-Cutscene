using UnityEngine;

public class QualitySettingsConfiguration
{
	private int pixelLightCount;

	private int masterTextureLimit;

	private int textureQuality;

	private AnisotropicFiltering anisotropicFiltering;

	private int antiAliasing;

	private bool softParticles;

	private bool realtimeReflectionProbes;

	private bool billboardsFaceCameraPosition;

	private float resolutionScalingFixedDPIFactor;

	private bool streamingMipmapsActive;

	private float streamingMipmapsMemoryBudget;

	private ShadowmaskMode shadowmaskMode;

	private ShadowQuality shadows;

	private ShadowResolution shadowResolution;

	private ShadowProjection shadowProjection;

	private float shadowDistance;

	private float shadowNearPlaneOffset;

	private int shadowCascades;

	private float shadowCascade2Split;

	private Vector3 shadowCascade4Split;

	private SkinWeights blendWeights;

	private int vSyncCount;

	private float lodBias;

	private int maximumLODLevel;

	private int particleRaycastBudget;

	private int asyncUploadBufferSize;

	private int asyncUploadTimeSlice;

	private bool asyncUploadPersistentBuffer;

	public static QualitySettingsConfiguration CopyCurrentSettings(QualitySettingsConfiguration copyTo)
	{
		copyTo.pixelLightCount = QualitySettings.pixelLightCount;
		copyTo.masterTextureLimit = QualitySettings.globalTextureMipmapLimit;
		copyTo.anisotropicFiltering = QualitySettings.anisotropicFiltering;
		copyTo.antiAliasing = QualitySettings.antiAliasing;
		copyTo.softParticles = QualitySettings.softParticles;
		copyTo.realtimeReflectionProbes = QualitySettings.realtimeReflectionProbes;
		copyTo.billboardsFaceCameraPosition = QualitySettings.billboardsFaceCameraPosition;
		copyTo.resolutionScalingFixedDPIFactor = QualitySettings.resolutionScalingFixedDPIFactor;
		copyTo.streamingMipmapsActive = QualitySettings.streamingMipmapsActive;
		copyTo.streamingMipmapsMemoryBudget = QualitySettings.streamingMipmapsMemoryBudget;
		copyTo.shadowmaskMode = QualitySettings.shadowmaskMode;
		copyTo.shadows = QualitySettings.shadows;
		copyTo.shadowResolution = QualitySettings.shadowResolution;
		copyTo.shadowProjection = QualitySettings.shadowProjection;
		copyTo.shadowDistance = QualitySettings.shadowDistance;
		copyTo.shadowNearPlaneOffset = QualitySettings.shadowNearPlaneOffset;
		copyTo.shadowCascades = QualitySettings.shadowCascades;
		copyTo.shadowCascade2Split = QualitySettings.shadowCascade2Split;
		copyTo.shadowCascade4Split = QualitySettings.shadowCascade4Split;
		copyTo.blendWeights = QualitySettings.skinWeights;
		copyTo.vSyncCount = QualitySettings.vSyncCount;
		copyTo.lodBias = QualitySettings.lodBias;
		copyTo.maximumLODLevel = QualitySettings.maximumLODLevel;
		copyTo.particleRaycastBudget = QualitySettings.particleRaycastBudget;
		copyTo.asyncUploadBufferSize = QualitySettings.asyncUploadBufferSize;
		copyTo.asyncUploadTimeSlice = QualitySettings.asyncUploadTimeSlice;
		copyTo.asyncUploadPersistentBuffer = QualitySettings.asyncUploadPersistentBuffer;
		return copyTo;
	}

	public virtual void SetQualitySettings()
	{
		QualitySettings.pixelLightCount = pixelLightCount;
		QualitySettings.globalTextureMipmapLimit = masterTextureLimit;
		QualitySettings.anisotropicFiltering = anisotropicFiltering;
		QualitySettings.antiAliasing = antiAliasing;
		QualitySettings.softParticles = softParticles;
		QualitySettings.realtimeReflectionProbes = realtimeReflectionProbes;
		QualitySettings.billboardsFaceCameraPosition = billboardsFaceCameraPosition;
		QualitySettings.resolutionScalingFixedDPIFactor = resolutionScalingFixedDPIFactor;
		QualitySettings.streamingMipmapsActive = streamingMipmapsActive;
		QualitySettings.streamingMipmapsMemoryBudget = streamingMipmapsMemoryBudget;
		QualitySettings.shadowmaskMode = shadowmaskMode;
		QualitySettings.shadows = shadows;
		QualitySettings.shadowResolution = shadowResolution;
		QualitySettings.shadowProjection = shadowProjection;
		QualitySettings.shadowDistance = shadowDistance;
		QualitySettings.shadowNearPlaneOffset = shadowNearPlaneOffset;
		QualitySettings.shadowCascades = shadowCascades;
		QualitySettings.shadowCascade2Split = shadowCascade2Split;
		QualitySettings.shadowCascade4Split = shadowCascade4Split;
		QualitySettings.skinWeights = blendWeights;
		QualitySettings.vSyncCount = vSyncCount;
		QualitySettings.lodBias = lodBias;
		QualitySettings.maximumLODLevel = maximumLODLevel;
		QualitySettings.particleRaycastBudget = particleRaycastBudget;
		QualitySettings.asyncUploadBufferSize = asyncUploadBufferSize;
		QualitySettings.asyncUploadTimeSlice = asyncUploadTimeSlice;
		QualitySettings.asyncUploadPersistentBuffer = asyncUploadPersistentBuffer;
	}
}
