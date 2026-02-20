using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class TaskAsyncOperationBundle : IAsyncOperationBundle
{
	private AsyncOperation sceneAsyncOperation;

	private Task<object>[] tasks;

	private Action<Task<object>>[] onFinishTasks;

	private bool _allowSceneActivation;

	private bool finishedTasks;

	public bool allowSceneActivation
	{
		get
		{
			return _allowSceneActivation;
		}
		set
		{
			_allowSceneActivation = value;
			ActivateSceneIfCan();
		}
	}

	public float progress => sceneAsyncOperation.progress;

	public TaskAsyncOperationBundle(MonoBehaviour owner, AsyncOperation sceneAsyncOperation, Task<object>[] task, Action<Task<object>>[] onFinish)
	{
		this.sceneAsyncOperation = sceneAsyncOperation;
		tasks = task;
		onFinishTasks = onFinish;
		sceneAsyncOperation.allowSceneActivation = false;
		owner.StartCoroutine(CheckStatusRoutine());
	}

	public TaskAsyncOperationBundle(MonoBehaviour owner, AsyncOperation sceneAsyncOperation, Task<object> task, Action<Task<object>> onFinish)
		: this(owner, sceneAsyncOperation, new Task<object>[1] { task }, new Action<Task<object>>[1] { onFinish })
	{
	}

	public TaskAsyncOperationBundle(MonoBehaviour owner, AsyncOperation sceneAsyncOperation)
		: this(owner, sceneAsyncOperation, new Task<object>[0], new Action<Task<object>>[0])
	{
	}

	public IEnumerator CheckStatusRoutine()
	{
		while (true)
		{
			bool flag = false;
			for (int i = 0; i < tasks.Length; i++)
			{
				if (tasks[i].IsCompleted)
				{
					if (onFinishTasks[i] != null)
					{
						onFinishTasks[i](tasks[i]);
						onFinishTasks[i] = null;
					}
				}
				else
				{
					flag = true;
				}
			}
			if (!flag)
			{
				break;
			}
			yield return null;
		}
		finishedTasks = true;
		ActivateSceneIfCan();
	}

	public void ActivateSceneIfCan()
	{
		if (_allowSceneActivation && finishedTasks)
		{
			sceneAsyncOperation.allowSceneActivation = true;
		}
	}
}
