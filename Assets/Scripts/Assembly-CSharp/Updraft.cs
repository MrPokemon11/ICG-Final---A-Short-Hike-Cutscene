using System;
using UnityEngine;

public class Updraft : MonoBehaviour
{
	public float upwardForce = 100f;

	public float inwardForce = 50f;

	public float movementForce = 40f;

	public float drag = 3f;

	public event Action onRegister;

	public event Action onUnregister;

	private void OnTriggerEnter(Collider other)
	{
		Player component = other.GetComponent<Player>();
		if ((bool)component)
		{
			component.RegisterUpdraft(this);
			this.onRegister?.Invoke();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Player component = other.GetComponent<Player>();
		if ((bool)component)
		{
			component.UnregisterUpdraft(this);
			this.onUnregister?.Invoke();
		}
	}
}
