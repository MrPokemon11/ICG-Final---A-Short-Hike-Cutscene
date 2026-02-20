using System;
using UnityEngine;

public class CollectOnInteract : MonoBehaviour, IInteractableComponent, ICollectable
{
	private const string COLLECTED_PREFIX = "IntCollect_";

	public CollectableItem collectable;

	public string rememberCollectedTag;

	bool IInteractableComponent.enabled
	{
		get
		{
			return base.enabled;
		}
		set
		{
			base.enabled = value;
		}
	}

	public event Action onCollect;

	private void Start()
	{
		GameObjectID component = GetComponent<GameObjectID>();
		if ((bool)component && component.GetBoolForID("IntCollect_"))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void Interact()
	{
		Singleton<GameServiceLocator>.instance.levelController.player.ikAnimator.PickUp();
		this.RegisterTimer(0.075f, Collect);
	}

	public void Collect()
	{
		if (!(this == null))
		{
			Player player = Singleton<GameServiceLocator>.instance.levelController.player;
			player.pickUpSound.Play();
			player.StartCoroutine(collectable.PickUpRoutine());
			GameObjectID component = GetComponent<GameObjectID>();
			if ((bool)component)
			{
				component.SaveBoolForID("IntCollect_", value: true);
			}
			if (this.onCollect != null)
			{
				this.onCollect();
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
