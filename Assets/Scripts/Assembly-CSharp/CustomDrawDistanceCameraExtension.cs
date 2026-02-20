using Cinemachine;

public class CustomDrawDistanceCameraExtension : CinemachineExtension
{
	public float lowQualityFarClip = 400f;

	protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
	{
		if (stage == CinemachineCore.Stage.Body)
		{
			AdjustDrawDistance(ref state);
		}
	}

	private void AdjustDrawDistance(ref CameraState state)
	{
		if (GameSettings.useLowQualityOptimizations)
		{
			LensSettings lens = state.Lens;
			lens.FarClipPlane = lowQualityFarClip;
			state.Lens = lens;
		}
	}
}
