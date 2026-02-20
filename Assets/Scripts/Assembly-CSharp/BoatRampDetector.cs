using UnityEngine;

public class BoatRampDetector : MonoBehaviour
{
	public BoatScripting boatScripting;

	private void OnTriggerEnter(Collider other)
	{
		Motorboat component = other.GetComponent<Motorboat>();
		if (component != null && component.mounted)
		{
			boatScripting.StartCoroutine(boatScripting.SlowMotionRampRoutine());
		}
	}
}
