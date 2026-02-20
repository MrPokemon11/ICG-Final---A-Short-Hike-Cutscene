using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CleanUpParticles : MonoBehaviour
{
	private ParticleSystem system;

	private void Start()
	{
		system = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if (!system.IsAlive())
		{
			Object.Destroy(base.gameObject);
		}
	}
}
