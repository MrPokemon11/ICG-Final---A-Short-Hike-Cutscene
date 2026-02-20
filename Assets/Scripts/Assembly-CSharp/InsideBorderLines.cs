using UnityEngine;

public class InsideBorderLines : MonoBehaviour
{
	private Rigidbody body;

	public float pushForce = 100f;

	private void Start()
	{
		body = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		Vector3 force = Vector3.zero;
		float num = 0f;
		foreach (BorderLine allLine in BorderLine.allLines)
		{
			if (allLine.OutsideBorder(base.transform.position))
			{
				Vector3 vector = Vector3.ProjectOnPlane(base.transform.position - allLine.transform.position, allLine.normal) + allLine.transform.position;
				float magnitude = (base.transform.position - vector).magnitude;
				Vector3 vector2 = allLine.normal * pushForce * magnitude;
				float sqrMagnitude = vector2.sqrMagnitude;
				if (num == 0f || sqrMagnitude < num)
				{
					num = sqrMagnitude;
					force = vector2;
				}
			}
		}
		if (num > 0f)
		{
			body.AddForce(force, ForceMode.Acceleration);
		}
	}
}
