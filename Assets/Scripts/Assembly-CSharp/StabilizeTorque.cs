using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StabilizeTorque : MonoBehaviour
{
	public Vector3 upDirection;

	public float upTorque;

	public Vector3 forwardDirection;

	public float forwardTorque;

	private Rigidbody body;

	private void Start()
	{
		body = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		Vector3 torque = Vector3.Cross(base.transform.up, upDirection) * upTorque;
		body.AddTorque(torque);
		torque = Vector3.Cross(base.transform.forward, forwardDirection) * forwardTorque;
		body.AddTorque(torque);
	}
}
