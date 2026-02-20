using UnityEngine;

public class SimpleAsyncOperationBundle : IAsyncOperationBundle, IWaitable
{
	private AsyncOperation asyncOperation;

	public bool allowSceneActivation
	{
		get
		{
			return asyncOperation.allowSceneActivation;
		}
		set
		{
			asyncOperation.allowSceneActivation = value;
		}
	}

	public float progress => asyncOperation.progress;

	public bool isCompleted => progress >= 0.9f;

	public SimpleAsyncOperationBundle(AsyncOperation asyncOperation)
	{
		this.asyncOperation = asyncOperation;
	}
}
