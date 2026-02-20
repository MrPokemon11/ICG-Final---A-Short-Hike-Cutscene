using System;
using System.Collections;
using System.Collections.Generic;

public class EventList<T> : IEnumerable<T>, IEnumerable
{
	private List<T> list;

	public event Action onModified;

	public EventList()
	{
		list = new List<T>();
	}

	public void Add(T item)
	{
		list.Add(item);
		if (this.onModified != null)
		{
			this.onModified();
		}
	}

	public void Remove(T item)
	{
		list.Remove(item);
		if (this.onModified != null)
		{
			this.onModified();
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		return list.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return list.GetEnumerator();
	}
}
