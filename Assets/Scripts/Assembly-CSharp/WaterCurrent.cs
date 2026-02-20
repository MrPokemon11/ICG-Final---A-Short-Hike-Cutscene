using UnityEngine;

public class WaterCurrent : MonoBehaviour
{
	public Vector3[] positions;

	public Vector3[] tangents;

	public Vector3 GetCurrent(Vector3 position)
	{
		int num = 0;
		float num2 = float.MaxValue;
		for (int i = 0; i < positions.Length; i++)
		{
			float sqrMagnitude = (positions[i] - position).sqrMagnitude;
			if (sqrMagnitude < num2)
			{
				num = i;
				num2 = sqrMagnitude;
			}
		}
		float distanceSqrSafe = GetDistanceSqrSafe(position, num - 1);
		float distanceSqrSafe2 = GetDistanceSqrSafe(position, num + 1);
		int num3 = ((distanceSqrSafe < distanceSqrSafe2) ? (num - 1) : (num + 1));
		int num4 = ((num3 < num) ? num3 : num);
		int num5 = ((num3 < num) ? num : num3);
		return (positions[num5] - positions[num4]).normalized;
	}

	private float GetDistanceSqrSafe(Vector3 position, int index)
	{
		if (index < 0 || index > positions.Length - 1)
		{
			return float.MaxValue;
		}
		return (positions[index] - position).sqrMagnitude;
	}
}
