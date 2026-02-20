using Cinemachine;
using QuickUnityTools.Input;
using UnityEngine;

[ExecuteInEditMode]
public class GameVirtualCameraExtension : CinemachineExtension
{
	public const float MAX_MOUSE_SUM = 10f;

	public static readonly Vector2 LOOK_ANGLE = new Vector2(15f, 10f);

	public bool allowLookAround = true;

	public float customLowQualityDrawDistance = -1f;

	private Vector2 angle;

	private LevelController level;

	private bool hasCustomDrawDistance;

	private Vector2 mouseLookSum;

	private Vector2 smoothedMouseLookVelocity;

	protected override void Awake()
	{
		base.Awake();
		hasCustomDrawDistance = GetComponent<CustomDrawDistanceCameraExtension>() != null;
		if (Application.isPlaying)
		{
			level = Singleton<GameServiceLocator>.instance.levelController;
		}
	}

	protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
	{
		if (stage == CinemachineCore.Stage.Body)
		{
			PerformLookAround(vcam, stage, ref state, deltaTime);
			if (!hasCustomDrawDistance)
			{
				AdjustDrawDistance(ref state);
			}
		}
	}

	private void AdjustDrawDistance(ref CameraState state)
	{
		if (GameSettings.useLowQualityOptimizations)
		{
			LensSettings lens = state.Lens;
			lens.FarClipPlane = ((customLowQualityDrawDistance > 0f) ? customLowQualityDrawDistance : (0.5f * lens.FarClipPlane));
			state.Lens = lens;
		}
	}

	private void PerformLookAround(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		Transform transform = ((vcam.LookAt != null) ? vcam.LookAt : vcam.Follow);
		if (transform == null)
		{
			return;
		}
		Vector2 input = Vector2.zero;
		if (level.player != null)
		{
			input = level.player.input.GetLookDirection();
			if (!level.player.input.hasFocus && level.player.isMounted)
			{
				input = level.player.mountedVehicle.input.rightStick.vector;
			}
		}
		if (GameUserInput.sharedActionSet.LastInputType.IsMouseOrKeyboard())
		{
			input = SmoothMouseInput(input, deltaTime);
		}
		angle = Vector2.Lerp(b: new Vector2(input.x * (0f - LOOK_ANGLE.x), input.y * (0f - LOOK_ANGLE.y)), a: angle, t: deltaTime * 10f);
		if (!((angle - Vector2.zero).sqrMagnitude < 0.01f))
		{
			Quaternion quaternion = Quaternion.AngleAxis(angle.y, state.CorrectedOrientation * Vector3.right) * Quaternion.Euler(0f, angle.x, 0f);
			Vector3 vector = state.FinalPosition - transform.transform.position;
			Vector3 vector2 = quaternion * vector;
			state.PositionCorrection = vector2 - vector;
			state.OrientationCorrection = Quaternion.Inverse(state.CorrectedOrientation) * quaternion * state.CorrectedOrientation;
		}
	}

	private Vector2 SmoothMouseInput(Vector2 input, float deltaTime)
	{
		mouseLookSum += input;
		if (mouseLookSum.sqrMagnitude > 100f)
		{
			mouseLookSum = mouseLookSum.normalized * 10f;
		}
		mouseLookSum = Vector2.SmoothDamp(mouseLookSum, Vector2.zero, ref smoothedMouseLookVelocity, 0.5f, float.MaxValue, deltaTime);
		return mouseLookSum * 0.1f;
	}
}
