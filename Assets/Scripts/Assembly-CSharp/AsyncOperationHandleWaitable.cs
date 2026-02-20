using UnityEngine.ResourceManagement.AsyncOperations;

public class AsyncOperationHandleWaitable : IWaitable
{
	private readonly AsyncOperationHandle handle;

	public bool isCompleted => handle.IsDone;

	public AsyncOperationHandleWaitable(AsyncOperationHandle handle)
	{
		this.handle = handle;
	}
}
