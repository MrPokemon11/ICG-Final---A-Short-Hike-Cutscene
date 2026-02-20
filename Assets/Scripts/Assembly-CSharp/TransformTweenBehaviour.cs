using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class TransformTweenBehaviour : PlayableBehaviour
{
	public enum TweenType
	{
		Linear = 0,
		Deceleration = 1,
		Harmonic = 2,
		Custom = 3
	}

	public Transform startLocation;

	public Transform endLocation;

	public bool tweenPosition = true;

	public bool tweenRotation = true;

	public TweenType tweenType;

	public float customStartingSpeed;

	public float customEndingSpeed;

	public float inverseDuration;

	public Vector3 startingPosition;

	public Quaternion startingRotation = Quaternion.identity;

	public AnimationCurve currentCurve;

	private AnimationCurve m_LinearCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private AnimationCurve m_DecelerationCurve = new AnimationCurve(new Keyframe(0f, 0f, -MathF.PI / 2f, MathF.PI / 2f), new Keyframe(1f, 1f, 0f, 0f));

	private AnimationCurve m_HarmonicCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	private AnimationCurve m_CustomCurve;

	private const float k_RightAngleInRads = MathF.PI / 2f;

	public override void OnGraphStart(Playable playable)
	{
		double duration = playable.GetDuration();
		if (Mathf.Approximately((float)duration, 0f))
		{
			throw new UnityException("A TransformTween cannot have a duration of zero.");
		}
		inverseDuration = 1f / (float)duration;
		m_CustomCurve = new AnimationCurve(new Keyframe(0f, 0f, (0f - customStartingSpeed) * (MathF.PI / 2f), customStartingSpeed * (MathF.PI / 2f)), new Keyframe(1f, 1f, customEndingSpeed * (MathF.PI / 2f), customEndingSpeed * (MathF.PI / 2f)));
		switch (tweenType)
		{
		case TweenType.Linear:
			currentCurve = m_LinearCurve;
			break;
		case TweenType.Deceleration:
			currentCurve = m_DecelerationCurve;
			break;
		case TweenType.Harmonic:
			currentCurve = m_HarmonicCurve;
			break;
		case TweenType.Custom:
			currentCurve = m_CustomCurve;
			break;
		}
		if ((bool)startLocation)
		{
			startingPosition = startLocation.position;
			startingRotation = startLocation.rotation;
		}
	}
}
