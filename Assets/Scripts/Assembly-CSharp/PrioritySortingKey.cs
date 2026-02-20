using System;

public class PrioritySortingKey : IComparable
{
	private static int uniqueConsecutiveTimeOrderIds = int.MinValue;

	private int timestamp;

	public int priority { get; private set; }

	public PrioritySortingKey(int priority)
		: this(priority, GetUniqueTime())
	{
	}

	public PrioritySortingKey(int priority, int timestamp)
	{
		this.priority = priority;
		this.timestamp = timestamp;
	}

	public int CompareTo(object obj)
	{
		int num = -1 * priority.CompareTo(((PrioritySortingKey)obj).priority);
		if (num == 0)
		{
			return -1 * timestamp.CompareTo(((PrioritySortingKey)obj).timestamp);
		}
		return num;
	}

	public override string ToString()
	{
		return "(Priority: " + priority + ", Time: " + timestamp + ")";
	}

	public static int GetUniqueTime()
	{
		uniqueConsecutiveTimeOrderIds++;
		return uniqueConsecutiveTimeOrderIds;
	}
}
