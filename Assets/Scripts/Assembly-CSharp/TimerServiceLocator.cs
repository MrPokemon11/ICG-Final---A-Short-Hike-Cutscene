using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerServiceLocator : AbstractServiceLocator<TimerServiceLocator>
{
	public class TimerManager : ServiceMonoBehaviour
	{
		private List<Timer> timers = new List<Timer>();

		private List<Timer> timersToAddBuffer = new List<Timer>();

		private List<Timer> timersToRemoveBuffer = new List<Timer>();

		public event Action onTimerError;

		private void Update()
		{
			UpdateAllRegisteredTimers();
		}

		private void OnDestroy()
		{
			CancelAllRegisteredTimers();
		}

		public void AddTimer(Timer timer)
		{
			timersToAddBuffer.Add(timer);
		}

		private void UpdateAllRegisteredTimers()
		{
			timers.AddRange(timersToAddBuffer);
			if (timersToAddBuffer.Count > 0)
			{
				timersToAddBuffer.Clear();
			}
			for (int i = 0; i < timers.Count; i++)
			{
				Timer timer = timers[i];
				if (!timer.IsDoneOptimized())
				{
					try
					{
						timer.Update();
					}
					catch (Exception exception)
					{
						timersToRemoveBuffer.Add(timer);
						Debug.LogError("Threw away broken timer!");
						Debug.LogException(exception);
						this.onTimerError?.Invoke();
					}
				}
				else
				{
					timersToRemoveBuffer.Add(timer);
				}
			}
			foreach (Timer item in timersToRemoveBuffer)
			{
				timers.Remove(item);
				Timer.ReleaseFromManager(item);
			}
			timersToRemoveBuffer.Clear();
		}

		private void CancelAllRegisteredTimers()
		{
			foreach (Timer timer in timers)
			{
				timer.Cancel();
			}
			timers.Clear();
		}
	}

	public TimerManager timerManager => LocateOrCreateServiceInActiveScene<TimerManager>();
}
