using System;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
	public static List<Timer> recycledTimers = new List<Timer>();

	private float accumulatedTime;

	private Action onComplete;

	private bool usesUnscaledTime;

	private bool hasAutoDestroyOwner;

	private UnityEngine.Object autoDestroyOwner;

	private bool releasedFromManager;

	private bool releasedFromUser;

	public bool isLooped { get; set; }

	public bool isPaused { get; set; }

	public float duration { get; private set; }

	public bool isCancelled { get; private set; }

	public bool isCompleted { get; private set; }

	public static Timer GetTimerObject()
	{
		if (recycledTimers.Count > 0)
		{
			Timer result = recycledTimers[recycledTimers.Count - 1];
			recycledTimers.RemoveAt(recycledTimers.Count - 1);
			return result;
		}
		return new Timer();
	}

	public void Setup(float duration, Action onComplete, bool isLooped, bool useUnscaledTime)
	{
		this.duration = duration;
		this.onComplete = onComplete;
		this.isLooped = isLooped;
		usesUnscaledTime = useUnscaledTime;
		accumulatedTime = 0f;
		isCancelled = false;
		isPaused = false;
		isCompleted = false;
		hasAutoDestroyOwner = false;
		autoDestroyOwner = null;
		releasedFromUser = false;
		releasedFromManager = false;
	}

	private float GetDeltaTime()
	{
		if (usesUnscaledTime)
		{
			return Time.unscaledDeltaTime;
		}
		return Time.deltaTime;
	}

	public void SetAutoDestroyOwner(UnityEngine.Object owner)
	{
		autoDestroyOwner = owner;
		hasAutoDestroyOwner = owner != null;
	}

	public bool IsDone()
	{
		if (!isCompleted && !isCancelled)
		{
			if (hasAutoDestroyOwner)
			{
				return autoDestroyOwner == null;
			}
			return false;
		}
		return true;
	}

	public bool IsDoneOptimized()
	{
		if (!isCompleted)
		{
			return isCancelled;
		}
		return true;
	}

	public void Cancel()
	{
		isCancelled = true;
	}

	public void Update()
	{
		if (isPaused)
		{
			return;
		}
		accumulatedTime += GetDeltaTime();
		if (accumulatedTime >= duration)
		{
			bool flag = !hasAutoDestroyOwner || autoDestroyOwner != null;
			if (flag)
			{
				onComplete();
			}
			if (isLooped && flag)
			{
				accumulatedTime = 0f;
			}
			else
			{
				isCompleted = true;
			}
		}
	}

	public float GetTimeRemaining()
	{
		if (!IsDone())
		{
			return duration - accumulatedTime;
		}
		return 0f;
	}

	public float GetTime()
	{
		return accumulatedTime;
	}

	public float GetPercentageComplete()
	{
		if (isCompleted)
		{
			return 1f;
		}
		return accumulatedTime / duration;
	}

	public static Timer Register(float duration, Action onComplete, bool isLooped = false, bool useUnscaledTime = false, UnityEngine.Object autoCancelObj = null)
	{
		Timer timerObject = GetTimerObject();
		timerObject.Setup(duration, onComplete, isLooped, useUnscaledTime);
		timerObject.SetAutoDestroyOwner(autoCancelObj);
		if (Singleton<TimerServiceLocator>.instance != null && Singleton<TimerServiceLocator>.instance.timerManager != null)
		{
			Singleton<TimerServiceLocator>.instance.timerManager.AddTimer(timerObject);
		}
		return timerObject;
	}

	public static void Cancel(Timer timer)
	{
		timer?.Cancel();
	}

	public static void FinishImmediately(Timer timer)
	{
		if (timer != null && !timer.IsDone())
		{
			timer.onComplete();
			timer.isCompleted = true;
		}
	}

	public static void FlagToRecycle(Timer timer)
	{
		if (timer != null)
		{
			timer.releasedFromUser = true;
			if (timer.releasedFromManager && timer.releasedFromUser)
			{
				timer.Recycle();
			}
		}
	}

	public static void ReleaseFromManager(Timer timer)
	{
		timer.releasedFromManager = true;
		if (timer.releasedFromManager && timer.releasedFromUser)
		{
			timer.Recycle();
		}
	}

	private void Recycle()
	{
		onComplete = null;
		autoDestroyOwner = null;
		recycledTimers.Add(this);
	}
}
