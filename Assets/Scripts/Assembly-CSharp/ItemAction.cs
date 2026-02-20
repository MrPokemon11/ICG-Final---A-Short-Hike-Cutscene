using System;

public struct ItemAction
{
	public string name;

	public Func<bool> action;

	public ItemAction(string name, Func<bool> action)
	{
		this.name = name;
		this.action = action;
	}
}
