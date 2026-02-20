using System;

public class CustomWaitable : IWaitable
{
	private readonly Func<bool> waitUntil;

	public bool isCompleted => waitUntil();

	public CustomWaitable(Func<bool> waitUntil)
	{
		this.waitUntil = waitUntil;
	}
}
