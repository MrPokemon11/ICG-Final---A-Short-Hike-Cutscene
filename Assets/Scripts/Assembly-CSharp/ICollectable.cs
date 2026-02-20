using System;

public interface ICollectable
{
	event Action onCollect;

	void Collect();
}
