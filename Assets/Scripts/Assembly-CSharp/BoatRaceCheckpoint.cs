using UnityEngine;

public class BoatRaceCheckpoint : MonoBehaviour
{
	public float disappearTime;

	public float disappearDistance = 6f;

	public AnimationCurve disappearCurve;

	private Rigidbody body;

	private Collider myCollider;

	private Vector3 startPos;

	private float disappearCountdown;

	public bool isFinished { get; private set; }

	private void Awake()
	{
		startPos = base.transform.position;
		body = GetComponent<Rigidbody>();
		myCollider = GetComponent<Collider>();
		body.Sleep();
	}

	private void Update()
	{
		if (isFinished)
		{
			if (disappearCountdown > 0f)
			{
				float time = 1f - disappearCountdown / disappearTime;
				base.transform.position = base.transform.position.SetY(startPos.y - disappearDistance * disappearCurve.Evaluate(time));
				body.position = base.transform.position;
				disappearCountdown -= Time.deltaTime;
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}

	public void Reset()
	{
		isFinished = false;
		base.transform.position = startPos;
		base.gameObject.SetActive(value: true);
		body.interpolation = RigidbodyInterpolation.None;
		body.isKinematic = true;
		myCollider.enabled = false;
	}

	public void Awaken()
	{
		body.isKinematic = false;
		myCollider.enabled = true;
	}

	public void Finished()
	{
		if (!isFinished)
		{
			body.interpolation = RigidbodyInterpolation.Interpolate;
			isFinished = true;
			disappearCountdown = disappearTime;
		}
	}
}
