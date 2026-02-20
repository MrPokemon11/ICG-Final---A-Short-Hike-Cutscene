using UnityEngine;

public class RestoreFeatherZone : MonoBehaviour
{
	public float restoreFeatherSpeed = 1f;

	private Player playerInside;

	private void Update()
	{
		if ((bool)playerInside)
		{
			playerInside.RegainFeatherStamina(restoreFeatherSpeed * Time.deltaTime);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Player component = other.gameObject.GetComponent<Player>();
		if ((bool)component)
		{
			playerInside = component;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.GetComponent<Player>() == playerInside)
		{
			playerInside = null;
		}
	}
}
