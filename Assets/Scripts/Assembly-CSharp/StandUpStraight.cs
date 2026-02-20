using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StandUpStraight : MonoBehaviour
{
	public enum UpVector
	{
		Up = 0,
		Forward = 1,
		Right = 2
	}

	public float standUpTorue = 10f;

	public UpVector upVector;

	public Vector3 worldUpVector = Vector3.up;

	private Rigidbody body;

	private void Start()
	{
		body = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if (!body.isKinematic && !body.IsSleeping())
		{
			Vector3 lhs = Vector3.up;
			switch (upVector)
			{
			case UpVector.Up:
				lhs = base.transform.up;
				break;
			case UpVector.Right:
				lhs = base.transform.right;
				break;
			case UpVector.Forward:
				lhs = base.transform.forward;
				break;
			}
			Vector3 torque = Vector3.Cross(lhs, worldUpVector) * standUpTorue;
			if (torque.sqrMagnitude > 1f)
			{
				body.AddTorque(torque);
			}
		}
	}
}
