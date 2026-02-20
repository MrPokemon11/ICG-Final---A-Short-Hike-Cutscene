using UnityEngine;

public class NoFeatherZone : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		Player component = other.gameObject.GetComponent<Player>();
		if ((bool)component)
		{
			component.feathersRegenerate = false;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Player component = other.gameObject.GetComponent<Player>();
		if ((bool)component)
		{
			component.feathersRegenerate = true;
		}
	}
}
