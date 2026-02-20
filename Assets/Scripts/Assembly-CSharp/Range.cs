using System;
using UnityEngine;

[Serializable]
public struct Range
{
	public float min;

	public float max;

	public Range(float min, float max)
	{
		this.max = max;
		this.min = min;
	}

	public float Random()
	{
		return UnityEngine.Random.Range(min, max);
	}

	public float InverseLerp(float value)
	{
		return Mathf.InverseLerp(min, max, value);
	}

	public float Lerp(float value)
	{
		return Mathf.Lerp(min, max, value);
	}
}
