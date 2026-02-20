using UnityEngine;

public class MagnetRigidbody : MonoBehaviour
{
	public float force = 10f;

	private Rigidbody body;

	private LevelController controller;

	private void Start()
	{
		body = GetComponent<Rigidbody>();
		controller = Singleton<ServiceLocator>.instance.Locate<LevelController>();
	}

	private void FixedUpdate()
	{
		if (controller.player != null)
		{
			body.AddForce((controller.player.transform.position - base.transform.position) * force);
		}
	}
}
