using System.Collections.Generic;
using UnityEngine;

public class BorderLine : MonoBehaviour
{
	public static List<BorderLine> allLines = new List<BorderLine>();

	public Vector3 otherPoint;

	private Vector3 toOther;

	private float toOtherLength;

	public Vector3 normal { get; private set; }

	private void OnEnable()
	{
		allLines.Add(this);
		toOther = (otherPoint - base.transform.position).SetY(0f);
		toOtherLength = toOther.magnitude;
		normal = Vector3.Cross(toOther.normalized, Vector3.up);
	}

	private void OnDisable()
	{
		allLines.Remove(this);
	}

	public bool OutsideBorder(Vector3 point)
	{
		Vector3 lhs = (point - base.transform.position).SetY(0f);
		if (Vector3.Cross(lhs, toOther).y <= 0f)
		{
			float num = Vector3.Dot(lhs, toOther) / toOtherLength / toOtherLength;
			if (num >= 0f)
			{
				return num <= 1f;
			}
			return false;
		}
		return false;
	}

	private void OnDrawGizmos()
	{
		Vector3 vector = Vector3.Cross((otherPoint - base.transform.position).normalized, Vector3.up);
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(base.transform.position, otherPoint);
		Gizmos.DrawWireSphere(base.transform.position, 1f);
		Gizmos.DrawWireSphere(otherPoint, 1f);
		Gizmos.DrawLine(base.transform.position, base.transform.position + vector * 50f);
		Gizmos.DrawLine(otherPoint, otherPoint + vector * 50f);
	}
}
