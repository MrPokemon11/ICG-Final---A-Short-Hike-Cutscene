using UnityEngine;

public class LockRotation : MonoBehaviour
{
	private Quaternion startRotation;

	private void Start()
	{
		startRotation = base.transform.rotation;
	}

	private void Update()
	{
		base.transform.rotation = startRotation;
	}
}
