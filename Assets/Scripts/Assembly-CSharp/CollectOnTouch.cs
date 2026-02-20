using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CollectOnTouch : MonoBehaviour, ICollectable
{
	private const string COLLECTED_PREFIX = "COLLECTED_";

	private const float PICKUP_COMBO_TIME = 0.3f;

	private const float PICKUP_PITCH_INCREASE = 0.15f;

	private static float lastPickupTime;

	private static int pickupCombo;

	public CollectableItem collectable;

	[FormerlySerializedAs("value")]
	public int amount = 1;

	[FormerlySerializedAs("coinSound")]
	public AudioClip pickUpSound;

	[FormerlySerializedAs("deathParticles")]
	public ParticleSystem pickUpParticles;

	public event Action onCollect;

	private void Start()
	{
		GameObjectID component = GetComponent<GameObjectID>();
		if ((bool)component && component.GetBoolForID("COLLECTED_"))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.gameObject.GetComponent<Player>())
		{
			Collect();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if ((bool)collision.gameObject.GetComponent<Player>())
		{
			Collect();
		}
	}

	public void Collect()
	{
		if (pickUpParticles != null)
		{
			pickUpParticles.gameObject.transform.parent = null;
			pickUpParticles.Play();
		}
		GameObjectID component = GetComponent<GameObjectID>();
		if (component.GetBoolForID("COLLECTED_"))
		{
			Debug.LogWarning("This has already been collected!");
			return;
		}
		if ((bool)component)
		{
			component.SaveBoolForID("COLLECTED_", value: true);
		}
		if (this.onCollect != null)
		{
			this.onCollect();
		}
		if (pickUpSound != null)
		{
			AudioSource audioSource = pickUpSound.Play();
			if (Time.time - lastPickupTime < 0.3f)
			{
				audioSource.pitch = Mathf.Min(2f, 1f + (float)pickupCombo * 0.15f);
				pickupCombo++;
			}
			else
			{
				pickupCombo = 0;
			}
			lastPickupTime = Time.time;
		}
		Singleton<GameServiceLocator>.instance.levelController.player.StartCoroutine(collectable.PickUpRoutine(amount));
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
