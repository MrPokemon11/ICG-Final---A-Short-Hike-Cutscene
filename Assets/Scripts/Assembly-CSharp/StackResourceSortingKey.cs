using System;

public class StackResourceSortingKey : PrioritySortingKey
{
	private Action<StackResourceSortingKey> releaseResource;

	public StackResourceSortingKey(int priority, Action<StackResourceSortingKey> releaseResource)
		: base(priority)
	{
		Asserts.NotNull(releaseResource);
		this.releaseResource = releaseResource;
	}

	public void ReleaseResource()
	{
		releaseResource(this);
	}

	public static void Release(StackResourceSortingKey key)
	{
		key?.ReleaseResource();
	}
}
