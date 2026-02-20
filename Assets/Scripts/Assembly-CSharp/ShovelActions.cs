using UnityEngine;

public class ShovelActions : MonoBehaviour, IHoldableAction
{
	public float raycastBelow = 1f;

	public float raycastAbove = 2f;

	public LayerMask diggableLayers;

	public LayerMask buriedItemLayers;

	public GameObject shovelHolePrefab;

	public Transform shovelHead;

	public ParticleSystem groundDirtParticles;

	public ParticleSystem groundClashParticles;

	public ParticleSystem dirtThrownParticles;

	public AudioClip clashSound;

	public AudioClip digSound;

	public AudioClip shovelMiss;

	public AudioSource audioSource;

	public void ActivateAction(int parameter)
	{
		if (Physics.Raycast(new Ray(shovelHead.position + Vector3.up * raycastAbove, Vector3.down), out var hitInfo, raycastAbove + raycastBelow, -5, QueryTriggerInteraction.Ignore))
		{
			if ((int)diggableLayers == ((int)diggableLayers | (1 << hitInfo.collider.gameObject.layer)))
			{
				groundDirtParticles.transform.position = hitInfo.point;
				groundDirtParticles.transform.forward = Vector3.up;
				groundDirtParticles.Play();
				dirtThrownParticles.Play();
				shovelHolePrefab.CloneAt(hitInfo.point + Vector3.up * 0.1f);
				DetectBuriedItems(hitInfo.point);
				audioSource.clip = digSound;
				audioSource.Play();
			}
			else
			{
				groundClashParticles.transform.position = hitInfo.point;
				groundClashParticles.transform.forward = Vector3.up;
				groundClashParticles.Play();
				audioSource.clip = clashSound;
				audioSource.Play();
				hitInfo.collider.GetComponent<IShovelWhackable>()?.WhackWithShovel();
			}
		}
		else
		{
			audioSource.clip = shovelMiss;
			audioSource.Play();
		}
	}

	private void DetectBuriedItems(Vector3 atPoint)
	{
		Collider[] array = Physics.OverlapSphere(atPoint, 0.5f, buriedItemLayers.value);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GetComponent<IBuried>()?.Unearth(atPoint);
		}
	}
}
