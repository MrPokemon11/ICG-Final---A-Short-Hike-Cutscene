using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class OrbitalCameraRotater : CinemachineExtension
{
	[FormerlySerializedAs("worldCenter")]
	public Transform center;

	public bool reverseDirection;

	protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
	{
		if (stage == CinemachineCore.Stage.Aim)
		{
			UpdateRotation(ref state);
		}
	}

	private void UpdateRotation(ref CameraState state)
	{
		Quaternion rawOrientation = state.RawOrientation;
		Quaternion quaternion = Quaternion.LookRotation((center.position - state.RawPosition).SetY(0f) * ((!reverseDirection) ? 1 : (-1)));
		rawOrientation.eulerAngles = rawOrientation.eulerAngles.SetY(quaternion.eulerAngles.y);
		state.RawOrientation = rawOrientation;
	}
}
