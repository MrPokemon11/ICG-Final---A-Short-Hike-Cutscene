using System.Collections.Generic;
using UnityEngine;

public class TimeScaleController : Singleton<TimeScaleController>
{
	private const float TIME_SCALE_CHANGE_SPEED = 1000f;

	private float slowMoScale;

	private float slowMoDuration;

	private float timeScaleChangeSpeed = 1000f;

	private float initalFixedDeltaTime;

	private SortedList<StackResourceSortingKey, float> timeScaleAdjustments = new SortedList<StackResourceSortingKey, float>();

	private IList<float> timeScaleListCache;

	public float baseTimeScale { get; set; }

	private void Awake()
	{
		initalFixedDeltaTime = Time.fixedDeltaTime;
		timeScaleListCache = timeScaleAdjustments.Values;
		baseTimeScale = 1f;
	}

	private void Update()
	{
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		if (slowMoDuration > 0f)
		{
			slowMoDuration = Mathf.MoveTowards(slowMoDuration, 0f, unscaledDeltaTime);
			Time.timeScale = Mathf.MoveTowards(Time.timeScale, slowMoScale, timeScaleChangeSpeed * unscaledDeltaTime);
			Time.fixedDeltaTime = Mathf.LerpUnclamped(0f, initalFixedDeltaTime, Time.timeScale);
			return;
		}
		float num = baseTimeScale;
		if (timeScaleAdjustments.Count > 0)
		{
			num *= timeScaleListCache[0];
		}
		if (Time.timeScale != num)
		{
			Time.timeScale = Mathf.MoveTowards(Time.timeScale, num, timeScaleChangeSpeed * unscaledDeltaTime);
			if (Time.timeScale == num)
			{
				timeScaleChangeSpeed = 1000f;
			}
		}
		float num2 = Mathf.Min(initalFixedDeltaTime, Mathf.LerpUnclamped(0f, initalFixedDeltaTime, Time.timeScale));
		if (Time.fixedDeltaTime != num2)
		{
			Time.fixedDeltaTime = num2;
		}
	}

	public StackResourceSortingKey AdjustTimeScale(float timeScale)
	{
		StackResourceSortingKey stackResourceSortingKey = new StackResourceSortingKey(0, delegate(StackResourceSortingKey key)
		{
			timeScaleAdjustments.Remove(key);
		});
		timeScaleAdjustments.Add(stackResourceSortingKey, timeScale);
		return stackResourceSortingKey;
	}

	public void SlowMo(float slowTimeScale, float duration, float? timeChangeSpeed = null)
	{
		slowMoScale = slowTimeScale;
		slowMoDuration = duration;
		if (timeChangeSpeed.HasValue)
		{
			timeScaleChangeSpeed = timeChangeSpeed.Value;
		}
	}
}
