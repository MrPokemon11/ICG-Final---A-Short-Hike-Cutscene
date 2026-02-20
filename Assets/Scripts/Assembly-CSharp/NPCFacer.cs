using UnityEngine;

public class NPCFacer : MonoBehaviour, ICanFace
{
	public float turnSpeed = 10f;

	private Transform faceTarget;

	private Vector3? faceDirection;

	private Vector3 defaultDirection;

	public bool isTurning => faceDirection.HasValue;

	private void Start()
	{
		defaultDirection = base.transform.forward;
	}

	public void FaceDefault()
	{
		faceDirection = defaultDirection;
	}

	public void TurnToFace(Transform target)
	{
		faceDirection = (target.position - base.transform.position).SetY(0f);
	}

	public void TurnToFace(Vector3 direction)
	{
		faceDirection = direction.SetY(0f);
	}

	private void Update()
	{
		if (faceDirection.HasValue)
		{
			float num = Vector3.Angle(base.transform.forward, faceDirection.Value) * Mathf.Sign(Vector3.Cross(base.transform.forward, faceDirection.Value).y);
			base.transform.Rotate(Vector3.up, num * Time.deltaTime * turnSpeed);
			if (Vector3.Angle(base.transform.forward, faceDirection.Value) < 5f)
			{
				faceDirection = null;
			}
		}
	}
}
