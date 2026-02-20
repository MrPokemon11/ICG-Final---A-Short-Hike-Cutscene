using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SaveQueue : IWaitable
{
	private MonoBehaviour owner;

	private Coroutine waitCoroutine;

	private Queue<Func<Task>> saveTasks = new Queue<Func<Task>>();

	private List<Action> onFinishCallbacks = new List<Action>();

	public bool isCompleted => waitCoroutine == null;

	public SaveQueue(MonoBehaviour owner)
	{
		this.owner = owner;
	}

	public void OnceDoneSaving(Action action)
	{
		if (waitCoroutine == null)
		{
			action();
		}
		else
		{
			onFinishCallbacks.Add(action);
		}
	}

	public void Enqueue(GlobalData.GameData clone, int bytes)
	{
		Enqueue(() => Task.Run(delegate
		{
			try
			{
				AutoResetEvent token = new AutoResetEvent(initialState: false);
				FileSystem.SaveObject(GlobalData.currentSaveFile, clone, bytes, delegate
				{
					token.Set();
				});
				token.WaitOne();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}));
	}

	public SaveQueue Enqueue(Func<Task> saveTaskGenerator)
	{
		saveTasks.Enqueue(saveTaskGenerator);
		if (waitCoroutine == null)
		{
			waitCoroutine = owner.StartCoroutine(WaitForTasks());
		}
		return this;
	}

	private IEnumerator WaitForTasks()
	{
		while (saveTasks.Count > 0)
		{
			Task task = saveTasks.Dequeue()();
			while (!task.IsCompleted)
			{
				yield return null;
			}
		}
		waitCoroutine = null;
		foreach (Action onFinishCallback in onFinishCallbacks)
		{
			onFinishCallback();
		}
		onFinishCallbacks.Clear();
	}
}
