using System;
using System.Collections;
using UnityEngine;

public static class WaitFor
{
	public static void WithCoroutine(MonoBehaviour coroutineOwner, IWaitable waitable, Action onFinish)
	{
		WithCoroutine(coroutineOwner, onFinish, waitable);
	}

	public static void WithCoroutine(MonoBehaviour coroutineOwner, Action onFinish, params IWaitable[] waitables)
	{
		coroutineOwner.StartCoroutine(WaitForTasks(waitables, onFinish));
	}

	private static IEnumerator WaitForTasks(IWaitable[] waitables, Action onFinish)
	{
		while (true)
		{
			bool flag = true;
			for (int i = 0; i < waitables.Length; i++)
			{
				if (!waitables[i].isCompleted)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				break;
			}
			yield return null;
		}
		onFinish();
	}
}
