using UnityEngine;

public class GlideDraft : MonoBehaviour
{
	public float forwardForce = 100f;

	public float groundedForce = 100f;

	private Player playerInside;

	private void OnTriggerEnter(Collider other)
	{
		Player component = other.GetComponent<Player>();
		if ((bool)component)
		{
			playerInside = component;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((bool)other.GetComponent<Player>())
		{
			playerInside = null;
		}
	}

	private void FixedUpdate()
	{
		if ((bool)playerInside)
		{
			if (playerInside.isGliding)
			{
				playerInside.body.AddForce(base.transform.forward * forwardForce * Time.fixedDeltaTime);
			}
			else
			{
				playerInside.body.AddForce(base.transform.forward * groundedForce * Time.fixedDeltaTime);
			}
		}
	}
}
